namespace TreeMage.Core.Drawing.Styles
{
    /// <summary>
    /// クレードのスタイルを表します。
    /// </summary>
    public class CladeStyle : ICloneable
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
        /// 折りたたまれているか否かを表す値を取得または設定します。
        /// </summary>
        public bool Collapsed { get; set; }

        /// <summary>
        /// クレード名を表す値を取得または設定します。
        /// </summary>
        public string? CladeLabel { get; set; }

        /// <summary>
        /// シェードの色を取得または設定します。
        /// </summary>
        public string? ShadeColor { get; set; }

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
            Collapsed = style.Collapsed;
            CladeLabel = style.CladeLabel;
            ShadeColor = style.ShadeColor;
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public CladeStyle Clone() => new CladeStyle()
        {
            BranchColor = BranchColor,
            LeafColor = LeafColor,
            Collapsed = Collapsed,
            CladeLabel = CladeLabel,
            ShadeColor = ShadeColor,
        };

        object ICloneable.Clone() => Clone();
    }
}
