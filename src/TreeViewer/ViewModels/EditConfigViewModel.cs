using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Core.Drawing;
using TreeViewer.Data;
using TreeViewer.Services;
using TreeViewer.Settings;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// コンフィグ編集画面のViewModelのクラスです。
    /// </summary>
    public class EditConfigViewModel : ViewModelBase
    {
        private readonly IElectronService electronService;
        private readonly Configurations config;

        /// <summary>
        /// 枝の色付け方法のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<BranchColoringType> BranchColoring { get; }

        /// <summary>
        /// 自動枝順ソートのモードのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<AutoOrderingMode> AutoOrderingMode { get; }

        /// <summary>
        /// ウィンドウを閉じるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> CloseCommand { get; }

        /// <summary>
        /// <see cref="EditConfigViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EditConfigViewModel([FromKeyedServices("config")] IElectronService electronService)
        {
            this.electronService = electronService;
            electronService.AttachViewModel(this);
            config = Configurations.LoadOrCreate();

            BranchColoring = new ReactivePropertySlim<BranchColoringType>(config.BranchColoring).WithSubscribe(x => config.BranchColoring = x)
                                                                                                .AddTo(Disposables);
            AutoOrderingMode = new ReactivePropertySlim<AutoOrderingMode>(config.AutoOrderingMode).WithSubscribe(x => config.AutoOrderingMode = x)
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

            electronService.Close();
        }
    }
}
