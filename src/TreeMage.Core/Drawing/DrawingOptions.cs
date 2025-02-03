namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// 描画のオプションを表します。
    /// </summary>
    public class DrawingOptions
    {
        /// <summary>
        /// 枝の色付けの方式を取得または設定します。
        /// </summary>
        public BranchColoringType BranchColoring { get; set; } = BranchColoringType.Both;

        /// <summary>
        /// <see cref="DrawingOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DrawingOptions()
        {
        }
    }
}
