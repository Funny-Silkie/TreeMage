﻿using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using TreeMage.Core.Drawing;
using TreeMage.Core.Exporting;
using TreeMage.Data;
using TreeMage.Json;

namespace TreeMage.Settings
{
    /// <summary>
    /// 設定項目を表します。
    /// </summary>
    public class Configurations
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        /// <summary>
        /// 設定ファイルのパスを取得します。
        /// </summary>
        internal static string Location { get; }

        /// <summary>
        /// メインウィンドウの横幅を取得または設定します。
        /// </summary>
        public int MainWindowWidth { get; set; } = 960;

        /// <summary>
        /// メインウィンドウの高さを取得または設定します。
        /// </summary>
        public int MainWindowHeight { get; set; } = 720;

        /// <summary>
        /// 最大化されているかどうかを表す値を取得または設定します。
        /// </summary>
        public bool IsMaximized { get; set; }

        /// <summary>
        /// 枝の色付けの方式を取得または設定します。
        /// </summary>
        public BranchColoringType BranchColoring { get; set; } = BranchColoringType.Both;

        /// <summary>
        /// 自動枝順ソートのモードを取得または設定します。
        /// </summary>
        public AutoOrderingMode AutoOrderingMode { get; set; } = AutoOrderingMode.Descending;

        /// <summary>
        /// 使用するカルチャを取得または設定します。
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// <see cref="Configurations"/>の静的リソースを初期化します。
        /// </summary>
        static Configurations()
        {
            string? directory;
#if TEST
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) directory = Path.GetDirectoryName(Environment.ProcessPath);
            else directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

#else
            directory = Path.GetDirectoryName(Environment.ProcessPath)!;
#endif
            Debug.Assert(directory is not null);

            Location = Path.Combine(directory, "config.json");

            options.Converters.Add(new CultureInfoConverter());
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
        public static Configurations LoadOrCreate()
        {
            if (File.Exists(Location))
            {
                using var stream = new FileStream(Location, FileMode.Open);
                try
                {
                    Configurations? loaded = JsonSerializer.Deserialize<Configurations>(stream, options);
                    if (loaded is not null) return loaded;
                }
                catch (Exception e)
                {
#if !TEST
                    Console.Error.WriteLine(e);
#endif
                }
            }

            return new Configurations();
        }

        /// <summary>
        /// ファイルが存在する場合は読み込み，存在しない場合は新しいインスタンスを生成します。
        /// </summary>
        /// <returns>読み込まれたインスタンス</returns>
        /// <exception cref="InvalidOperationException">読み込みに失敗した</exception>
        public static async ValueTask<Configurations> LoadOrCreateAsync()
        {
            if (File.Exists(Location))
            {
                using var stream = new FileStream(Location, FileMode.Open);
                try
                {
                    Configurations? loaded = await JsonSerializer.DeserializeAsync<Configurations>(stream, options);
                    if (loaded is not null) return loaded;
                }
                catch (Exception e)
                {
#if !TEST
                    await Console.Error.WriteLineAsync(e.ToString());
#endif
                }
            }

            return new Configurations();
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public void Save()
        {
            using var stream = new FileStream(Location, FileMode.Create);
            JsonSerializer.Serialize(stream, this, options);
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public async Task SaveAsync()
        {
            using var stream = new FileStream(Location, FileMode.Create);
            await JsonSerializer.SerializeAsync(stream, this, options);
        }

        /// <summary>
        /// <see cref="ExportOptions"/>に変換します。
        /// </summary>
        /// <returns>変換後のオブジェクト</returns>
        public ExportOptions ToExportOptions()
        {
            var result = new ExportOptions();
            result.DrawingOptions.BranchColoring = BranchColoring;

            return result;
        }
    }
}
