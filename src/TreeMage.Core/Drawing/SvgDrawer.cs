using Svg;
using Svg.Pathing;
using Svg.Transforms;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Internal;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// SVGにおける<see cref="ITreeDrawer"/>の実装です。
    /// </summary>
    public class SvgDrawer : ITreeDrawer
    {
        private readonly PositionManager positionManager = new PositionManager();
        private DrawingInfo? drawingInfo;

        /// <summary>
        /// SVGドキュメントを取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">ドキュメントが初期化されていない</exception>
        [MemberNotNull(nameof(drawingInfo))]
        public SvgDocument Document
        {
            get
            {
                if (drawingInfo is null) throw new InvalidOperationException("ドキュメントが初期化されていません");
                return drawingInfo.Document;
            }
        }

        PositionManager ITreeDrawer.PositionManager => positionManager;

        void ITreeDrawer.InitDocument()
        {
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(drawingInfo))]
        public void BeginTree(TMSize size, Tree tree)
        {
            drawingInfo = new DrawingInfo(size, tree);
        }

        /// <inheritdoc/>
        public void DrawCladeShade(TMRect area, TMColor fill)
        {
            Debug.Assert(drawingInfo is not null);

            var rectangle = new SvgRectangle()
            {
                X = (SvgUnit)area.X,
                Y = (SvgUnit)area.Y,
                Width = (SvgUnit)area.Width,
                Height = (SvgUnit)area.Height,
                Fill = fill.ToSvgColorServer(),
            };
            rectangle.AddTo(drawingInfo.ShadesGroup);
        }

        /// <inheritdoc/>
        public void DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);

            var triangle = new SvgPath()
            {
                Fill = SvgPaintServer.None,
                Stroke = stroke.ToSvgColorServer(),
                StrokeWidth = lineThickness,
                PathData = new SvgPathSegmentList().MoveToAbsolutely((float)left.X, (float)left.Y)
                                                   .DrawLineAbsolutely((float)rightTop.X, (float)rightTop.Y)
                                                   .DrawLineAbsolutely((float)rightBottom.X, (float)rightBottom.Y)
                                                   .DrawLineAbsolutely((float)left.X, (float)left.Y),
            };
            triangle.AddTo(drawingInfo.LeafLabelsGroup);
        }

        /// <inheritdoc/>
        public void DrawLeafLabel(string taxon, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            var leafText = new SvgText(taxon)
            {
                X = [(SvgUnit)point.X],
                Y = [(SvgUnit)point.Y],
                Fill = fill.ToSvgColorServer(),
                FontSize = fontSize,
                FontFamily = FontManager.DefaultFontFamily,
            };
            leafText.AddTo(drawingInfo.LeafLabelsGroup);
        }

        /// <inheritdoc/>
        public void DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            var nodeValueText = new SvgText(value)
            {
                X = [(SvgUnit)point.X],
                Y = [(SvgUnit)point.Y],
                Fill = fill.ToSvgColorServer(),
                FontSize = fontSize,
                FontFamily = FontManager.DefaultFontFamily,
            };
            nodeValueText.AddTo(drawingInfo.NodeValuesGroup);
        }

        /// <inheritdoc/>
        public void DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            var branchValueText = new SvgText(value)
            {
                X = [(SvgUnit)point.X],
                Y = [(SvgUnit)point.Y],
                Fill = fill.ToSvgColorServer(),
                FontSize = fontSize,
                FontFamily = FontManager.DefaultFontFamily,
                TextAnchor = SvgTextAnchor.Middle,
            };
            branchValueText.AddTo(drawingInfo.BranchValuesGroup);
        }

        /// <inheritdoc/>
        public void DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int lineThickness, int fontSize)
        {
            Debug.Assert(drawingInfo is not null);

            SvgGroup group = new SvgGroup()
            {
                ID = cladeName,
            }.AddTo(drawingInfo.CladeLabelsGroup);

            if (lineThickness > 0)
            {
                var svgLine = new SvgLine()
                {
                    StartX = (SvgUnit)lineBegin.X,
                    EndX = (SvgUnit)lineEnd.X,
                    StartY = (SvgUnit)lineBegin.Y,
                    EndY = (SvgUnit)lineEnd.Y,
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = lineThickness,
                };
                svgLine.AddTo(group);
            }
            var svgText = new SvgText(cladeName)
            {
                X = [(SvgUnit)textPoint.X],
                Y = [(SvgUnit)textPoint.Y],
                FontFamily = FontManager.DefaultFontFamily,
                FontSize = fontSize,
            };
            svgText.AddTo(group);
        }

        /// <inheritdoc/>
        public void DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            var horizontalLine = new SvgLine()
            {
                StartX = (SvgUnit)parentPoint.X,
                StartY = (SvgUnit)parentPoint.Y,
                EndX = (SvgUnit)childPoint.X,
                EndY = (SvgUnit)childPoint.Y,
                Stroke = stroke.ToSvgColorServer(),
                StrokeWidth = thickness,
            };
            horizontalLine.AddTo(drawingInfo.BranchesGroup);
        }

        /// <inheritdoc/>
        public void DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
            Debug.Assert(drawingInfo is not null);

            var verticalLine = new SvgLine()
            {
                StartX = (SvgUnit)childPoint.X,
                StartY = (SvgUnit)childPoint.Y,
                EndX = (SvgUnit)parentPoint.X,
                EndY = (SvgUnit)parentPoint.Y,
                Stroke = stroke.ToSvgColorServer(),
                StrokeWidth = thickness,
            };
            verticalLine.AddTo(drawingInfo.BranchesGroup);
        }

        /// <inheritdoc/>
        public void DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
            Debug.Assert(drawingInfo is not null);

            var color = new TMColor(style.ShapeColor);
            SvgElement decorationSvg;
            switch (style.DecorationType)
            {
                case BranchDecorationType.ClosedCircle:
                case BranchDecorationType.OpenCircle:
                    {
                        (TMPoint center, double radius) = positionManager.CalcBranchDecorationCircleArea(target, style);

                        decorationSvg = new SvgCircle()
                        {
                            CenterX = (SvgUnit)center.X,
                            CenterY = (SvgUnit)center.Y,
                            Radius = (SvgUnit)radius,
                        };

                        if (style.DecorationType == BranchDecorationType.ClosedCircle)
                        {
                            decorationSvg.Stroke = SvgPaintServer.None;
                            decorationSvg.Fill = color.ToSvgColorServer();
                        }
                        else
                        {
                            decorationSvg.Stroke = color.ToSvgColorServer();
                            decorationSvg.Fill = new SvgColourServer(Color.White);
                        }
                    }
                    break;

                case BranchDecorationType.ClosedRectangle:
                case BranchDecorationType.OpenedRectangle:
                    {
                        (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(target, style);

                        decorationSvg = new SvgRectangle()
                        {
                            X = (SvgUnit)x,
                            Y = (SvgUnit)y,
                            Width = (SvgUnit)width,
                            Height = (SvgUnit)height,
                        };

                        if (style.DecorationType == BranchDecorationType.ClosedRectangle)
                        {
                            decorationSvg.Stroke = SvgPaintServer.None;
                            decorationSvg.Fill = color.ToSvgColorServer();
                        }
                        else
                        {
                            decorationSvg.Stroke = color.ToSvgColorServer();
                            decorationSvg.StrokeWidth = style.ShapeSize / 5 + 1;
                            decorationSvg.Fill = new SvgColourServer(Color.White);
                        }
                    }
                    break;

                default: throw new InvalidOperationException($"装飾の種類'{style.DecorationType}'が無効です");
            };
            decorationSvg.AddTo(drawingInfo.BranchDecorationsGroup);
        }

        /// <inheritdoc/>
        public void DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
            Debug.Assert(drawingInfo is not null);

            SvgGroup scaleBarArea = new SvgGroup()
            {
                ID = "scale-bar",
                Transforms = new SvgTransformCollection().Translate((SvgUnit)offset.X, (SvgUnit)offset.Y),
            }.AddTo(drawingInfo.Document);

            var scaleBarText = new SvgText(value.ToString())
            {
                X = [(SvgUnit)textPoint.X],
                Y = [(SvgUnit)textPoint.Y],
                FontFamily = FontManager.DefaultFontFamily,
                FontSize = fontSize,
                TextAnchor = SvgTextAnchor.Middle,
            };
            scaleBarText.AddTo(scaleBarArea);

            var scaleBarLine = new SvgLine()
            {
                StartX = (SvgUnit)lineBegin.X,
                StartY = (SvgUnit)lineBegin.Y,
                EndX = (SvgUnit)lineEnd.X,
                EndY = (SvgUnit)lineEnd.Y,
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = lineThickness,
            };
            scaleBarLine.AddTo(scaleBarArea);
        }

        void ITreeDrawer.FinishTree()
        {
        }

        private sealed class DrawingInfo
        {
            public SvgDocument Document { get; }

            public SvgGroup TreeArea { get; }

            public SvgGroup ShadesGroup { get; }

            public SvgGroup LeafLabelsGroup { get; }

            public SvgGroup CladeLabelsGroup { get; }

            public SvgGroup BranchesGroup { get; }

            public SvgGroup NodeValuesGroup { get; }

            public SvgGroup BranchValuesGroup { get; }

            public SvgGroup BranchDecorationsGroup { get; }

            public DrawingInfo(TMSize size, Tree tree)
            {
                Document = new SvgDocument()
                {
                    Width = (SvgUnit)size.Width,
                    Height = (SvgUnit)size.Height,
                };
                TreeArea = new SvgGroup()
                {
                    ID = "tree-area",
                    Transforms = new SvgTransformCollection().Translate(50, 50),
                }.AddTo(Document);
                ShadesGroup = new SvgGroup()
                {
                    ID = "shades",
                }.AddTo(TreeArea);
                LeafLabelsGroup = new SvgGroup()
                {
                    ID = "leaf-labels",
                };
                if (tree.Style.ShowLeafLabels) LeafLabelsGroup.AddTo(TreeArea);
                CladeLabelsGroup = new SvgGroup()
                {
                    ID = "clade-labels"
                };
                if (tree.Style.ShowCladeLabels) CladeLabelsGroup.AddTo(TreeArea);
                BranchesGroup = new SvgGroup()
                {
                    ID = "branches",
                }.AddTo(TreeArea);
                NodeValuesGroup = new SvgGroup()
                {
                    ID = "node-values",
                };
                if (tree.Style.ShowNodeValues) NodeValuesGroup.AddTo(TreeArea);
                BranchValuesGroup = new SvgGroup()
                {
                    ID = "branch-values",
                };
                if (tree.Style.ShowBranchValues) BranchValuesGroup.AddTo(TreeArea);
                BranchDecorationsGroup = new SvgGroup()
                {
                    ID = "branch-decorations",
                };
                if (tree.Style.ShowBranchDecorations) BranchDecorationsGroup.AddTo(TreeArea);
            }
        }
    }
}
