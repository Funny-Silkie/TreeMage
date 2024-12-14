﻿using System.Text.Json;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Exporting;

namespace TreeViewer.Settings
{
    /// <summary>
    /// 設定項目を表します。
    /// </summary>
    public class Configurations
    {
        /// <summary>
        /// 設定ファイルのパスを取得します。
        /// </summary>
        internal static string Location { get; }

        /// <summary>
        /// インスタンスを取得します。
        /// </summary>
        public static Configurations Instance => _instance ??= LoadOrCreate();

        private static Configurations? _instance;

        /// <summary>
        /// 枝の色付けの方式を取得または設定します。
        /// </summary>
        public BranchColoringType BranchColoring { get; set; } = BranchColoringType.Both;

        /// <summary>
        /// <see cref="Configurations"/>の静的リソースを初期化します。
        /// </summary>
        static Configurations()
        {
            Location = Path.Combine(Environment.ProcessPath!, "config.json");
        }

        /// <summary>
        /// <see cref="Configurations"/>の新しいインスタンスを初期化します。
        /// </summary>
        public Configurations()
        {
        }

        /// <summary>
        /// ファイルが存在する場合は読み込み，存在しない場合は新しいインスタンスを生成します。
        /// </summary>
        /// <returns>読み込まれたインスタンス</returns>
        /// <exception cref="InvalidOperationException">読み込みに失敗した</exception>
        internal static Configurations LoadOrCreate()
        {
            if (File.Exists(Location))
            {
                using var stream = new FileStream(Location, FileMode.Open);
                return JsonSerializer.Deserialize<Configurations>(stream) ?? throw new InvalidOperationException("設定の読み込みに失敗しました");
            }

            return new Configurations();
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public void Save()
        {
            using var stream = new FileStream(Location, FileMode.Create);
            JsonSerializer.Serialize(stream, this);
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public async Task SaveAsync()
        {
            using var stream = new FileStream(Location, FileMode.Create);
            await JsonSerializer.SerializeAsync(stream, this);
        }

        /// <summary>
        /// <see cref="ExportOptions"/>に変換します。
        /// </summary>
        /// <returns>変換後のオブジェクト</returns>
        public ExportOptions ToExportOptions()
        {
            return new ExportOptions()
            {
                BranchColoring = BranchColoring,
            };
        }
    }
}
