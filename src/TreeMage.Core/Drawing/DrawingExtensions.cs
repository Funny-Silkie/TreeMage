using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// 描画用の拡張を表します。
    /// </summary>
    public static class DrawingExtensions
    {
        /// <summary>
        /// 全ての外部ノードを取得します。
        /// </summary>
        /// <param name="tree">対象のツリー</param>
        /// <returns>全ての外部ノード。葉または折りたたまれたクレード</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>が<see langword="null"/></exception>
        public static IEnumerable<Clade> GetAllExternalNodes(this Tree tree)
        {
            ArgumentNullException.ThrowIfNull(tree);

            return GetAllExternalNodesCore(tree);
        }

        /// <summary>
        /// 全ての外部ノードを取得します。
        /// </summary>
        /// <param name="tree">対象のツリー</param>
        /// <returns>全ての外部ノード。葉または折りたたまれたクレード</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>が<see langword="null"/></exception>
        private static IEnumerable<Clade> GetAllExternalNodesCore(Tree tree)
        {
            ArgumentNullException.ThrowIfNull(tree);

            if (tree.Root.IsLeaf)
            {
                yield return tree.Root;
                yield break;
            }

            foreach (Clade current in GetAllExternalNodesOnClade(tree.Root)) yield return current;
        }

        /// <summary>
        /// 全ての外部ノードを取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns>全ての外部ノード。葉または折りたたまれたクレード</returns>
        private static IEnumerable<Clade> GetAllExternalNodesOnClade(Clade clade)
        {
            foreach (Clade child in clade.ChildrenInternal)
            {
                if (child.IsLeaf || child.Style.Collapsed) yield return child;
                else
                    foreach (Clade descendant in GetAllExternalNodesOnClade(child))
                        yield return descendant;
            }
        }

        /// <summary>
        /// 子孫の全ての外部ノードを取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns>全ての外部ノード</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public static IEnumerable<Clade> GetAllExternalDescendants(this Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            return GetAllExternalNodesOnClade(clade);
        }

        /// <summary>
        /// 描画されないクレードであるかどうかを表す値を取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>が描画される場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public static bool GetIsHidden(this Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            if (clade.IsRoot) return false;

            for (Clade? current = clade.Parent; current is not null; current = current.Parent)
                if (current.Style.Collapsed)
                    return true;
            return false;
        }

        /// <summary>
        /// 外部ノードであるかどうかを表す値を取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>が外部ノードであれば<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public static bool GetIsExternal(this Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            return clade.IsLeaf || clade.Style.Collapsed;
        }

        /// <summary>
        /// 描画される枝長を取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>の描画時の枝長</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public static double GetDrawnBranchLength(this Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);

            if (clade.IsRoot) return 0;
            if (!double.IsNaN(clade.BranchLength)) return clade.BranchLength;
            if (clade.Tree is not null) return clade.Tree.Style.DefaultBranchLength;
            return 0;
        }
    }
}
