using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// 座標の管理を行います。
    /// </summary>
    internal class PositionManager
    {
        private readonly Dictionary<Clade, double> y1Map = [];
        private readonly Dictionary<Clade, double> y2Map = [];
        private readonly ExportOptions options;
        private readonly Dictionary<Clade, int> indexTable;

        /// <summary>
        /// <see cref="PositionManager"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">オプション</param>
        /// <param name="indexTable">クレードとインデックスの関係</param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/>または<paramref name="indexTable"/>が<see langword="null"/></exception>
        public PositionManager(ExportOptions options, Dictionary<Clade, int> indexTable)
        {
            ArgumentNullException.ThrowIfNull(options);

            this.options = options;
            this.indexTable = indexTable;
        }

        /// <summary>
        /// Y座標1を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のY座標1</returns>
        public double CalcY1(Clade clade)
        {
            if (y1Map.TryGetValue(clade, out double result)) return result;

            if (clade.IsLeaf) result = indexTable[clade] * options.YScale;
            else
            {
                if (clade.Children.Count == 1) result = CalcY2(clade.Children[0]);
                result = (CalcY2(clade.Children[0]) + CalcY2(clade.Children[^1])) / 2;
            }
            y1Map.Add(clade, result);
            return result;
        }

        /// <summary>
        /// Y座標2を算出します。
        /// </summary>
        /// <param name="clade">計算を行うクレード</param>
        /// <returns><paramref name="clade"/>のY座標2</returns>
        public double CalcY2(Clade clade)
        {
            if (y2Map.TryGetValue(clade, out double result)) return result;

            result = CalcY2Core(clade, CalcY1(clade));
            y2Map.Add(clade, result);
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
        /// キャッシュされた値のリセットを行います。
        /// </summary>
        public void Reset()
        {
            y1Map.Clear();
            y2Map.Clear();
        }
    }
}
