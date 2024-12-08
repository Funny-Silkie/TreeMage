namespace TreeViewer.Core.Styles
{
    /// <summary>
    /// クレードのスタイルを表します。
    /// </summary>
    public class CladeStyle
    {
        /// <summary>
        /// 枝の色を取得または設定します。
        /// </summary>
        public string BranchColor { get; set; }

        /// <summary>
        /// 葉の文字色を取得または設定します。
        /// </summary>
        public string LeafColor { get; set; }

        /// <summary>
        /// <see cref="CladeStyle"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CladeStyle()
        {
            BranchColor = "black";
            LeafColor = "black";
        }
    }
}
