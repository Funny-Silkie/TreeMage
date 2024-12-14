using System.Text.Json.Serialization;

namespace TreeViewer.Core.ProjectData.Json
{
    /// <summary>
    /// 保存されるメインデータを表します。
    /// </summary>
    public sealed class JsonMainData
    {
        /// <summary>
        /// 系統樹のデータ一覧を取得または設定します。
        /// </summary>
        [JsonRequired]
        public JsonTreeData[] Trees { get; set; } = [];
    }
}
