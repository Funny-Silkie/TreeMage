using System.IO.Compression;
using System.Text.Json;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.ProjectData.Json;
using TreeMage.Core.Trees;
using TreeMage.Core.Trees.Parsers;

namespace TreeMage.Core.ProjectData
{
    /// <summary>
    /// プロジェクトデータを表します。
    /// </summary>
    public class ProjectData
    {
        private const string DataFormatVersion = "1";

        private static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
#if DEBUG
            WriteIndented = true,
#endif
            IgnoreReadOnlyProperties = true,
        };

        /// <summary>
        /// ツリー一覧を取得または設定します。
        /// </summary>
        public Tree[] Trees { get; set; }

        /// <summary>
        /// <see cref="ProjectData"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ProjectData()
        {
            Trees = [];
        }

        /// <summary>
        /// <see cref="ProjectData"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="json">JSONデータ</param>
        /// <exception cref="InvalidOperationException">JSONデータが無効</exception>
        private ProjectData(JsonRootData json)
        {
            JsonMainData? mainData = JsonSerializer.Deserialize<JsonMainData>(json.Data) ?? throw new InvalidOperationException("データフォーマットが無効です");
            Trees = Array.ConvertAll(mainData.Trees, x =>
            {
                Tree tree = Tree.ReadAsync(new StringReader(x.TreeText), TreeFormat.Newick).Result[0];
                tree.Style.ApplyValues(x.TreeStyle);
                foreach ((Clade clade, CladeStyle index) in tree.GetAllClades().Zip(x.CladeStyles)) clade.Style.ApplyValues(index);
                return tree;
            });
        }

        /// <summary>
        /// データを読み込みます。
        /// </summary>
        /// <param name="path">読み込むデータのパス</param>
        /// <returns><paramref name="path"/>から読み込まれた<see cref="ProjectData"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="InvalidOperationException">データフォーマットが無効</exception>
        /// <exception cref="NotSupportedException">データフォーマットのバージョンが無効</exception>
        public static ProjectData Load(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            return Load(stream);
        }

        /// <summary>
        /// データを読み込みます。
        /// </summary>
        /// <param name="stream">読み込むデータのストリームオブジェクト</param>
        /// <returns><paramref name="stream"/>から読み込まれた<see cref="ProjectData"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">データフォーマットが無効</exception>
        /// <exception cref="NotSupportedException">データフォーマットのバージョンが無効</exception>
        public static ProjectData Load(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true);

            JsonRootData? rootData = JsonSerializer.Deserialize<JsonRootData>(gzipStream, options) ?? throw new InvalidOperationException("データのフォーマットが無効です");
            if (rootData.Version != DataFormatVersion) throw new NotSupportedException("サポートされていないデータのバージョンです。https://github.com/Funny-Silkie/TreeMage/blob/master/README.md を確認ください");

            return new ProjectData(rootData);
        }

        /// <summary>
        /// データを読み込みます。
        /// </summary>
        /// <param name="path">読み込むデータのパス</param>
        /// <returns><paramref name="path"/>から読み込まれた<see cref="ProjectData"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="InvalidOperationException">データフォーマットが無効</exception>
        /// <exception cref="NotSupportedException">データフォーマットのバージョンが無効</exception>
        public static async Task<ProjectData> LoadAsync(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            return await LoadAsync(stream);
        }

        /// <summary>
        /// データを読み込みます。
        /// </summary>
        /// <param name="stream">読み込むデータのストリームオブジェクト</param>
        /// <returns><paramref name="stream"/>から読み込まれた<see cref="ProjectData"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">データフォーマットが無効</exception>
        /// <exception cref="NotSupportedException">データフォーマットのバージョンが無効</exception>
        public static async Task<ProjectData> LoadAsync(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true);

            JsonRootData? rootData = await JsonSerializer.DeserializeAsync<JsonRootData>(gzipStream, options) ?? throw new InvalidOperationException("データのフォーマットが無効です");
            if (rootData.Version != DataFormatVersion) throw new NotSupportedException("サポートされていないデータのバージョンです。https://github.com/Funny-Silkie/TreeMage/blob/master/README.md を確認ください");

            return new ProjectData(rootData);
        }

        /// <summary>
        /// JSON用データへと変換します。
        /// </summary>
        /// <returns>JSONデータ</returns>
        /// <exception cref="InvalidOperationException">ツリーデータの変換に失敗した</exception>
        internal JsonRootData ToJson()
        {
            var mainData = new JsonMainData()
            {
                Trees = Trees.Select(x => new JsonTreeData()
                {
                    TreeText = x.ToString(),
                    TreeStyle = x.Style,
                    CladeStyles = x.GetAllClades().Select(x => x.Style).ToArray(),
                }).ToArray(),
            };
            var result = new JsonRootData()
            {
                Version = DataFormatVersion,
                Data = JsonSerializer.SerializeToNode(mainData, options) ?? throw new InvalidOperationException("ツリーデータのシリアライズに失敗しました"),
            };

            return result;
        }

        /// <summary>
        /// データの保存を行います。
        /// </summary>
        /// <param name="path">保存先のストリームオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        public void Save(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            Save(stream);
        }

        /// <summary>
        /// データの保存を行います。
        /// </summary>
        /// <param name="stream">保存先のストリームオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        public void Save(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var gzipStream = new GZipStream(stream, CompressionMode.Compress, true);

            JsonSerializer.Serialize(gzipStream, ToJson(), options);
        }

        /// <summary>
        /// データの保存を行います。
        /// </summary>
        /// <param name="path">保存先のストリームオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        public async Task SaveAsync(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            await SaveAsync(stream);
        }

        /// <summary>
        /// データの保存を行います。
        /// </summary>
        /// <param name="stream">保存先のストリームオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>が<see langword="null"/></exception>
        public async Task SaveAsync(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var gzipStream = new GZipStream(stream, CompressionMode.Compress, true);

            await JsonSerializer.SerializeAsync(gzipStream, ToJson(), options);
        }
    }
}
