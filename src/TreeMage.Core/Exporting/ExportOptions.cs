using TreeMage.Core.Drawing;

namespace TreeMage.Core.Exporting
{
    /// <summary>
    /// エクスポートのオプションを表します。
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// 描画用オプションを取得または設定します。
        /// </summary>
        public DrawingOptions DrawingOptions { get; set; }

        /// <summary>
        /// <see cref="ExportOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ExportOptions()
        {
            DrawingOptions = new DrawingOptions();
        }
    }
}
