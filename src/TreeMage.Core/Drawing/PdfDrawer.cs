using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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

        /// <inheritdoc/>
        [MemberNotNull(nameof(document))]
        public void InitDocument()
        {
            document = new PdfDocument();
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(document))]
        [MemberNotNull(nameof(drawingInfo))]
        public void BeginTree(TMSize size, Tree tree)
        {
            Debug.Assert(document is not null);

            drawingInfo = new DrawingInfo(Document, size.Width, size.Height, tree);
            drawingInfo.Graphics.TranslateTransform(50, 50);
        }

        /// <inheritdoc/>
        public void DrawCladeShade(TMRect area, TMColor fill, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawRectangle(fill.ToPdfBrush(), area);
        }

        /// <inheritdoc/>
        public void DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            var path = new XGraphicsPath();
            path.AddLine(left, rightTop);
            path.AddLine(rightTop, rightBottom);
            path.AddLine(left, rightBottom);
            drawingInfo.Graphics.DrawPath(stroke.ToPdfPen(lineThickness),
                                          XBrushes.Transparent,
                                          path);
        }

        /// <inheritdoc/>
        public void DrawLeafLabel(string taxon, TMRect area, TMColor fill, int fontSize, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(taxon,
                                            drawingInfo.LeafLabelFont,
                                            fill.ToPdfBrush(),
                                            area.Point);
        }

        /// <inheritdoc/>
        public void DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(value,
                                            drawingInfo.NodeValuesFont,
                                            fill.ToPdfBrush(),
                                            point);
        }

        /// <inheritdoc/>
        public void DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawString(value,
                                            drawingInfo.BranchValuesFont,
                                            fill.ToPdfBrush(),
                                            point,
                                            new XStringFormat()
                                            {
                                                Alignment = XStringAlignment.Center,
                                                LineAlignment = XLineAlignment.BaseLine,
                                            });
        }

        /// <inheritdoc/>
        public void DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int lineThickness, int fontSize, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            if (lineThickness > 0)
            {
                drawingInfo.Graphics.DrawLine(new XPen(XBrushes.Black, lineThickness),
                                              lineBegin,
                                              lineEnd);
            }

            drawingInfo.Graphics.DrawString(cladeName,
                                            drawingInfo.CladeLabelFont,
                                            XBrushes.Black,
                                            textPoint);
        }

        /// <inheritdoc/>
        public void DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawLine(stroke.ToPdfPen(thickness),
                                          parentPoint,
                                          childPoint);
        }

        /// <inheritdoc/>
        public void DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Graphics.DrawLine(stroke.ToPdfPen(thickness),
                                          parentPoint,
                                          childPoint);
        }

        /// <inheritdoc/>
        public void DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
            Debug.Assert(drawingInfo is not null);

            TMRect shapeArea = positionManager.CalcBranchDecorationRectangleArea(target, style);
            var color = new TMColor(style.ShapeColor);

            switch (style.DecorationType)
            {
                case BranchDecorationType.ClosedCircle:
                    drawingInfo.Graphics.DrawPie(color.ToPdfBrush(),
                                                 shapeArea,
                                                 0,
                                                 360);
                    break;

                case BranchDecorationType.OpenCircle:
                    drawingInfo.Graphics.DrawPie(XBrushes.White,
                                                 shapeArea,
                                                 0,
                                                 360);
                    drawingInfo.Graphics.DrawArc(color.ToPdfPen(1),
                                                 shapeArea,
                                                 0,
                                                 360);
                    break;

                case BranchDecorationType.ClosedRectangle:
                    drawingInfo.Graphics.DrawRectangle(color.ToPdfBrush(), shapeArea);
                    break;

                case BranchDecorationType.OpenedRectangle:
                    drawingInfo.Graphics.DrawRectangle(XBrushes.White, shapeArea);
                    drawingInfo.Graphics.DrawRectangle(color.ToPdfPen(style.ShapeSize / 5 + 1), shapeArea);
                    break;
            }
        }

        /// <inheritdoc/>
        public void DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);
            drawingInfo.Graphics.Dispose();

            using XGraphics graphics = XGraphics.FromPdfPage(drawingInfo.Page);
            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.DrawString(value.ToString(),
                                FontManager.GetPdfFont(fontSize),
                                XBrushes.Black,
                                textPoint,
                                new XStringFormat()
                                {
                                    Alignment = XStringAlignment.Center,
                                    LineAlignment = XLineAlignment.BaseLine,
                                });
            graphics.DrawLine(new XPen(XBrushes.Black, lineThickness),
                              lineBegin,
                              lineEnd);
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

                LeafLabelFont = FontManager.GetPdfFont(tree.Style.LeafLabelsFontSize);
                CladeLabelFont = FontManager.GetPdfFont(tree.Style.CladeLabelsFontSize);
                NodeValuesFont = FontManager.GetPdfFont(tree.Style.NodeValueFontSize);
                BranchValuesFont = FontManager.GetPdfFont(tree.Style.BranchValueFontSize);
            }
        }
    }
}
