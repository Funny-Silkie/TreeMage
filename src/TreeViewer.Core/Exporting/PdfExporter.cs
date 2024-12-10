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
        /// <param name="options">オプション</param>
        /// <returns><paramref name="tree"/>の図を表すPDFオブジェクト</returns>
        internal static PdfDocument CreatePdf(Tree tree, ExportOptions options)
        {
            var result = new PdfDocument();
            result.Info.Creator = "TreeViewer";
            result.Info.Keywords = "TreeViewer, Phylogeny";
#if DEBUG
            result.Info.CreationDate = new DateTime(2000, 1, 1);
#endif
            result.PageLayout = PdfPageLayout.SinglePage;
            PdfPage mainPage = result.AddPage();

            using XGraphics graphics = XGraphics.FromPdfPage(mainPage);

            var leafFont = new XFont(FontFamily, options.LeafLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var nodeValuesFont = new XFont(FontFamily, options.NodeValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            var branchValuesFont = new XFont(FontFamily, options.BranchValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);

            Clade[] allLeaves = tree.GetAllLeaves()
                                    .ToArray();

            double pageWidth = allLeaves.Select(x => x.GetTotalBranchLength()).Max() * options.XScale + 100;
            if (options.ShowLeafLabels) pageWidth += allLeaves.Select(x => (x.Taxon ?? string.Empty).Length).Max() * options.LeafLabelsFontSize / 1.25;
            mainPage.Width = pageWidth;
            mainPage.Height = allLeaves.Length * options.YScale + 100;

            graphics.TranslateTransform(50, 50);

            #region 系統樹部分

            Dictionary<Clade, int> indexTable = tree.GetAllLeaves()
                                                    .Select((x, i) => (x, i))
                                                    .ToDictionary();
            var positionManager = new PositionManager(options, indexTable);

            foreach (Clade current in tree.GetAllClades())
            {
                double totalLength = current.GetTotalBranchLength();
                XPen branchPen = ExportHelpers.CreatePdfColor(current.Style.BranchColor)
                                              .ToPen(options.BranchThickness);

                if (current.IsLeaf)
                {
                    double x = totalLength * options.XScale + 5;
                    double y = positionManager.CalcY1(current) + options.LeafLabelsFontSize / 2.5;

                    // 系統名
                    if (options.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
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
                    if (options.ShowNodeValues)
                    {
                        string nodeValue = ExportHelpers.SelectShowValue(current, options.NodeValueType);
                        if (nodeValue.Length > 0)
                        {
                            double y = positionManager.CalcY1(current) + options.NodeValueFontSize / 2.5;
                            if (current.Children.Count % 2 == 1) y += options.BranchThickness / 2 + 3 + options.NodeValueFontSize / 2.5;

                            graphics.DrawString(nodeValue,
                                                nodeValuesFont,
                                                ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                new XPoint(totalLength * options.XScale + 5, y));
                        }
                    }
                }

                double x1 = (totalLength - current.BranchLength) * options.XScale;
                double x2;
                if (current.BranchLength > 0)
                {
                    x2 = totalLength * options.XScale;

                    // 横棒
                    {
                        double x2Offset = current.IsLeaf ? 0 : options.BranchThickness / 2;
                        double y = positionManager.CalcY1(current);

                        graphics.DrawLine(branchPen,
                                          new XPoint(x1 - options.BranchThickness / 2, y),
                                          new XPoint(x2 + x2Offset, y));
                    }

                    // 枝の装飾
                    if (options.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                        foreach (BranchDecorationStyle currentDecoration in options.DecorationStyles.Where(x => x.Regex.IsMatch(current.Supports)))
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
                    if (options.ShowBranchValues)
                    {
                        string branchValue = ExportHelpers.SelectShowValue(current, options.BranchValueType);
                        if (branchValue.Length > 0)
                        {
                            graphics.DrawString(branchValue,
                                                branchValuesFont,
                                                ExportHelpers.CreatePdfColor(current.Style.BranchColor).ToBrush(),
                                                new XPoint((x1 + x2) / 2, positionManager.CalcY1(current) - options.BranchValueFontSize / 2.5 - options.BranchThickness / 2),
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
                        double y1 = positionManager.CalcY1(current);
                        double y2 = positionManager.CalcY2(current);
                        // 縦棒
                        if (y1 != y2)
                        {
                            graphics.DrawLine(branchPen,
                                              new XPoint(x1, y1),
                                              new XPoint(x1, y2));
                        }
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (options.ShowScaleBar && options.ScaleBarValue > 0)
            {
                double yOffset = allLeaves.Length * options.YScale + 30;

                double scaleBarWidth = options.ScaleBarValue * options.XScale;
                graphics.DrawString(options.ScaleBarValue.ToString(),
                                    new XFont(FontFamily, options.ScaleBarFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault),
                                    XBrushes.Black,
                                    new XPoint(scaleBarWidth / 2, yOffset), new XStringFormat()
                                    {
                                        Alignment = XStringAlignment.Center,
                                        LineAlignment = XLineAlignment.BaseLine,
                                    });
                graphics.DrawLine(new XPen(XBrushes.Black, options.ScaleBarThickness),
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
