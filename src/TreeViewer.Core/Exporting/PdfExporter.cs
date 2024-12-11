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

            using XGraphics graphics = XGraphics.FromPdfPage(mainPage);

            var leafFont = new XFont(FontFamily, tree.Style.LeafLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var nodeValuesFont = new XFont(FontFamily, tree.Style.NodeValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var branchValuesFont = new XFont(FontFamily, tree.Style.BranchValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);

            Clade[] allLeaves = tree.GetAllLeaves()
                                    .ToArray();

            double pageWidth = allLeaves.Select(x => x.GetTotalBranchLength()).Max() * tree.Style.XScale + 100;
            if (tree.Style.ShowLeafLabels) pageWidth += allLeaves.Select(x => (x.Taxon ?? string.Empty).Length).Max() * tree.Style.LeafLabelsFontSize / 1.25;
            double pageHeight = allLeaves.Length * tree.Style.YScale + 100;
            if (tree.Style.ShowScaleBar) pageHeight += tree.Style.ScaleBarFontSize;

            mainPage.Width = pageWidth;
            mainPage.Height = pageHeight;

            graphics.TranslateTransform(50, 50);

            #region 系統樹部分

            var positionManager = new PositionManager(tree);

            foreach (Clade current in tree.GetAllClades())
            {
                double totalLength = positionManager.CalcTotalBranchLength(current);
                XPen branchPen = ExportHelpers.CreatePdfColor(current.Style.BranchColor)
                                              .ToPen(tree.Style.BranchThickness);

                if (current.IsLeaf)
                {
                    double x = totalLength * tree.Style.XScale + 5;
                    double y = positionManager.CalcY1(current) + tree.Style.LeafLabelsFontSize / 2.5;

                    // 系統名
                    if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                    {
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
                            double y = positionManager.CalcY1(current) + tree.Style.NodeValueFontSize / 2.5;
                            if (current.Children.Count % 2 == 1) y += tree.Style.BranchThickness / 2 + 3 + tree.Style.NodeValueFontSize / 2.5;

                            graphics.DrawString(nodeValue,
                                                nodeValuesFont,
                                                ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                new XPoint(totalLength * tree.Style.XScale + 5, y));
                        }
                    }
                }

                double x1 = (totalLength - current.BranchLength) * tree.Style.XScale;
                double x2;
                if (current.BranchLength > 0)
                {
                    x2 = totalLength * tree.Style.XScale;

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
                            int size = currentDecoration.ShapeSize;
                            string color = currentDecoration.ShapeColor;
                            var shapeArea = new XRect((x1 + x2) / 2 - size, positionManager.CalcY1(current) - size, size * 2, size * 2);

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
                                    graphics.DrawRectangle(ExportHelpers.CreatePdfColor(currentDecoration.ShapeColor).ToPen(size / 5 + 1), shapeArea);
                                    break;
                            }
                        }

                    // 二分岐の値
                    if (tree.Style.ShowBranchValues)
                    {
                        string branchValue = ExportHelpers.SelectShowValue(current, tree.Style.BranchValueType);
                        if (branchValue.Length > 0)
                        {
                            graphics.DrawString(branchValue,
                                                branchValuesFont,
                                                ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                new XPoint((x1 + x2) / 2, positionManager.CalcY1(current) - tree.Style.BranchValueFontSize / 2.5 - tree.Style.BranchThickness / 2),
                                                new XStringFormat()
                                                {
                                                    Alignment = XStringAlignment.Center,
                                                    LineAlignment = XLineAlignment.BaseLine,
                                                });
                        }
                    }
                }
                else x2 = x1;

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

            #endregion 系統樹部分

            #region スケールバー

            if (tree.Style.ShowScaleBar && tree.Style.ScaleBarValue > 0)
            {
                double yOffset = allLeaves.Length * tree.Style.YScale + 30;

                double scaleBarWidth = tree.Style.ScaleBarValue * tree.Style.XScale;
                graphics.DrawString(tree.Style.ScaleBarValue.ToString(),
                                    new XFont(FontFamily, tree.Style.ScaleBarFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault),
                                    XBrushes.Black,
                                    new XPoint(scaleBarWidth / 2, yOffset), new XStringFormat()
                                    {
                                        Alignment = XStringAlignment.Center,
                                        LineAlignment = XLineAlignment.BaseLine,
                                    });
                graphics.DrawLine(new XPen(XBrushes.Black, tree.Style.ScaleBarThickness),
                                  new XPoint(0, 10 + yOffset),
                                  new XPoint(scaleBarWidth, 10 + yOffset));
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
