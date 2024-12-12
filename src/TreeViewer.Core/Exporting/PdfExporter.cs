using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using TreeViewer.Core.Styles;
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
        /// <returns><paramref name="tree"/>の図を表すPDFオブジェクト</returns>
        internal static PdfDocument CreatePdf(Tree tree)
        {
            var result = new PdfDocument();
            result.Info.Creator = "TreeViewer";
            result.Info.Keywords = "TreeViewer, Phylogeny";
            result.PageLayout = PdfPageLayout.SinglePage;
            PdfPage mainPage = result.AddPage();

            var leafFont = new XFont(FontFamily, tree.Style.LeafLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var nodeValuesFont = new XFont(FontFamily, tree.Style.NodeValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var branchValuesFont = new XFont(FontFamily, tree.Style.BranchValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);

            var positionManager = new PositionManager(tree);

            (double pageWidth, double pageHeight) = positionManager.ClacDocumentSize();

            mainPage.Width = pageWidth;
            mainPage.Height = pageHeight;

            #region 系統樹部分

            using (XGraphics graphics = XGraphics.FromPdfPage(mainPage))
            {
                graphics.TranslateTransform(50, 50);

                foreach (Clade current in tree.GetAllClades())
                {
                    XPen branchPen = ExportHelpers.CreatePdfColor(current.Style.BranchColor)
                                                  .ToPen(tree.Style.BranchThickness);

                    if (current.IsLeaf)
                    {
                        // 系統名
                        if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                        {
                            (double x, double y) = positionManager.CalcLeafPosition(current);

                            graphics.DrawString(current.Taxon,
                                                leafFont,
                                                ExportHelpers.CreatePdfColor(current.Style.LeafColor).ToBrush(),
                                                new XPoint(x, y));
                        }
                    }
                    else
                    {
                        // 結節点の値
                        if (tree.Style.ShowNodeValues)
                        {
                            string nodeValue = ExportHelpers.SelectShowValue(current, tree.Style.NodeValueType);
                            if (nodeValue.Length > 0)
                            {
                                (double x, double y) = positionManager.CalcNodeValuePosition(current);

                                graphics.DrawString(nodeValue,
                                                    nodeValuesFont,
                                                    ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                    new XPoint(x, y));
                            }
                        }
                    }

                    if (current.BranchLength > 0)
                    {
                        // 横棒
                        {
                            (double xParent, double xChild, double y) = positionManager.CalcHorizontalBranchPositions(current);

                            graphics.DrawLine(branchPen,
                                              new XPoint(xParent, y),
                                              new XPoint(xChild, y));
                        }

                        // 枝の装飾
                        if (tree.Style.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                            foreach (BranchDecorationStyle currentDecoration in tree.Style.DecorationStyles.Where(x => x.Regex.IsMatch(current.Supports)))
                            {
                                (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(current, currentDecoration);
                                var shapeArea = new XRect(x, y, width, height);
                                string color = currentDecoration.ShapeColor;

                                switch (currentDecoration.DecorationType)
                                {
                                    case BranchDecorationType.ClosedCircle:
                                        graphics.DrawPie(ExportHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToBrush(),
                                                         shapeArea,
                                                         0,
                                                         360);
                                        break;

                                    case BranchDecorationType.OpenCircle:
                                        graphics.DrawPie(XBrushes.White,
                                                         shapeArea,
                                                         0,
                                                         360);
                                        graphics.DrawArc(ExportHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToPen(1),
                                                         shapeArea,
                                                         0,
                                                         360);
                                        break;

                                    case BranchDecorationType.ClosedRectangle:
                                        graphics.DrawRectangle(ExportHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToBrush(), shapeArea);
                                        break;

                                    case BranchDecorationType.OpenedRectangle:
                                        graphics.DrawRectangle(XBrushes.White, shapeArea);
                                        graphics.DrawRectangle(ExportHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToPen(currentDecoration.ShapeSize / 5 + 1), shapeArea);
                                        break;
                                }
                            }

                        // 二分岐の値
                        if (tree.Style.ShowBranchValues)
                        {
                            string branchValue = ExportHelpers.SelectShowValue(current, tree.Style.BranchValueType);
                            if (branchValue.Length > 0)
                            {
                                (double x, double y) = positionManager.CalcBranchValuePosition(current);

                                graphics.DrawString(branchValue,
                                                    branchValuesFont,
                                                    ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
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
                                graphics.DrawLine(branchPen,
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

            using PdfDocument document = CreatePdf(tree);
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
