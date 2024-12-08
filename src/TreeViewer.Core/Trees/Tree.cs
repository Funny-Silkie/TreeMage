using TreeViewer.Core.Trees.Parsers;

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

        /// <summary>
        /// 対象の<see cref="TextReader"/>から系統樹を読み込みます。
        /// </summary>
        /// <param name="reader">使用する<see cref="TextReader"/>のインスタンス</param>
        /// <param name="format">系統樹ファイルのフォーマット</param>
        /// <returns>読み込まれた系統樹一覧</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/>が無効</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>が<see langword="null"/></exception>
        /// <exception cref="TreeFormatException">系統樹のフォーマットが無効</exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="reader"/>が既に破棄されている</exception>
        public static Task<Tree[]> ReadAsync(TextReader reader, TreeFormat format)
        {
            ITreeParser parser = ITreeParser.CreateFromTargetFormat(format);
            return parser.ReadAsync(reader);
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

        /// <summary>
        /// <see cref="TextWriter"/>を用いて系統樹を出力します。
        /// </summary>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="format">系統樹ファイルのフォーマット</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/>が無効</exception>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>が<see langword="null"/></exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public Task WriteAsync(TextWriter writer, TreeFormat format)
        {
            ITreeParser parser = ITreeParser.CreateFromTargetFormat(format);
            return parser.WriteAsync(writer, this);
        }
    }
}
