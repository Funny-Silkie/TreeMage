using Reactive.Bindings;

namespace TreeViewer
{
    /// <summary>
    /// コマンドの拡張を表します。
    /// </summary>
    internal static class CommandExtensions
    {
        /// <summary>
        /// デリゲートに変換します。
        /// </summary>
        /// <param name="command">対象のコマンド</param>
        /// <returns><paramref name="command"/>の実行を表すデリゲート</returns>
        public static Func<Task> ToDelegate(this AsyncReactiveCommand command) => command.ExecuteAsync;

        /// <summary>
        /// デリゲートに変換します。
        /// </summary>
        /// <typeparam name="T">コマンドの引数の型</typeparam>
        /// <param name="command">対象のコマンド</param>
        /// <returns><paramref name="command"/>の実行を表すデリゲート</returns>
        public static Func<T, Task> ToDelegate<T>(this AsyncReactiveCommand<T> command) => command.ExecuteAsync;

        /// <summary>
        /// デリゲートに変換します。
        /// </summary>
        /// <typeparam name="T">コマンドの引数の型</typeparam>
        /// <param name="command">対象のコマンド</param>
        /// <param name="arg">引数</param>
        /// <returns><paramref name="command"/>の実行を表すデリゲート</returns>
        public static Func<Task> ToDelegate<T>(this AsyncReactiveCommand<T> command, T arg) => () => command.ExecuteAsync(arg);
    }
}
