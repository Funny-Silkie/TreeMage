using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// PDFへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class PdfExporter : IExporter
    {
        private const string FontFamily = "Arial";

        /// <inheritdoc/>
        public ExportType Type => ExportType.Pdf;

        static PdfExporter()
        {
            GlobalFontSettings.FontResolver = new FontResolver();
        }

        /// <summary>
        /// <see cref="PdfExporter"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PdfExporter()
        {
        }

        /// <summary>
        /// PDFオブジェクトを生成します。
        /// </summary>
        /// <param name="tree">描画するツリー</param>
        /// <param name="options">エクスポート時のオプション</param>
        /// <returns><paramref name="tree"/>の図を表すPDFオブジェクト</returns>
        internal static PdfDocument CreatePdf(Tree tree, ExportOptions options)
        {
            var result = new PdfDocument();
            result.Info.Creator = "TreeViewer";
            result.Info.Keywords = "TreeViewer, Phylogeny";
            result.PageLayout = PdfPageLayout.SinglePage;
            PdfPage mainPage = result.AddPage();

            var leafLabelFont = new XFont(FontFamily, tree.Style.LeafLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var cladeLabelFont = new XFont(FontFamily, tree.Style.CladeLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var nodeValuesFont = new XFont(FontFamily, tree.Style.NodeValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var branchValuesFont = new XFont(FontFamily, tree.Style.BranchValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);

            var positionManager = new PositionManager(tree);

            (double pageWidth, double pageHeight) = positionManager.CalcDocumentSize();

            mainPage.Width = pageWidth;
            mainPage.Height = pageHeight;

            #region 系統樹部分

            using (XGraphics graphics = XGraphics.FromPdfPage(mainPage))
            {
                graphics.TranslateTransform(50, 50);

                foreach (Clade current in tree.GetAllClades())
                {
                    if (current.GetIsHidden()) continue;

                    if (!string.IsNullOrEmpty(current.Style.ShadeColor))
                    {
                        (double x, double y, double width, double height) = positionManager.CalcCladeShadePosition(current);

                        graphics.DrawRectangle(DrawHelpers.CreatePdfColor(current.Style.ShadeColor).ToBrush(), x, y, width, height);
                    }

                    if (current.GetIsExternal() && !current.IsLeaf)
                    {
                        var (left, rightTop, rightBottom) = positionManager.CalcCollapseTrianglePositions(current);

                        var path = new XGraphicsPath();
                        path.AddLine(left.x, left.y, rightTop.x, rightTop.y);
                        path.AddLine(rightTop.x, rightTop.y, rightBottom.x, rightBottom.y);
                        path.AddLine(left.x, left.y, rightBottom.x, rightBottom.y);
                        graphics.DrawPath(DrawHelpers.CreatePdfColor(current.Style.BranchColor).ToPen(tree.Style.BranchThickness),
                                          XBrushes.Transparent,
                                          path);
                    }

                    if (current.IsLeaf)
                    {
                        // 系統名
                        if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                        {
                            (double x, double y, _, _) = positionManager.CalcLeafPosition(current);

                            graphics.DrawString(current.Taxon,
                                                leafLabelFont,
                                                DrawHelpers.CreatePdfColor(current.Style.LeafColor).ToBrush(),
                                                new XPoint(x, y));
                        }
                    }
                    else
                    {
                        // 結節点の値
                        if (tree.Style.ShowNodeValues && !current.GetIsExternal())
                        {
                            string nodeValue = DrawHelpers.SelectShowValue(current, tree.Style.NodeValueType);
                            if (nodeValue.Length > 0)
                            {
                                (double x, double y) = positionManager.CalcNodeValuePosition(current, nodeValue);

                                graphics.DrawString(nodeValue,
                                                    nodeValuesFont,
                                                    DrawHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                    new XPoint(x, y));
                            }
                        }
                    }

                    // クレード名
                    if (tree.Style.ShowCladeLabels && !string.IsNullOrEmpty(current.Style.CladeLabel))
                    {
                        var (line, text) = positionManager.CalcCladeLabelPosition(current);

                        if (tree.Style.CladeLabelsLineThickness > 0)
                        {
                            graphics.DrawLine(DrawHelpers.CreatePdfColor("black").ToPen(tree.Style.CladeLabelsLineThickness),
                                              new XPoint(line.x, line.yTop),
                                              new XPoint(line.x, line.yBottom));
                        }

                        graphics.DrawString(current.Style.CladeLabel,
                                            cladeLabelFont,
                                            XBrushes.Black,
                                            new XPoint(text.x, text.y));
                    }
                    if (current.GetDrawnBranchLength() > 0)
                    {
                        // 横棒
                        {
                            (double xParent, double xChild, double y) = positionManager.CalcHorizontalBranchPositions(current);

                            XColor branchColor = options.BranchColoring is BranchColoringType.Both or BranchColoringType.Horizontal ? DrawHelpers.CreatePdfColor(current.Style.BranchColor) : XColor.FromArgb(0, 0, 0);

                            graphics.DrawLine(branchColor.ToPen(tree.Style.BranchThickness),
                                              new XPoint(xParent, y),
                                              new XPoint(xChild, y));
                        }

                        // 枝の装飾
                        if (tree.Style.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                            foreach (BranchDecorationStyle currentDecoration in tree.Style.DecorationStyles.Where(x => x.Enabled && (x.Regex?.IsMatch(current.Supports) ?? false)))
                            {
                                (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(current, currentDecoration);
                                var shapeArea = new XRect(x, y, width, height);
                                string color = currentDecoration.ShapeColor;

                                switch (currentDecoration.DecorationType)
                                {
                                    case BranchDecorationType.ClosedCircle:
                                        graphics.DrawPie(DrawHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToBrush(),
                                                         shapeArea,
                                                         0,
                                                         360);
                                        break;

                                    case BranchDecorationType.OpenCircle:
                                        graphics.DrawPie(XBrushes.White,
                                                         shapeArea,
                                                         0,
                                                         360);
                                        graphics.DrawArc(DrawHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToPen(1),
                                                         shapeArea,
                                                         0,
                                                         360);
                                        break;

                                    case BranchDecorationType.ClosedRectangle:
                                        graphics.DrawRectangle(DrawHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToBrush(), shapeArea);
                                        break;

                                    case BranchDecorationType.OpenedRectangle:
                                        graphics.DrawRectangle(XBrushes.White, shapeArea);
                                        graphics.DrawRectangle(DrawHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToPen(currentDecoration.ShapeSize / 5 + 1), shapeArea);
                                        break;
                                }
                            }

                        // 二分岐の値
                        if (tree.Style.ShowBranchValues)
                        {
                            string branchValue = DrawHelpers.SelectShowValue(current, tree.Style.BranchValueType);
                            if (branchValue.Length > 0 && (!tree.Style.BranchValueHideRegex?.IsMatch(branchValue) ?? true))
                            {
                                (double x, double y) = positionManager.CalcBranchValuePosition(current, branchValue);

                                graphics.DrawString(branchValue,
                                                    branchValuesFont,
                                                    DrawHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                    new XPoint(x, y),
                                                    new XStringFormat()
                                                    {
                                                        Alignment = XStringAlignment.Center,
                                                        LineAlignment = XLineAlignment.BaseLine,
                                                    });
                            }
                        }
                    }

                    Clade? parent = current.Parent;
                    if (parent is not null)
                    {
                        if (parent.Children.Count > 1)
                        {
                            (double x, double yParent, double yChild) = positionManager.CalcVerticalBranchPositions(current);

                            // 縦棒
                            if (yParent != yChild)
                            {
                                XColor branchColor = options.BranchColoring is BranchColoringType.Both or BranchColoringType.Vertical ? DrawHelpers.CreatePdfColor(current.Style.BranchColor) : XColor.FromArgb(0, 0, 0);

                                graphics.DrawLine(branchColor.ToPen(tree.Style.BranchThickness),
                                                  new XPoint(x, yChild),
                                                  new XPoint(x, yParent));
                            }
                        }
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (tree.Style.ShowScaleBar && tree.Style.ScaleBarValue > 0)
            {
                using XGraphics graphics = XGraphics.FromPdfPage(mainPage);

                (double offsetX, double offsetY) = positionManager.CalcScaleBarOffset();
                graphics.TranslateTransform(offsetX, offsetY);

                ((double xLeft, double xRight, double y) line, (double x, double y) text) = positionManager.CalcScaleBarPositions();

                graphics.DrawString(tree.Style.ScaleBarValue.ToString(),
                                    new XFont(FontFamily, tree.Style.ScaleBarFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault),
                                    XBrushes.Black,
                                    new XPoint(text.x, text.y), new XStringFormat()
                                    {
                                        Alignment = XStringAlignment.Center,
                                        LineAlignment = XLineAlignment.BaseLine,
                                    });
                graphics.DrawLine(new XPen(XBrushes.Black, tree.Style.ScaleBarThickness),
                                  new XPoint(line.xLeft, line.y),
                                  new XPoint(line.xRight, line.y));
            }

            #endregion スケールバー

            return result;
        }

        /// <inheritdoc/>
        public void Export(Tree tree, Stream destination, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(options);

            using PdfDocument document = CreatePdf(tree, options);
            document.Save(destination);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            Export(tree, destination, options);

            await Task.CompletedTask;
        }
    }
}
