using SixLabors.Fonts;
using System.Diagnostics;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
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
        public static TMSize CalcTextSize(string? text, int fontSize)
        {
            if (string.IsNullOrEmpty(text)) return new TMSize(0, 0);

            FontRectangle rectangle = TextMeasurer.MeasureSize(text, new TextOptions(FontManager.GetImageSharpFont(fontSize)));
            return new TMSize(rectangle.Width, rectangle.Height);
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

            double result = 0;
            for (Clade current = clade; current.Parent is not null; current = current.Parent) result += current.GetDrawnBranchLength();

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

            double result = (CalcTotalBranchLength(clade) - clade.GetDrawnBranchLength()) * treeStyle.XScale;
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
            if (clade.GetDrawnBranchLength() > 0) return CalcTotalBranchLength(clade) * treeStyle.XScale;
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
            if (clade.GetIsExternal())
            {
                int index = indexTable[clade];
                double result;
                if (index == 0) result = 0;
                else
                {
                    Clade prevClade = allExternalNodes[index - 1];
                    double prevYScale = CalcYScale(prevClade);
                    result = CalcY1(prevClade) + (prevClade.Style.Collapsed ? prevYScale / 2 : prevYScale);
                }
                if (clade.Style.Collapsed) result += CalcYScale(clade) / 2;
                return result;
            }
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
        /// Y方向の拡大率を算出します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>におけるY方向の拡大率</returns>
        public double CalcYScale(Clade clade)
        {
            if (!positions.TryGetValue(clade, out PositionInfo? info))
            {
                info = new PositionInfo();
                positions.Add(clade, info);
            }
            if (!double.IsNaN(info.YScale)) return info.YScale;
            double result = CalcYScaleCore(clade);
            info.YScale = result;
            return result;
        }

        /// <summary>
        /// Y方向の拡大率を算出します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>におけるY方向の拡大率</returns>
        private double CalcYScaleCore(Clade clade)
        {
            if (clade.IsRoot) return treeStyle.YScale * clade.Style.YScale;

            return clade.Style.YScale * CalcYScale(clade.Parent);
        }

        /// <summary>
        /// ドキュメントのサイズを取得します。
        /// </summary>
        /// <returns>ドキュメントのサイズ</returns>
        public TMSize CalcDocumentSize()
        {
            double width;
            if (allExternalNodes.Length == 0) width = 0;
            else
            {
                width = allExternalNodes[0].FindRoot()
                                           .GetDescendants()
                                           .Where(x => x.IsLeaf)
                                           .Max(CalcTotalBranchLength) * treeStyle.XScale + 100;
            }

            if (treeStyle.ShowLeafLabels) width += allExternalNodes.Select(x => CalcTextSize(x.Taxon, treeStyle.LeafLabelsFontSize).Width).Max();
            if (treeStyle.ShowCladeLabels && allExternalNodes.Length > 0)
            {
                Clade root = allExternalNodes[0].FindRoot();
                double maxLength = root.GetDescendants()
                                       .Prepend(root)
                                       .Max(x => CalcTextSize(x.Style.CladeLabel, treeStyle.CladeLabelsFontSize).Width);
                if (maxLength > 0) width += maxLength + treeStyle.CladeLabelsLineThickness + 20;
            }

            double height = allExternalNodes.Sum(CalcYScale) + 100;
            if (treeStyle.ShowScaleBar) height += CalcTextSize(treeStyle.ScaleBarValue.ToString(), treeStyle.ScaleBarFontSize).Height + 20;

            return (width, height);
        }

        /// <summary>
        /// シェードの座標を算出します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>のシェードの座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public TMRect CalcCladeShadePosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double CalcXRight(Clade external)
            {
                (double x, _, double width, _) = CalcLeafPosition(external);
                return x + width;
            }

            double xLeft = (CalcX1(clade) + CalcX2(clade)) / 2;
            double xRight, yTop, yBottom;
            double halfLeafHeight = CalcYScale(clade) / 2;

            if (clade.GetIsExternal())
            {
                if (clade.IsLeaf) xRight = CalcXRight(clade);
                else
                {
                    Debug.Assert(clade.ChildrenInternal.Count > 0);

                    double maxLength = clade.GetDescendants()
                                            .Where(x => x.IsLeaf)
                                            .Max(CalcTotalBranchLength);
                    xRight = CalcX2(clade) + (maxLength - CalcTotalBranchLength(clade)) * treeStyle.XScale;
                }

                double y1 = CalcY1(clade);
                yTop = y1 - halfLeafHeight;
                yBottom = y1 + halfLeafHeight;
            }
            else
            {
                Clade[] externals = clade.GetAllExternalDescendants()
                                         .ToArray();
                xRight = externals.Max(CalcXRight);
                yTop = CalcY1(externals[0]) - halfLeafHeight;
                yBottom = CalcY1(externals[^1]) + halfLeafHeight;
            }

            return new TMRect(xLeft, yTop, xRight - xLeft + 5, yBottom - yTop);
        }

        /// <summary>
        /// 折り畳みの三角形の座標を算出します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns>三角形の三点の座標</returns>
        public (TMPoint left, TMPoint rightTop, TMPoint rightBottom) CalcCollapseTrianglePositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double xRightTop, xRightBottom;
            switch (treeStyle.CollapseType)
            {
                case CladeCollapseType.TopMax:
                case CladeCollapseType.BottomMax:
                case CladeCollapseType.AllMax:
                    double maxLength = clade.GetDescendants()
                                            .Where(x => x.IsLeaf)
                                            .Max(CalcTotalBranchLength);
                    switch (treeStyle.CollapseType)
                    {
                        case CladeCollapseType.TopMax:
                            xRightTop = maxLength;
                            xRightBottom = clade.GetDescendants()
                                                .Where(x => x.IsLeaf)
                                                .Min(CalcTotalBranchLength);
                            break;

                        case CladeCollapseType.BottomMax:
                            xRightTop = clade.GetDescendants()
                                             .Where(x => x.IsLeaf)
                                             .Min(CalcTotalBranchLength);
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
            xLeft -= 5;
            xRightTop = xRightTop * treeStyle.XScale + xLeft;
            xRightBottom = xRightBottom * treeStyle.XScale + xLeft;
            double yOffset = Math.Max(CalcYScale(clade) / 2d - 7, 0);

            return (new TMPoint(xLeft, y), new TMPoint(xRightTop, y - yOffset), new TMPoint(xRightBottom, y + yOffset));
        }

        /// <summary>
        /// 葉の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における葉の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public TMRect CalcLeafPosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            (double width, double height) = CalcTextSize(clade.Taxon, treeStyle.LeafLabelsFontSize);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            double y = CalcY1(clade) + height / 2;

            return new TMRect(x, y, width, height);
        }

        /// <summary>
        /// クレード名の座標を算出します。
        /// </summary>
        /// <param name="clade">クレード名</param>
        /// <returns>クレード名の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (TMPoint lineBegin, TMPoint lineEnd, TMPoint text) CalcCladeLabelPosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x, y, yTop, yBottom;
            if (clade.IsLeaf)
            {
                (x, y, double width, double height) = CalcLeafPosition(clade);
                if (treeStyle.ShowLeafLabels) x += width + 10;
                yTop = y - height;
                yBottom = yTop + CalcYScale(clade);
            }
            else if (clade.Style.Collapsed)
            {
                Debug.Assert(clade.ChildrenInternal.Count > 0);

                double maxLength = clade.GetDescendants()
                                        .Where(x => x.IsLeaf)
                                        .Max(CalcTotalBranchLength);

                x = CalcX2(clade) + (maxLength - CalcTotalBranchLength(clade)) * treeStyle.XScale + 10;

                (_, double height) = CalcTextSize(clade.Style.CladeLabel, treeStyle.CladeLabelsFontSize);

                yTop = CalcY1(clade) - CalcYScale(clade) / 2;
                yBottom = CalcY1(clade) + CalcYScale(clade) / 2;
                y = (yTop + yBottom) / 2 + height / 2;
            }
            else
            {
                (_, double height) = CalcTextSize(clade.Style.CladeLabel, treeStyle.CladeLabelsFontSize);

                Clade[] allExternals = clade.GetAllExternalDescendants()
                                            .ToArray();
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
                yTop = CalcY1(allExternals[0]) - CalcYScale(clade) / 2;
                yBottom = CalcY1(allExternals[^1]) + CalcYScale(clade) / 2;
                y = (yTop + yBottom) / 2 + height / 2;
            }

            double lineX = x + treeStyle.CladeLabelsLineThickness / 2d;
            return (new TMPoint(lineX, yTop), new TMPoint(lineX, yBottom), new TMPoint(x + treeStyle.CladeLabelsLineThickness + 5, y));
        }

        /// <summary>
        /// 枝の横棒の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における枝の横棒の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (TMPoint parent, TMPoint child) CalcHorizontalBranchPositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double xParent = CalcX1(clade) - treeStyle.BranchThickness / 2;
            double y = CalcY1(clade);
            double xChild = CalcX2(clade);
            if (clade.IsLeaf) xChild += treeStyle.BranchThickness / 2;

            return (new TMPoint(xParent, y), new TMPoint(xChild, y));
        }

        /// <summary>
        /// 枝の縦棒の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における枝の縦棒の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (TMPoint parent, TMPoint child) CalcVerticalBranchPositions(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = CalcX1(clade);
            return (new TMPoint(x, CalcY2(clade)), new TMPoint(x, CalcY1(clade)));
        }

        /// <summary>
        /// 結節点の値の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="text">表示する値</param>
        /// <returns><paramref name="clade"/>における結節点の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public TMPoint CalcNodeValuePosition(Clade clade, string text)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            (_, double height) = CalcTextSize(text, treeStyle.NodeValueFontSize);
            double y = CalcY1(clade) + height / 2;

            if (clade.ChildrenInternal.Count % 2 == 1) y += treeStyle.BranchThickness / 2 + height / 2 + 3;

            return new TMPoint(x, y);
        }

        /// <summary>
        /// 結節点の装飾の矩形領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>の結節点の装飾の矩形領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public TMRect CalcNodeDecorationRectangleArea(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            const int size = 5;

            double x = CalcX2(clade) - size;
            double y = CalcY1(clade) - size;
            double length = size * 2;

            return new TMRect(x, y, length, length);
        }

        /// <summary>
        /// 枝の値の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="text">表示する値</param>
        /// <returns><paramref name="clade"/>における枝の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public TMPoint CalcBranchValuePosition(Clade clade, string text)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = (CalcX1(clade) + CalcX2(clade)) / 2;
            (_, double height) = CalcTextSize(text, treeStyle.BranchValueFontSize);
            double y = CalcY1(clade) - height / 2 - treeStyle.BranchThickness / 2;

            return new TMPoint(x, y);
        }

        /// <summary>
        /// 枝の装飾の矩形領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="decorationStyle">装飾スタイル</param>
        /// <returns><paramref name="clade"/>の枝の装飾の矩形領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>または<paramref name="decorationStyle"/>が<see langword="null"/></exception>
        public TMRect CalcBranchDecorationRectangleArea(Clade clade, BranchDecorationStyle decorationStyle)
        {
            ArgumentNullException.ThrowIfNull(clade);
            ArgumentNullException.ThrowIfNull(decorationStyle);

            double xParent = CalcX1(clade);
            double xChild = CalcX2(clade);
            int size = decorationStyle.ShapeSize;

            double x = (xParent + xChild) / 2 - size;
            double y = CalcY1(clade) - size;
            double length = size * 2;

            return new TMRect(x, y, length, length);
        }

        /// <summary>
        /// 枝の装飾の円領域を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <param name="decorationStyle">装飾スタイル</param>
        /// <returns><paramref name="clade"/>の枝の装飾の円領域</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>または<paramref name="decorationStyle"/>が<see langword="null"/></exception>
        public (TMPoint center, double radius) CalcBranchDecorationCircleArea(Clade clade, BranchDecorationStyle decorationStyle)
        {
            ArgumentNullException.ThrowIfNull(clade);
            ArgumentNullException.ThrowIfNull(decorationStyle);

            double xParent = CalcX1(clade);
            double xChild = CalcX2(clade);

            double x = (xParent + xChild) / 2;
            double y = CalcY1(clade);

            return (new TMPoint(x, y), decorationStyle.ShapeSize);
        }

        /// <summary>
        /// スケールバーの座標オフセットを算出します。
        /// </summary>
        /// <returns>スケールバーの座標オフセット</returns>
        public TMPoint CalcScaleBarOffset()
        {
            TMSize textSize = CalcTextSize(treeStyle.ScaleBarValue.ToString(), treeStyle.ScaleBarFontSize);
            double y = allExternalNodes.Sum(CalcYScale) + 30 + textSize.Height;

            return new TMPoint(50 + textSize.Width / 2, 20 + y);
        }

        /// <summary>
        /// スケールバーの座標を算出します。
        /// </summary>
        /// <returns>スケールバーの座標</returns>
        public (TMPoint lineBegin, TMPoint lineEnd, TMPoint text) CalcScaleBarPositions()
        {
            double barWidth = treeStyle.ScaleBarValue * treeStyle.XScale;

            return (new TMPoint(0, 10), new TMPoint(barWidth, 10), new TMPoint(barWidth / 2, 0));
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
            public double YScale;

            /// <summary>
            /// <see cref="PositionInfo"/>の新しいインスタンスを初期化します。
            /// </summary>
            public PositionInfo()
            {
                X1 = X2 = Y1 = Y2 = TotalLength = YScale = double.NaN;
            }
        }
    }
}
