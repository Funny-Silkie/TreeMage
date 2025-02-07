using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// 描画のヘルパークラスを表します。
    /// </summary>
    internal static class DrawHelpers
    {
        /// <summary>
        /// 表示するクレードの値を選択します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <param name="valueType">取得する値のタイプ</param>
        /// <returns><paramref name="valueType"/>に応じた<paramref name="clade"/>における値</returns>
        public static string SelectShowValue(Clade clade, CladeValueType valueType)
        {
            switch (valueType)
            {
                case CladeValueType.Supports:
                    string supports = clade.Supports ?? string.Empty;
                    return supports.Trim();

                case CladeValueType.BranchLength:
                    double branchLength = clade.BranchLength;
                    if (double.IsNaN(branchLength)) return string.Empty;
                    return branchLength.ToString();

                default: return string.Empty;
            }
        }
    }
}
