using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// PDFにおける<see cref="ITreeDrawer"/>の実装です。
    /// </summary>
    public class PdfDrawer : ITreeDrawer
    {
        internal const string FontFamily = "Arial";

        private readonly PositionManager positionManager = new PositionManager();
        private PdfDocument? document;
        private DrawingInfo? drawingInfo;

        /// <summary>
        /// PDFドキュメントを取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">ドキュメントが初期化されていない</exception>
        [MemberNotNull(nameof(document))]
        public PdfDocument Document
        {
            get
            {
                if (document is null) throw new InvalidOperationException("ドキュメントが初期化されていません");
                return document;
            }
        }

        PositionManager ITreeDrawer.PositionManager => positionManager;

        static PdfDrawer()
        {
            GlobalFontSettings.FontResolver = new FontResolver();
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(document))]
        public void InitDocument()
        {
            document = new PdfDocument();
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(document))]
        [MemberNotNull(nameof(drawingInfo))]
        public void BeginTree(double width, double height, Tree tree)
        {
            Debug.Assert(document is not null);

            drawingInfo = new DrawingInfo(Document, width, height, tree);
            drawingInfo.Graphics.TranslateTransform(50, 50);
        }

        /// <inheritdoc/>
        public void DrawCladeShade(double x, double y, double width, double height, string fill)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawRectangle(DrawHelpers.CreatePdfColor(fill).ToBrush(), x, y, width, height);
        }

        /// <inheritdoc/>
        public void DrawCollapsedTriangle((double x, double y) left, (double x, double y) rightTop, (double x, double y) rightBottom, string stroke, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);

            var path = new XGraphicsPath();
            path.AddLine(left.x, left.y, rightTop.x, rightTop.y);
            path.AddLine(rightTop.x, rightTop.y, rightBottom.x, rightBottom.y);
            path.AddLine(left.x, left.y, rightBottom.x, rightBottom.y);
            drawingInfo.Graphics.DrawPath(DrawHelpers.CreatePdfColor(stroke).ToPen(lineThickness),
                                          XBrushes.Transparent,
                                          path);
        }

        /// <inheritdoc/>
        public void DrawLeafLabel(string taxon, double x, double y, string fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(taxon,
                                            drawingInfo.LeafLabelFont,
                                            DrawHelpers.CreatePdfColor(fill).ToBrush(),
                                            new XPoint(x, y));
        }

        /// <inheritdoc/>
        public void DrawNodeValue(string value, double x, double y, string fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(value,
                                            drawingInfo.NodeValuesFont,
                                            DrawHelpers.CreatePdfColor(fill).ToBrush(),
                                            new XPoint(x, y));
        }

        /// <inheritdoc/>
        public void DrawBranchValue(string value, double x, double y, string fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(value,
                                            drawingInfo.BranchValuesFont,
                                            DrawHelpers.CreatePdfColor(fill).ToBrush(),
                                            new XPoint(x, y),
                                            new XStringFormat()
                                            {
                                                Alignment = XStringAlignment.Center,
                                                LineAlignment = XLineAlignment.BaseLine,
                                            });
        }

        /// <inheritdoc/>
        public void DrawCladeLabel(string cladeName, (double x, double yTop, double yBottom) linePosition, (double x, double y) textPosition, int lineThickness, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            if (lineThickness > 0)
            {
                drawingInfo.Graphics.DrawLine(DrawHelpers.CreatePdfColor("black").ToPen(lineThickness),
                                              new XPoint(linePosition.x, linePosition.yTop),
                                              new XPoint(linePosition.x, linePosition.yBottom));
            }

            drawingInfo.Graphics.DrawString(cladeName,
                                            drawingInfo.CladeLabelFont,
                                            XBrushes.Black,
                                            new XPoint(textPosition.x, textPosition.y));
        }

        /// <inheritdoc/>
        public void DrawHorizontalBranch(double x1, double x2, double y, string stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawLine(DrawHelpers.CreatePdfColor(stroke).ToPen(thickness),
                                          new XPoint(x1, y),
                                          new XPoint(x2, y));
        }

        /// <inheritdoc/>
        public void DrawVerticalBranch(double x, double y1, double y2, string stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawLine(DrawHelpers.CreatePdfColor(stroke).ToPen(thickness),
                              new XPoint(x, y2),
                              new XPoint(x, y1));
        }

        /// <inheritdoc/>
        public void DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
            Debug.Assert(drawingInfo is not null);

            (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(target, style);
            var shapeArea = new XRect(x, y, width, height);

            switch (style.DecorationType)
            {
                case BranchDecorationType.ClosedCircle:
                    drawingInfo.Graphics.DrawPie(DrawHelpers.CreatePdfColor(style.ShapeColor).ToBrush(),
                                                 shapeArea,
                                                 0,
                                                 360);
                    break;

                case BranchDecorationType.OpenCircle:
                    drawingInfo.Graphics.DrawPie(XBrushes.White,
                                                 shapeArea,
                                                 0,
                                                 360);
                    drawingInfo.Graphics.DrawArc(DrawHelpers.CreatePdfColor(style.ShapeColor).ToPen(1),
                                                 shapeArea,
                                                 0,
                                                 360);
                    break;

                case BranchDecorationType.ClosedRectangle:
                    drawingInfo.Graphics.DrawRectangle(DrawHelpers.CreatePdfColor(style.ShapeColor).ToBrush(), shapeArea);
                    break;

                case BranchDecorationType.OpenedRectangle:
                    drawingInfo.Graphics.DrawRectangle(XBrushes.White, shapeArea);
                    drawingInfo.Graphics.DrawRectangle(DrawHelpers.CreatePdfColor(style.ShapeColor).ToPen(style.ShapeSize / 5 + 1), shapeArea);
                    break;
            }
        }

        /// <inheritdoc/>
        public void DrawScalebar(double value, double offsetX, double offsetY, (double, double, double) linePosition, (double, double) textPosition, int fontSize, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);
            drawingInfo.Graphics.Dispose();

            using XGraphics graphics = XGraphics.FromPdfPage(drawingInfo.Page);
            graphics.TranslateTransform(offsetX, offsetY);

            ((double xLeft, double xRight, double y) line, (double x, double y) text) = positionManager.CalcScaleBarPositions();

            graphics.DrawString(value.ToString(),
                                new XFont(FontFamily, fontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault),
                                XBrushes.Black,
                                new XPoint(text.x, text.y), new XStringFormat()
                                {
                                    Alignment = XStringAlignment.Center,
                                    LineAlignment = XLineAlignment.BaseLine,
                                });
            graphics.DrawLine(new XPen(XBrushes.Black, lineThickness),
                              new XPoint(line.xLeft, line.y),
                              new XPoint(line.xRight, line.y));
        }

        /// <inheritdoc/>
        public void FinishTree()
        {
            drawingInfo?.Graphics?.Dispose();
        }

        public sealed class DrawingInfo
        {
            public PdfPage Page { get; }

            public XGraphics Graphics { get; }

            public XFont LeafLabelFont { get; }

            public XFont CladeLabelFont { get; }

            public XFont NodeValuesFont { get; }

            public XFont BranchValuesFont { get; }

            public DrawingInfo(PdfDocument document, double width, double height, Tree tree)
            {
                document.Info.Creator = "TreeMage";
                document.Info.Keywords = "TreeMage, Phylogeny";
                document.PageLayout = PdfPageLayout.SinglePage;

                Page = document.AddPage();
                Page.Width = width;
                Page.Height = height;
                Graphics = XGraphics.FromPdfPage(Page);

                LeafLabelFont = new XFont(FontFamily, tree.Style.LeafLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
                CladeLabelFont = new XFont(FontFamily, tree.Style.CladeLabelsFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
                NodeValuesFont = new XFont(FontFamily, tree.Style.NodeValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
                BranchValuesFont = new XFont(FontFamily, tree.Style.BranchValueFontSize, XFontStyle.Regular, XPdfFontOptions.UnicodeDefault);
            }
        }
    }
}
