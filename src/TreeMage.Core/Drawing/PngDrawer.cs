using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Internal;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// PNGにおける<see cref="ITreeDrawer"/>の実装です。
    /// </summary>
    public class PngDrawer : ITreeDrawer, IDisposable
    {
        internal static readonly FontFamily DefaultFontFamily;

        private readonly PositionManager positionManager = new PositionManager();
        private DrawingInfo? drawingInfo;

        /// <summary>
        /// PNGイメージを取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">ドキュメントが初期化されていない</exception>
        [MemberNotNull(nameof(drawingInfo))]
        public Image<Rgba32> Image
        {
            get
            {
                if (drawingInfo is null) throw new InvalidOperationException("ドキュメントが初期化されていません");
                return drawingInfo.Image;
            }
        }

        PositionManager ITreeDrawer.PositionManager => positionManager;

        static PngDrawer()
        {
#warning to constant
            DefaultFontFamily = SystemFonts.Get("Arial");
        }

        #region IDisposable

        private bool disposedValue;

        /// <summary>
        /// オブジェクトの破棄を行います。
        /// </summary>
        /// <param name="disposing">managed resourceも破棄するかどうかを表す値</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                drawingInfo?.Image.Dispose();
            }

            disposedValue = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        void ITreeDrawer.InitDocument()
        {
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(drawingInfo))]
        public void BeginTree(TMSize size, Tree tree)
        {
            drawingInfo = new DrawingInfo(size.Width, size.Height, tree);
        }

        /// <inheritdoc/>
        public void DrawCladeShade(TMRect area, TMColor fill)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawClosedRectangle(fill.ToPngBrush(), area);
        }

        /// <inheritdoc/>
        public void DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawPath(stroke.ToPngPen(lineThickness),
                                       new SolidBrush(Color.Transparent),
                                       left, rightTop, rightBottom, left);
        }

        /// <inheritdoc/>
        public void DrawLeafLabel(string taxon, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawString(taxon,
                                         drawingInfo.LeafLabelFont,
                                         fill.ToPngBrush(),
                                         point);
        }

        /// <inheritdoc/>
        public void DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawString(value,
                                         drawingInfo.NodeValuesFont,
                                         fill.ToPngBrush(),
                                         point);
        }

        /// <inheritdoc/>
        public void DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawString(value,
                                         fill.ToPngBrush(),
                                         point,
                                         new RichTextOptions(drawingInfo.BranchValuesFont)
                                         {
                                             TextAlignment = TextAlignment.Center,
                                         });
        }

        /// <inheritdoc/>
        public void DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int lineThickness, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            if (lineThickness > 0)
            {
                drawingInfo.Image.DrawLine(new SolidPen(Color.Black, lineThickness),
                                           lineBegin,
                                           lineEnd);
            }

            drawingInfo.Image.DrawString(cladeName,
                                         drawingInfo.CladeLabelFont,
                                         new SolidBrush(Color.Black),
                                         textPoint);
        }

        /// <inheritdoc/>
        public void DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawLine(stroke.ToPngPen(thickness),
                                       parentPoint,
                                       childPoint);
        }

        /// <inheritdoc/>
        public void DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawLine(stroke.ToPngPen(thickness),
                                       parentPoint,
                                       childPoint);
        }

        /// <inheritdoc/>
        public void DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
            Debug.Assert(drawingInfo is not null);

            var color = new TMColor(style.ShapeColor);

            switch (style.DecorationType)
            {
                case BranchDecorationType.ClosedCircle:
                    {
                        (TMPoint center, double radius) = positionManager.CalcBranchDecorationCircleArea(target, style);
                        drawingInfo.Image.DrawClosedCircle(color.ToPngBrush(),
                                                           center,
                                                           radius);
                    }
                    break;

                case BranchDecorationType.OpenCircle:
                    {
                        (TMPoint center, double radius) = positionManager.CalcBranchDecorationCircleArea(target, style);
                        drawingInfo.Image.DrawClosedCircle(new SolidBrush(Color.White),
                                                           center,
                                                           radius);
                        drawingInfo.Image.DrawOpenCircle(color.ToPngPen(1),
                                                         center,
                                                         radius);
                    }
                    break;

                case BranchDecorationType.ClosedRectangle:
                    {
                        TMRect area = positionManager.CalcBranchDecorationRectangleArea(target, style);
                        drawingInfo.Image.DrawClosedRectangle(color.ToPngBrush(), area);
                    }
                    break;

                case BranchDecorationType.OpenedRectangle:
                    {
                        TMRect area = positionManager.CalcBranchDecorationRectangleArea(target, style);
                        drawingInfo.Image.DrawClosedRectangle(color.ToPngBrush(), area);
                        drawingInfo.Image.DrawOpenRectangle(color.ToPngPen(style.ShapeSize / 5 + 1), area);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);

            drawingInfo.Image.DrawString(value.ToString(),
                                         new SolidBrush(Color.Black),
                                         textPoint + offset,
                                         new RichTextOptions(new Font(DefaultFontFamily, fontSize))
                                         {
                                             TextAlignment = TextAlignment.Center,
                                         });
            drawingInfo.Image.DrawLine(new SolidPen(Color.Black, lineThickness),
                                       lineBegin + offset,
                                       lineEnd + offset);
        }

        void ITreeDrawer.FinishTree()
        {
        }

        public sealed class DrawingInfo
        {
            public Image<Rgba32> Image { get; }

            public Font LeafLabelFont { get; }

            public Font CladeLabelFont { get; }

            public Font NodeValuesFont { get; }

            public Font BranchValuesFont { get; }

            public DrawingInfo(double width, double height, Tree tree)
            {
                Image = new Image<Rgba32>((int)width, (int)height);

                LeafLabelFont = new Font(DefaultFontFamily, tree.Style.LeafLabelsFontSize);
                CladeLabelFont = new Font(DefaultFontFamily, tree.Style.CladeLabelsFontSize);
                NodeValuesFont = new Font(DefaultFontFamily, tree.Style.NodeValueFontSize);
                BranchValuesFont = new Font(DefaultFontFamily, tree.Style.BranchValueFontSize);
            }
        }
    }
}
