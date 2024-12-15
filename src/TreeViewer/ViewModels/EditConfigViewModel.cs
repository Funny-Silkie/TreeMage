using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Core.Drawing;
using TreeViewer.Settings;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// コンフィグ編集画面のViewModelのクラスです。
    /// </summary>
    public class EditConfigViewModel : WindowViewModel<EditConfigWindow, EditConfigViewModel>
    {
        private readonly Configurations config;

        /// <summary>
        /// 枝の色付け方法のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<BranchColoringType> BranchColoring { get; }

        /// <summary>
        /// ウィンドウを閉じるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> CloseCommand { get; }

        /// <summary>
        /// <see cref="EditConfigViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EditConfigViewModel() : base(EditConfigWindow.Instance)
        {
            config = Configurations.LoadOrCreate();

            BranchColoring = new ReactivePropertySlim<BranchColoringType>(config.BranchColoring).WithSubscribe(x => config.BranchColoring = x)
                                                                                                .AddTo(Disposables);

            CloseCommand = new AsyncReactiveCommand<bool>().WithSubscribe(Close)
                                                           .AddTo(Disposables);
        }

        /// <summary>
        /// 編集画面を終了します。
        /// </summary>
        /// <param name="save">編集内容を保存するかどうかを表す値</param>
        private async Task Close(bool save)
        {
            if (save) await config.SaveAsync();

            Window.CloseWindow();
        }
    }
}
