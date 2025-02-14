using System.Diagnostics;
using TreeMage.Services;

namespace TreeMage
{
    /// <summary>
    /// エラーハンドリングを行います。
    /// </summary>
    internal static class ErrorHandle
    {
        private static readonly string logDirectory;

        static ErrorHandle()
        {
            string? logDir = Path.GetDirectoryName(Environment.ProcessPath);
            Debug.Assert(logDir is not null);
            logDirectory = logDir;
        }

        /// <summary>
        /// ログファイルのパスを生成します。
        /// </summary>
        /// <returns></returns>
        private static string CreateLogFilePath() => Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss.ffff}.txt");

        /// <summary>
        /// エラーを主力します。
        /// </summary>
        /// <param name="exception">エラー</param>
        /// <param name="service">Electron用サービス</param>
        public static void OutputError(Exception? exception, IElectronService? service)
        {
            if (exception is null) return;

            Console.Error.WriteLine(exception.ToString());

            string logFilePath = CreateLogFilePath();
            Console.Out.WriteLine(logFilePath);

            try
            {
                File.WriteAllText(logFilePath, exception.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            service?.ShowErrorMessageAsync(exception);
        }

        /// <inheritdoc cref="OutputError(Exception?, IElectronService?)"/>
        public static async ValueTask OutputErrorAsync(Exception? exception, IElectronService? service)
        {
            if (exception is null) return;

            await Console.Error.WriteLineAsync(exception.ToString());

            string logFilePath = CreateLogFilePath();
            await Console.Out.WriteLineAsync(logFilePath);

            try
            {
                await File.WriteAllTextAsync(logFilePath, exception.ToString());
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }

            if (service is not null) await service.ShowErrorMessageAsync(exception);
        }
    }
}
