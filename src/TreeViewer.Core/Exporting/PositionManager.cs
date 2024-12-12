using TreeViewer.Core.Styles;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// 座標の管理を行います。
    /// </summary>
    public class PositionManager
    {
        private readonly Dictionary<Clade, PositionInfo> positions = [];
        private Clade[] allLeaves;
        private readonly Dictionary<Clade, int> indexTable;
        private TreeStyle treeStyle;

        /// <summary>
        /// <see cref="PositionManager"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PositionManager()
        {
            allLeaves = [];
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

            allLeaves = tree.GetAllLeaves().ToArray();
            treeStyle = tree.Style;
            indexTable = tree.GetAllLeaves()
                             .Select((x, i) => (x, i))
                             .ToDictionary();
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
            if (clade.IsLeaf) return indexTable[clade] * treeStyle.YScale;
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
            IList<Clade> sisters = clade.Parent!.Children;
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
        public (double width, double height) ClacDocumentSize()
        {
            double width = allLeaves.Select(x => x.GetTotalBranchLength()).Max() * treeStyle.XScale + 100;
            if (treeStyle.ShowLeafLabels) width += allLeaves.Select(x => (x.Taxon ?? string.Empty).Length).Max() * treeStyle.LeafLabelsFontSize / 1.25;

            double height = allLeaves.Length * treeStyle.YScale + 100;

            return (width, height);
        }

        /// <summary>
        /// 葉の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における葉の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y) CalcLeafPosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            double y = CalcY1(clade) + treeStyle.LeafLabelsFontSize / 2.5;

            return (x, y);
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
        /// <returns><paramref name="clade"/>における結節点の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y) CalcNodeValuePosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = CalcTotalBranchLength(clade) * treeStyle.XScale + 5;
            double y = CalcY1(clade) + treeStyle.NodeValueFontSize / 2.5;
            if (clade.ChildrenInternal.Count % 2 == 1) y += treeStyle.BranchThickness / 2 + treeStyle.NodeValueFontSize / 2.5 + 3;

            return (x, y);
        }

        /// <summary>
        /// 枝の値の座標を算出します。
        /// </summary>
        /// <param name="clade">計算対象</param>
        /// <returns><paramref name="clade"/>における枝の値の座標</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public (double x, double y) CalcBranchValuePosition(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            double x = (CalcX1(clade) + CalcX2(clade)) / 2;
            double y = CalcY1(clade) - treeStyle.BranchValueFontSize / 2.5 - treeStyle.BranchThickness / 2;

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
            double y = indexTable.Count * treeStyle.YScale + 30 + treeStyle.ScaleBarFontSize;

            return (50, y);
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
            allLeaves = tree.GetAllLeaves().ToArray();
            foreach ((int index, Clade clade) in tree.GetAllLeaves().Index()) indexTable.Add(clade, index);
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
