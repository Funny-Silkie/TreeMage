namespace TreeViewer.Core.Trees
{
    /// <summary>
    /// 系統樹を表します。
    /// </summary>
    public class Tree
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
        /// <exception cref="ArgumentException"><paramref name="root"/>がルートを表していない</exception>
        internal Tree(Clade root)
        {
            ArgumentNullException.ThrowIfNull(root);

            Root = root;
        }
    }
}
