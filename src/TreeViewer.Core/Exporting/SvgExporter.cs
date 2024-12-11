﻿using Svg;
using Svg.Transforms;
using System.Drawing;
using TreeViewer.Core.Internal;
using TreeViewer.Core.Styles;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// SVGへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class SvgExporter : IExporter
    {
        private const string FontFamily = "Arial, Helvetica, sans-serif";

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
        /// <param name="options">オプション</param>
        /// <returns><paramref name="tree"/>の図を表すSVGオブジェクト</returns>
        internal static SvgDocument CreateSvg(Tree tree, ExportOptions options)
        {
            Clade[] allLeaves = tree.GetAllLeaves()
                                    .ToArray();

            double svgWidth = allLeaves.Select(x => x.GetTotalBranchLength()).Max() * options.XScale + 100;
            if (options.ShowLeafLabels) svgWidth += allLeaves.Select(x => (x.Taxon ?? string.Empty).Length).Max() * options.LeafLabelsFontSize / 1.25;

            var result = new SvgDocument()
            {
                Width = (SvgUnit)svgWidth,
                Height = allLeaves.Length * options.YScale + 100,
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
            if (options.ShowLeafLabels) leavesGroup.AddTo(treeArea);
            SvgGroup branchesGroup = new SvgGroup()
            {
                ID = "branches",
            }.AddTo(treeArea);
            var nodeValuesGroup = new SvgGroup()
            {
                ID = "node-values",
            };
            if (options.ShowNodeValues) nodeValuesGroup.AddTo(treeArea);
            var branchValuesGroup = new SvgGroup()
            {
                ID = "branch-values",
            };
            if (options.ShowBranchValues) branchValuesGroup.AddTo(treeArea);
            var branchDecorationsGroup = new SvgGroup()
            {
                ID = "branch-decorations",
            };
            if (options.ShowBranchDecorations) branchDecorationsGroup.AddTo(treeArea);

            var positionManager = new PositionManager(options, tree);

            foreach (Clade current in tree.GetAllClades())
            {
                double totalLength = positionManager.CalcTotalBranchLength(current);

                if (current.IsLeaf)
                {
                    double x = totalLength * options.XScale + 5;
                    double y = positionManager.CalcY1(current) + options.LeafLabelsFontSize / 2.5;

                    // 系統名
                    if (options.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                    {
                        var leafText = new SvgText(current.Taxon)
                        {
                            X = [(SvgUnit)x],
                            Y = [(SvgUnit)y],
                            Fill = ExportHelpers.CreateSvgColor(current.Style.LeafColor),
                            FontSize = options.LeafLabelsFontSize,
                            FontFamily = FontFamily,
                        };
                        leafText.AddTo(leavesGroup);
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

                            var nodeValueText = new SvgText(nodeValue)
                            {
                                X = [(SvgUnit)(totalLength * options.XScale + 5)],
                                Y = [(SvgUnit)y],
                                Fill = ExportHelpers.CreateSvgColor(current.Style.BranchColor),
                                FontSize = options.NodeValueFontSize,
                                FontFamily = FontFamily,
                            };
                            nodeValueText.AddTo(nodeValuesGroup);
                        }
                    }
                }

                double x1 = positionManager.CalcX1(current);
                double x2 = positionManager.CalcX2(current);
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
                            Stroke = ExportHelpers.CreateSvgColor(current.Style.BranchColor),
                            StrokeWidth = options.BranchThickness,
                        };
                        horizontalLine.AddTo(branchesGroup);
                    }

                    // 枝の装飾
                    if (options.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                    {
                        foreach (BranchDecorationStyle currentDecoration in options.DecorationStyles.Where(x => x.Regex.IsMatch(current.Supports)))
                        {
                            int size = currentDecoration.ShapeSize;
                            string color = currentDecoration.ShapeColor;
                            SvgElement decorationSvg = currentDecoration.DecorationType switch
                            {
                                BranchDecorationType.ClosedCircle => new SvgCircle()
                                {
                                    CenterX = (SvgUnit)((x1 + x2) / 2),
                                    CenterY = (SvgUnit)positionManager.CalcY1(current),
                                    Radius = size,
                                    Stroke = SvgPaintServer.None,
                                    Fill = ExportHelpers.CreateSvgColor(currentDecoration.ShapeColor),
                                },
                                BranchDecorationType.OpenCircle => new SvgCircle()
                                {
                                    CenterX = (SvgUnit)((x1 + x2) / 2),
                                    CenterY = (SvgUnit)positionManager.CalcY1(current),
                                    Radius = size,
                                    Stroke = ExportHelpers.CreateSvgColor(currentDecoration.ShapeColor),
                                    Fill = new SvgColourServer(Color.White),
                                },
                                BranchDecorationType.ClosedRectangle => new SvgRectangle()
                                {
                                    X = (SvgUnit)((x1 + x2) / 2 - size),
                                    Y = (SvgUnit)(positionManager.CalcY1(current) - size),
                                    Width = size * 2,
                                    Height = size * 2,
                                    Stroke = SvgPaintServer.None,
                                    Fill = ExportHelpers.CreateSvgColor(currentDecoration.ShapeColor),
                                },
                                BranchDecorationType.OpenedRectangle => new SvgRectangle()
                                {
                                    X = (SvgUnit)((x1 + x2) / 2 - size),
                                    Y = (SvgUnit)(positionManager.CalcY1(current) - size),
                                    Width = size * 2,
                                    Height = size * 2,
                                    Stroke = ExportHelpers.CreateSvgColor(currentDecoration.ShapeColor),
                                    StrokeWidth = size / 5 + 1,
                                    Fill = new SvgColourServer(Color.White),
                                },
                                _ => throw new InvalidOperationException($"装飾の種類'{currentDecoration.DecorationType}'が無効です"),
                            };
                            decorationSvg.AddTo(branchDecorationsGroup);
                        }
                    }

                    // 二分岐の値
                    if (options.ShowBranchValues)
                    {
                        string branchValue = ExportHelpers.SelectShowValue(current, options.BranchValueType);
                        if (branchValue.Length > 0)
                        {
                            var branchValueText = new SvgText(branchValue)
                            {
                                X = [(SvgUnit)((x1 + x2) / 2)],
                                Y = [(SvgUnit)(positionManager.CalcY1(current) - options.BranchValueFontSize / 2.5 - options.BranchThickness / 2)],
                                Fill = ExportHelpers.CreateSvgColor(current.Style.BranchColor),
                                FontSize = options.BranchValueFontSize,
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
                                Stroke = ExportHelpers.CreateSvgColor(current.Style.BranchColor),
                                StrokeWidth = options.BranchThickness,
                            };
                            branchesGroup.Children.Add(verticalLine);
                        }
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (options.ShowScaleBar && options.ScaleBarValue > 0)
            {
                SvgGroup scaleBarArea = new SvgGroup()
                {
                    ID = "scale-bar",
                    Transforms = new SvgTransformCollection().Translate(50, (float)(allLeaves.Length * options.YScale + 30 + options.ScaleBarFontSize)),
                }.AddTo(result);

                double scaleBarWidth = options.ScaleBarValue * options.XScale;
                var scaleBarText = new SvgText(options.ScaleBarValue.ToString())
                {
                    X = [(SvgUnit)(scaleBarWidth / 2)],
                    FontFamily = FontFamily,
                    FontSize = options.ScaleBarFontSize,
                    TextAnchor = SvgTextAnchor.Middle,
                };
                scaleBarText.AddTo(scaleBarArea);

                var scaleBarLine = new SvgLine()
                {
                    StartX = 0,
                    StartY = 10,
                    EndX = (SvgUnit)scaleBarWidth,
                    EndY = 10,
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = options.ScaleBarThickness,
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
