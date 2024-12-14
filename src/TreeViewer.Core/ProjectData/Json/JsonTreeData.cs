using System.Text.Json.Serialization;
using TreeViewer.Core.Drawing.Styles;

namespace TreeViewer.Core.ProjectData.Json
{
    /// <summary>
    /// 系統樹のデータを表します。
    /// </summary>
    public sealed class JsonTreeData
    {
        /// <summary>
        /// ツリーを表すNEWICK文字列を取得または設定します。
        /// </summary>
        [JsonRequired]
        public string TreeText { get; set; } = string.Empty;

        /// <summary>
        /// 系統樹の描画スタイルを取得または設定します。
        /// </summary>
        [JsonRequired]
        public TreeStyle TreeStyle { get; set; } = default!;

        /// <summary>
        /// クレードの描画スタイル一覧を取得または設定します。
        /// </summary>
        [JsonRequired]
        public CladeStyle[] CladeStyles { get; set; } = [];
    }
}
