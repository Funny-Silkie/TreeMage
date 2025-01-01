using Reactive.Bindings;

namespace TreeViewer
{
    /// <summary>
    /// ReactiveProperty関連の拡張を表します。
    /// </summary>
    internal static class ReactiveExtensions
    {
        /// <summary>
        /// 値変更時のコールバックを登録します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型</typeparam>
        /// <param name="property">対象のプロパティ</param>
        /// <param name="onNext">登録するコールバック</param>
        /// <returns><paramref name="property"/></returns>
        public static ReactivePropertySlim<T> WithSubscribe<T>(this ReactivePropertySlim<T> property, Action<T> onNext)
        {
            property.Subscribe(onNext);
            return property;
        }

        /// <summary>
        /// 値変更時のコールバックを登録します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型</typeparam>
        /// <param name="property">対象のプロパティ</param>
        /// <param name="onNext">登録するコールバック</param>
        /// <returns><paramref name="property"/></returns>
        public static ReadOnlyReactivePropertySlim<T> WithSubscribe<T>(this ReadOnlyReactivePropertySlim<T> property, Action<T> onNext)
        {
            property.Subscribe(onNext);
            return property;
        }

        /// <summary>
        /// 処理を登録します。
        /// </summary>
        /// <param name="command">対象のコマンド</param>
        /// <param name="process">登録する同期処理</param>
        /// <returns><paramref name="command"/></returns>
        public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand command, Action process)
        {
            command.Subscribe(async () =>
            {
                process.Invoke();
                await Task.CompletedTask;
            });
            return command;
        }

        /// <summary>
        /// 処理を登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="command">対象のコマンド</param>
        /// <param name="process">登録する同期処理</param>
        /// <returns><paramref name="command"/></returns>
        public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> command, Action<T> process)
        {
            command.Subscribe(async x =>
            {
                process.Invoke(x);
                await Task.CompletedTask;
            });
            return command;
        }
    }
}
