using TreeMage.Core.Drawing;

namespace TreeMage.Core.Exporting
{
    /// <summary>
    /// エクスポートのオプションを表します。
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// 枝の色付けの方式を取得または設定します。
        /// </summary>
        public BranchColoringType BranchColoring { get; set; } = BranchColoringType.Both;

        /// <summary>
        /// <see cref="ExportOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ExportOptions()
        {
        }
    }
}
