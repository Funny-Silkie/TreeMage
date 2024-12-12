namespace TreeViewer.Core.Drawing.Styles
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

        /// <summary>
        /// 他のスタイルの内容を適用します。
        /// </summary>
        /// <param name="style">適用するインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="style"/>が<see langword="null"/></exception>
        public void ApplyValues(CladeStyle style)
        {
            ArgumentNullException.ThrowIfNull(style);

            BranchColor = style.BranchColor;
            LeafColor = style.LeafColor;
        }
    }
}
