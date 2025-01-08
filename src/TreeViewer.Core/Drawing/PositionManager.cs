using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing
{
    /// <summary>
    /// 座標の管理を行います。
    /// </summary>
    public class PositionManager
    {
        //
        // y1   +----current
        //      |
        // y2   |
        //   .Parent
        //      x1  x2

        private readonly Dictionary<Clade, PositionInfo> positions = [];
        private Clade[] allExternalNodes;
        private readonly Dictionary<Clade, int> indexTable;
        private TreeStyle treeStyle;

        /// <summary>
        /// <see cref="PositionManager"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PositionManager()
        {
            allExternalNodes = [];
            indexTable = [];
            treeStyle = new TreeStyle();
        }

        /// <summary>
        /// <see cref="PositionManager"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="tree">対象の樹形</param>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>が<see langword="null"/></exception>
        public PositionManager(Tree tree)
        {
            ArgumentNullException.ThrowIfNull(tree);

            allExternalNodes = tree.GetAllExternalNodes()
                                   .ToArray();
            treeStyle = tree.Style;
            indexTable = allExternalNodes.Select((x, i) => (x, i))
                                         .ToDictionary();
        }

        /// <summary>
        /// テキストのサイズを算出します。
        /// </summary>
        /// <param name="text">テキスト内容</param>
        /// <param name="fontSize">フォントサイズ</param>
        /// <returns>テキストエリアのサイズ</returns>
        public static (double width, double height) CalcTextSize(string? text, int fontSize)
        {
            var svg = new Svg.SvgText(text)
            {
                FontSize = fontSize,
                FontFamily = SvgExporter.FontFamily,
            };
            System.Drawing.SizeF size = svg.Bounds.Size;
            return (size.Width, size.Height);
        }

        /// <summary>
        /// 枝長の合計を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>の合計枝長</returns>
        public double CalcTotalBranchLength(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.TotalLength)) return info.TotalLength;

            double result = clade.GetTotalBranchLength();
            info.TotalLength = result;
            return result;
        }

        /// <summary>
        /// X座標1を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のX座標1</returns>
        public double CalcX1(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.X1)) return info.X1;

            double result = (CalcTotalBranchLength(clade) - clade.BranchLength) * treeStyle.XScale;
            info.X1 = result;
            return result;
        }

        /// <summary>
        /// X座標2を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のX座標2</returns>
        public double CalcX2(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.X2)) return info.X2;

            double result = CalcX2Core(clade);
            info.X2 = result;
            return result;
        }

        /// <summary>
        /// X座標2を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のX座標2</returns>
        private double CalcX2Core(Clade clade)
        {
            if (clade.BranchLength > 0) return CalcTotalBranchLength(clade) * treeStyle.XScale;
            return CalcX1(clade);
        }

        /// <summary>
        /// Y座標1を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のY座標1</returns>
        public double CalcY1(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.Y1)) return info.Y1;

            double result = CalcY1Core(clade);
            info.Y1 = result;
            return result;
        }

        /// <summary>
        /// Y座標1を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のY座標1</returns>
        private double CalcY1Core(Clade clade)
        {
            if (clade.GetIsExternal()) return indexTable[clade] * treeStyle.YScale;
            if (clade.Children.Count == 1) return CalcY2(clade.Children[0]);
            return (CalcY2(clade.Children[0]) + CalcY2(clade.Children[^1])) / 2;
        }

        /// <summary>
        /// Y座標2を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のY座標2</returns>
        public double CalcY2(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.Y2)) return info.Y2;

            double result = CalcY2Core(clade, CalcY1(clade));
            info.Y2 = result;
            return result;
        }

        /// <summary>
        /// Y座標2を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <param name="y1"><paramref name="clade"/>のY座標1</param>
        /// <returns><paramref name="clade"/>のY座標2</returns>
        private double CalcY2Core(Clade clade, double y1)
        {
            if (clade.Parent is null) return y1;

            IList<Clade> sisters = clade.Parent.Children;
            int indexOfClade = sisters.IndexOf(clade);
            (int halfCount, int countRem) = Math.DivRem(sisters.Count, 2);

            if (countRem == 1 && indexOfClade == halfCount) return y1;
            if (indexOfClade < halfCount)
            {
                double otherY1 = CalcY1(sisters[indexOfClade + 1]);
                double length = otherY1 - y1;
                if (countRem == 0 && indexOfClade + 1 == halfCount) length /= 2;
                return y1 + length;
            }
            else
            {
                double otherY1 = CalcY1(sisters[indexOfClade - 1]);
                double length = y1 - otherY1;
                if (countRem == 0 && indexOfClade == halfCount) length /= 2;
                return y1 - length;
            }
        }

        /// <summary>
        /// ドキュメントのサイズを取得します。
        /// </summary>
        /// <returns>ドキュメントのサイズ</returns>
        public (double width, double height) CalcDocumentSize()
        {
            double width = allExternalNodes.Select(x => x.GetTotalBranchLength()).Max() * treeStyle.XScale + 100;
            if (treeStyle.ShowLeafLabels) width += allExternalNodes.Select(x => CalcTextSize(x.Taxon, treeStyle.LeafLabelsFontSize).width).Max();
            if (treeStyle.ShowCladeLabels && allExternalNodes.Length > 0)
            {
                Clade root = allExternalNodes[0].FindRoot();
                double maxLength = root.GetDescendants().Prepend(root).Max(x => CalcTextSize(x.Style.CladeLabel, treeStyle.CladeLabelsFontSize).width);
                if (maxLength > 0) width += maxLength + treeStyle.CladeLabelsLineThickness + 20;
            }

            double height = allExternalNodes.Length * treeStyle.YScale + 100;
            if (treeStyle.ShowScaleBar) height += CalcTextSize(treeStyle.ScaleBarValue.ToString(), treeStyle.ScaleBarFontSize).height + 20;

            return (width, height);
        }

        /// <summary>
        /// 折り畳みの三角形の座標を算出します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns>三角形の三点の座標</returns>
        public ((double x, double y) left, (double x, double y) rightTop, (double x, double y) rightBottom) CalcCollapseTrianglePositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double xRightTop, xRightBottom;
            switch (treeStyle.CollapseType)
            {
                case CladeCollapseType.TopMax:
                case CladeCollapseType.BottomMax:
                case CladeCollapseType.AllMax:
                    double maxLength = clade.GetDescendants().Where(x => x.IsLeaf).Max(CalcTotalBranchLength);
                    switch (treeStyle.CollapseType)
                    {
                        case CladeCollapseType.TopMax:
                            xRightTop = maxLength;
                            xRightBottom = clade.GetDescendants().Where(x => x.IsLeaf).Min(CalcTotalBranchLength);
                            break;

                        case CladeCollapseType.BottomMax:
                            xRightTop = clade.GetDescendants().Where(x => x.IsLeaf).Min(CalcTotalBranchLength);
                            xRightBottom = maxLength;
                            break;

                        case CladeCollapseType.AllMax:
                            xRightTop = xRightBottom = maxLength;
                            break;

                        default: throw new InvalidOperationException();
                    }

                    double totalLength = CalcTotalBranchLength(clade);
                    xRightTop -= totalLength;
                    xRightBottom -= totalLength;
                    break;

                default:
                    xRightTop = xRightBottom = treeStyle.CollapsedConstantWidth;
                    break;
            }

            (double xLeft, double y, _, _) = CalcLeafPosition(clade);
            xRightTop = xRightTop * treeStyle.XScale + xLeft;
            xRightBottom = xRightBottom * treeStyle.XScale + xLeft;
            double yOffset = treeStyle.YScale / 2d;

            return ((xLeft, y), (xRightTop, y - yOffset), (xRightBottom, y + yOffset));
        }

        /// <summary>
        /// 葉の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における葉の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y, double width, double height) CalcLeafPosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            (double width, double height) = CalcTextSize(clade.Taxon, treeStyle.LeafLabelsFontSize);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            double y = CalcY1(clade) + height / 2;

            return (x, y, width, height);
        }

        /// <summary>
        /// クレード名の座標を算出します。
        /// </summary>
        /// <param name="clade">クレード名</param>
        /// <returns>クレード名の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public ((double x, double yTop, double yBottom) line, (double x, double y) text) CalcCladeLabelPosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x, y, yTop, yBottom;
            if (clade.GetIsExternal())
            {
                (x, y, double width, double height) = CalcLeafPosition(clade);
                if (treeStyle.ShowLeafLabels) x += width + 10;
                yTop = y - height;
                yBottom = yTop + treeStyle.YScale;
            }
            else
            {
                (_, double height) = CalcTextSize(clade.Style.CladeLabel, treeStyle.CladeLabelsFontSize);

                Clade[] allExternals = clade.GetDescendants().Where(x => x.GetIsExternal()).ToArray();
                x = allExternals.Max(x =>
                {
                    double result = CalcTotalBranchLength(x) * treeStyle.XScale;
                    if (treeStyle.ShowLeafLabels && !string.IsNullOrEmpty(x.Taxon))
                    {
                        (double width, _) = CalcTextSize(x.Taxon, treeStyle.LeafLabelsFontSize);
                        result += width + 15;
                    }

                    return result;
                });
                yTop = CalcY1(allExternals[0]) - treeStyle.YScale / 2;
                yBottom = CalcY1(allExternals[^1]) + treeStyle.YScale / 2;
                y = (yTop + yBottom) / 2 + height / 2;
            }

            return ((x + treeStyle.CladeLabelsLineThickness / 2d, yTop, yBottom), (x + treeStyle.CladeLabelsLineThickness + 5, y));
        }

        /// <summary>
        /// 枝の横棒の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における枝の横棒の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double xParent, double xChild, double y) CalcHorizontalBranchPositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double xParent = CalcX1(clade) - treeStyle.BranchThickness / 2;
            double y = CalcY1(clade);
            double xChild = CalcX2(clade);
            if (clade.IsLeaf) xChild += treeStyle.BranchThickness / 2;

            return (xParent, xChild, y);
        }

        /// <summary>
        /// 枝の縦棒の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における枝の縦棒の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double yParent, double yChild) CalcVerticalBranchPositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            return (CalcX1(clade), CalcY2(clade), CalcY1(clade));
        }

        /// <summary>
        /// 結節点の値の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="text">表示する値</param>
        /// <returns><paramref name="clade"/>における結節点の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y) CalcNodeValuePosition(Clade clade, string text)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            (_, double height) = CalcTextSize(text, treeStyle.NodeValueFontSize);
            double y = CalcY1(clade) + height / 2;

            if (clade.ChildrenInternal.Count % 2 == 1) y += treeStyle.BranchThickness / 2 + height / 2 + 3;

            return (x, y);
        }

        /// <summary>
        /// 結節点の装飾の矩形領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>の結節点の装飾の矩形領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y, double width, double height) CalcNodeDecorationRectangleArea(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            const int size = 5;

            double x = CalcX2(clade) - size;
            double y = CalcY1(clade) - size;
            double length = size * 2;

            return (x, y, length, length);
        }

        /// <summary>
        /// 枝の値の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="text">表示する値</param>
        /// <returns><paramref name="clade"/>における枝の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y) CalcBranchValuePosition(Clade clade, string text)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = (CalcX1(clade) + CalcX2(clade)) / 2;
            (_, double height) = CalcTextSize(text, treeStyle.BranchValueFontSize);
            double y = CalcY1(clade) - height / 2 - treeStyle.BranchThickness / 2;

            return (x, y);
        }

        /// <summary>
        /// 枝の装飾の矩形領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="decorationStyle">装飾スタイル</param>
        /// <returns><paramref name="clade"/>の枝の装飾の矩形領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>または<paramref name="decorationStyle"/>が<see langword="null"/></exception>
        public (double x, double y, double width, double height) CalcBranchDecorationRectangleArea(Clade clade, BranchDecorationStyle decorationStyle)
        {
            ArgumentNullException.ThrowIfNull(clade);
            ArgumentNullException.ThrowIfNull(decorationStyle);

            double xParent = CalcX1(clade);
            double xChild = CalcX2(clade);
            int size = decorationStyle.ShapeSize;

            double x = (xParent + xChild) / 2 - size;
            double y = CalcY1(clade) - size;
            double length = size * 2;

            return (x, y, length, length);
        }

        /// <summary>
        /// 枝の装飾の円領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="decorationStyle">装飾スタイル</param>
        /// <returns><paramref name="clade"/>の枝の装飾の円領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>または<paramref name="decorationStyle"/>が<see langword="null"/></exception>
        public (double centerX, double centerY, double radius) CalcBranchDecorationCircleArea(Clade clade, BranchDecorationStyle decorationStyle)
        {
            ArgumentNullException.ThrowIfNull(clade);
            ArgumentNullException.ThrowIfNull(decorationStyle);

            double xParent = CalcX1(clade);
            double xChild = CalcX2(clade);

            double x = (xParent + xChild) / 2;
            double y = CalcY1(clade);

            return (x, y, decorationStyle.ShapeSize);
        }

        /// <summary>
        /// スケールバーの座標オフセットを算出します。
        /// </summary>
        /// <returns>スケールバーの座標オフセット</returns>
        public (double x, double y) CalcScaleBarOffset()
        {
            (double textWidth, double textHeight) = CalcTextSize(treeStyle.ScaleBarValue.ToString(), treeStyle.ScaleBarFontSize);
            double y = indexTable.Count * treeStyle.YScale + 30 + textHeight;

            return (50 + textWidth / 2, 20 + y);
        }

        /// <summary>
        /// スケールバーの座標を算出します。
        /// </summary>
        /// <returns>スケールバーの座標</returns>
        public ((double xLeft, double xRight, double y) line, (double x, double y) text) CalcScaleBarPositions()
        {
            double barWidth = treeStyle.ScaleBarValue * treeStyle.XScale;

            return ((0, barWidth, 10), (barWidth / 2, 0));
        }

        /// <summary>
        /// キャッシュされた値のリセットを行います。
        /// </summary>
        public void ClearCache()
        {
            positions.Clear();
        }

        /// <summary>
        /// 値のリセットを行います。
        /// </summary>
        /// <param name="tree">読み込むツリー</param>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>が<see langword="null"/></exception>
        public void Reset(Tree tree)
        {
            ArgumentNullException.ThrowIfNull(tree);

            positions.Clear();
            indexTable.Clear();

            treeStyle = tree.Style;
            allExternalNodes = tree.GetAllExternalNodes()
                                   .ToArray();
            for (int i = 0; i < allExternalNodes.Length; i++) indexTable.Add(allExternalNodes[i], i);
        }

        internal sealed class PositionInfo
        {
            public double X1;
            public double X2;
            public double Y1;
            public double Y2;
            public double TotalLength;

            /// <summary>
            /// <see cref="PositionInfo"/>の新しいインスタンスを初期化します。
            /// </summary>
            public PositionInfo()
            {
                X1 = X2 = Y1 = Y2 = TotalLength = double.NaN;
            }
        }
    }
}
