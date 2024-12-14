using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TreeViewer.Core.ProjectData.Json
{
    /// <summary>
    /// プロジェクトデータのルートオブジェクトを表します。
    /// </summary>
    public sealed class JsonRootData
    {
        /// <summary>
        /// データフォーマットのバージョンを取得または設定します。
        /// </summary>
        [JsonRequired]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// メインデータを取得または設定します。
        /// </summary>
        /// <remarks><see cref="JsonMainData"/>としてシリアライズ/デシリアライズする</remarks>
        [JsonRequired]
        public JsonNode Data { get; set; } = default!;
    }
}
