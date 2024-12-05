namespace TreeViewer.Core.Trees
{
    /// <summary>
    /// 系統樹を表します。
    /// </summary>
    public class Tree : ICloneable
    {
        /// <summary>
        /// ルートとなる<see cref="Clade"/>のインスタンスを取得します。
        /// </summary>
        public Clade Root { get; private set; }

        /// <summary>
        /// <see cref="Tree"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="root">ルートとなる<see cref="Clade"/>のインスタンスs</param>
        /// <exception cref="ArgumentNullException"><paramref name="root"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="root"/>がルートを表していないまたは別のツリーに既に属している</exception>
        internal Tree(Clade root)
        {
            ArgumentNullException.ThrowIfNull(root);
            if (!root.IsRoot) throw new ArgumentException("ルートを表していません", nameof(root));
            if (root.Tree is not null) throw new ArgumentException("既に別のツリーに属しています", nameof(root));

            Root = root;
            root.TreeInternal = this;
        }

        [Obsolete]
        public static Tree CreateSample()
        {
            //          2                              5
            //      +------leafA    1         +---------------leafBAA
            // root-|   2         +---cladeBA-|     3
            //      |             |           +---------leafBAB  2
            //      +------cladeB-|                1          +------leafBBAA
            //      |             |   2          +---cladeBBA-| 1
            //      | 1           +------cladeBB-|    3       +---leafBBAB
            //      +---leafC                    +---------leafBBB

            var root = new Clade()
            {
                Supports = "80/100",
                BranchLength = 0,
            };
            var leafA = new Clade()
            {
                Taxon = "A",
                BranchLength = 2,
            };
            root.AddChild(leafA);
            var cladeB = new Clade()
            {
                Supports = "30/45",
                BranchLength = 2,
            };
            root.AddChild(cladeB);
            var leafC = new Clade()
            {
                Taxon = "C",
                BranchLength = 1,
            };
            root.AddChild(leafC);

            var cladeBA = new Clade()
            {
                Supports = "20/30",
                BranchLength = 1,
            };
            cladeB.AddChild(cladeBA);
            var cladeBB = new Clade()
            {
                BranchLength = 2,
                Supports = "100/100",
            };
            cladeB.AddChild(cladeBB);

            var leafBAA = new Clade()
            {
                Taxon = "BAA",
                BranchLength = 5,
            };
            cladeBA.AddChild(leafBAA);
            var leafBAB = new Clade()
            {
                Taxon = "BAB",
                BranchLength = 3,
            };
            cladeBA.AddChild(leafBAB);

            var cladeBBA = new Clade()
            {
                BranchLength = 1,
                Supports = "85/95",
            };
            cladeBB.AddChild(cladeBBA);
            var leafBBB = new Clade()
            {
                Taxon = "BBB",
                BranchLength = 3,
            };
            cladeBB.AddChild(leafBBB);

            var leafBBAA = new Clade()
            {
                Taxon = "BBAA",
                BranchLength = 2,
            };
            cladeBBA.AddChild(leafBBAA);

            var leafBBAB = new Clade()
            {
                Taxon = "BBAB",
                BranchLength = 1,
            };
            cladeBBA.AddChild(leafBBAB);

            return new Tree(root);
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public Tree Clone() => new Tree(Root.Clone(true));

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 属する全ての<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての<see cref="Clade"/>のインスタンス</returns>
        public IEnumerable<Clade> GetAllClades()
        {
            yield return Root;
            foreach (Clade currentClade in Root.GetDescendants()) yield return currentClade;
        }

        /// <summary>
        /// 全ての二分岐を表す<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての二分岐を表す<see cref="Clade"/>のインスタンス一覧</returns>
        public IEnumerable<Clade> GetAllBipartitions() => GetAllClades().Where(x => !x.IsLeaf);

        /// <summary>
        /// 全ての葉を表す<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての葉を表す<see cref="Clade"/>のインスタンス一覧</returns>
        public IEnumerable<Clade> GetAllLeaves() => GetAllClades().Where(x => x.IsLeaf);

        /// <summary>
        /// 枝長を全て削除します。
        /// </summary>
        public void ClearAllBranchLengthes()
        {
            foreach (Clade currentClade in GetAllClades()) currentClade.BranchLength = double.NaN;
        }

        /// <summary>
        /// サポート値を全て削除します。
        /// </summary>
        public void ClearAllSupports()
        {
            foreach (Clade currentClade in GetAllBipartitions()) currentClade.Supports = null;
        }
    }
}
