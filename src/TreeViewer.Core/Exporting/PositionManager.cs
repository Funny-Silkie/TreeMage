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
        private readonly Dictionary<Clade, int> indexTable;
        private TreeStyle treeStyle;

        /// <summary>
        /// <see cref="PositionManager"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PositionManager()
        {
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
