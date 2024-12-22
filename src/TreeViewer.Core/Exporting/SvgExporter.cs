using Svg;
using Svg.Transforms;
using System.Drawing;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Internal;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// SVGへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class SvgExporter : IExporter
    {
        internal const string FontFamily = "Arial, Helvetica, sans-serif";

        /// <inheritdoc/>
        public ExportType Type => ExportType.Svg;

        /// <summary>
        /// <see cref="SvgExporter"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SvgExporter()
        {
        }

        /// <summary>
        /// SVGオブジェクトを生成します。
        /// </summary>
        /// <param name="tree">描画するツリー</param>
        /// <param name="options">エクスポート時のオプション</param>
        /// <returns><paramref name="tree"/>の図を表すSVGオブジェクト</returns>
        internal static SvgDocument CreateSvg(Tree tree, ExportOptions options)
        {
            var positionManager = new PositionManager(tree);

            (double svgWidth, double svgHeight) = positionManager.CalcDocumentSize();

            var result = new SvgDocument()
            {
                Width = (SvgUnit)svgWidth,
                Height = (SvgUnit)svgHeight,
            };

            #region 系統樹部分

            SvgGroup treeArea = new SvgGroup()
            {
                ID = "tree-area",
                Transforms = new SvgTransformCollection().Translate(50, 50),
            }.AddTo(result);
            var leavesGroup = new SvgGroup()
            {
                ID = "leaves",
            };
            if (tree.Style.ShowLeafLabels) leavesGroup.AddTo(treeArea);
            SvgGroup branchesGroup = new SvgGroup()
            {
                ID = "branches",
            }.AddTo(treeArea);
            var nodeValuesGroup = new SvgGroup()
            {
                ID = "node-values",
            };
            if (tree.Style.ShowNodeValues) nodeValuesGroup.AddTo(treeArea);
            var branchValuesGroup = new SvgGroup()
            {
                ID = "branch-values",
            };
            if (tree.Style.ShowBranchValues) branchValuesGroup.AddTo(treeArea);
            var branchDecorationsGroup = new SvgGroup()
            {
                ID = "branch-decorations",
            };
            if (tree.Style.ShowBranchDecorations) branchDecorationsGroup.AddTo(treeArea);

            foreach (Clade current in tree.GetAllClades())
            {
                if (current.IsLeaf)
                {
                    // 系統名
                    if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                    {
                        (double x, double y, _, _) = positionManager.CalcLeafPosition(current);

                        var leafText = new SvgText(current.Taxon)
                        {
                            X = [(SvgUnit)x],
                            Y = [(SvgUnit)y],
                            Fill = DrawHelpers.CreateSvgColor(current.Style.LeafColor),
                            FontSize = tree.Style.LeafLabelsFontSize,
                            FontFamily = FontFamily,
                        };
                        leafText.AddTo(leavesGroup);
                    }
                }
                else
                {
                    // 結節点の値
                    if (tree.Style.ShowNodeValues)
                    {
                        string nodeValue = DrawHelpers.SelectShowValue(current, tree.Style.NodeValueType);
                        if (nodeValue.Length > 0)
                        {
                            (double x, double y) = positionManager.CalcNodeValuePosition(current, nodeValue);

                            var nodeValueText = new SvgText(nodeValue)
                            {
                                X = [(SvgUnit)x],
                                Y = [(SvgUnit)y],
                                Fill = DrawHelpers.CreateSvgColor(current.Style.BranchColor),
                                FontSize = tree.Style.NodeValueFontSize,
                                FontFamily = FontFamily,
                            };
                            nodeValueText.AddTo(nodeValuesGroup);
                        }
                    }
                }

                if (current.BranchLength > 0)
                {
                    // 横棒
                    {
                        (double xParent, double xChild, double y) = positionManager.CalcHorizontalBranchPositions(current);

                        var horizontalLine = new SvgLine()
                        {
                            StartX = (SvgUnit)xParent,
                            StartY = (SvgUnit)y,
                            EndX = (SvgUnit)xChild,
                            EndY = (SvgUnit)y,
                            Stroke = DrawHelpers.CreateSvgColor(options.BranchColoring is BranchColoringType.Both or BranchColoringType.Horizontal ? current.Style.BranchColor : "black"),
                            StrokeWidth = tree.Style.BranchThickness,
                        };
                        horizontalLine.AddTo(branchesGroup);
                    }

                    // 枝の装飾
                    if (tree.Style.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                    {
                        foreach (BranchDecorationStyle currentDecoration in tree.Style.DecorationStyles.Where(x => x.Enabled && (x.Regex?.IsMatch(current.Supports) ?? false)))
                        {
                            string color = currentDecoration.ShapeColor;

                            SvgElement decorationSvg;
                            switch (currentDecoration.DecorationType)
                            {
                                case BranchDecorationType.ClosedCircle:
                                case BranchDecorationType.OpenCircle:
                                    {
                                        (double centerX, double centerY, double radius) = positionManager.CalcBranchDecorationCircleArea(current, currentDecoration);

                                        decorationSvg = new SvgCircle()
                                        {
                                            CenterX = (SvgUnit)centerX,
                                            CenterY = (SvgUnit)centerY,
                                            Radius = (SvgUnit)radius,
                                        };

                                        if (currentDecoration.DecorationType == BranchDecorationType.ClosedCircle)
                                        {
                                            decorationSvg.Stroke = SvgPaintServer.None;
                                            decorationSvg.Fill = DrawHelpers.CreateSvgColor(currentDecoration.ShapeColor);
                                        }
                                        else
                                        {
                                            decorationSvg.Stroke = DrawHelpers.CreateSvgColor(currentDecoration.ShapeColor);
                                            decorationSvg.Fill = new SvgColourServer(Color.White);
                                        }
                                    }
                                    break;

                                case BranchDecorationType.ClosedRectangle:
                                case BranchDecorationType.OpenedRectangle:
                                    {
                                        (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(current, currentDecoration);

                                        decorationSvg = new SvgRectangle()
                                        {
                                            X = (SvgUnit)x,
                                            Y = (SvgUnit)y,
                                            Width = (SvgUnit)width,
                                            Height = (SvgUnit)height,
                                        };

                                        if (currentDecoration.DecorationType == BranchDecorationType.ClosedRectangle)
                                        {
                                            decorationSvg.Stroke = SvgPaintServer.None;
                                            decorationSvg.Fill = DrawHelpers.CreateSvgColor(currentDecoration.ShapeColor);
                                        }
                                        else
                                        {
                                            decorationSvg.Stroke = DrawHelpers.CreateSvgColor(currentDecoration.ShapeColor);
                                            decorationSvg.StrokeWidth = currentDecoration.ShapeSize / 5 + 1;
                                            decorationSvg.Fill = new SvgColourServer(Color.White);
                                        }
                                    }
                                    break;

                                default: throw new InvalidOperationException($"装飾の種類'{currentDecoration.DecorationType}'が無効です");
                            };
                            decorationSvg.AddTo(branchDecorationsGroup);
                        }
                    }

                    // 枝の値
                    if (tree.Style.ShowBranchValues)
                    {
                        string branchValue = DrawHelpers.SelectShowValue(current, tree.Style.BranchValueType);
                        if (branchValue.Length > 0 && (!tree.Style.BranchValueHideRegex?.IsMatch(branchValue) ?? true))
                        {
                            (double x, double y) = positionManager.CalcBranchValuePosition(current, branchValue);

                            var branchValueText = new SvgText(branchValue)
                            {
                                X = [(SvgUnit)x],
                                Y = [(SvgUnit)y],
                                Fill = DrawHelpers.CreateSvgColor(current.Style.BranchColor),
                                FontSize = tree.Style.BranchValueFontSize,
                                FontFamily = FontFamily,
                                TextAnchor = SvgTextAnchor.Middle,
                            };
                            branchValueText.AddTo(branchValuesGroup);
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
                            var verticalLine = new SvgLine()
                            {
                                StartX = (SvgUnit)x,
                                StartY = (SvgUnit)yChild,
                                EndX = (SvgUnit)x,
                                EndY = (SvgUnit)yParent,
                                Stroke = DrawHelpers.CreateSvgColor(options.BranchColoring is BranchColoringType.Both or BranchColoringType.Vertical ? current.Style.BranchColor : "black"),
                                StrokeWidth = tree.Style.BranchThickness,
                            };
                            branchesGroup.Children.Add(verticalLine);
                        }
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (tree.Style.ShowScaleBar && tree.Style.ScaleBarValue > 0)
            {
                (double offsetX, double offsetY) = positionManager.CalcScaleBarOffset();

                SvgGroup scaleBarArea = new SvgGroup()
                {
                    ID = "scale-bar",
                    Transforms = new SvgTransformCollection().Translate((SvgUnit)offsetX, (SvgUnit)offsetY),
                }.AddTo(result);

                ((double xLeft, double xRight, double y) line, (double x, double y) text) = positionManager.CalcScaleBarPositions();

                var scaleBarText = new SvgText(tree.Style.ScaleBarValue.ToString())
                {
                    X = [(SvgUnit)text.x],
                    Y = [(SvgUnit)text.y],
                    FontFamily = FontFamily,
                    FontSize = tree.Style.ScaleBarFontSize,
                    TextAnchor = SvgTextAnchor.Middle,
                };
                scaleBarText.AddTo(scaleBarArea);

                var scaleBarLine = new SvgLine()
                {
                    StartX = (SvgUnit)line.xLeft,
                    StartY = (SvgUnit)line.y,
                    EndX = (SvgUnit)line.xRight,
                    EndY = (SvgUnit)line.y,
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = tree.Style.ScaleBarThickness,
                };
                scaleBarLine.AddTo(scaleBarArea);
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

            SvgDocument svg = CreateSvg(tree, options);
            svg.Write(destination);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            Export(tree, destination, options);

            await Task.CompletedTask;
        }
    }
}
