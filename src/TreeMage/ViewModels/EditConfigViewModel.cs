using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Globalization;
using TreeMage.Core.Drawing;
using TreeMage.Data;
using TreeMage.Resources;
using TreeMage.Services;
using TreeMage.Settings;

namespace TreeMage.ViewModels
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
        /// 使用言語のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<CultureInfo> Language { get; }

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
            Language = new ReactivePropertySlim<CultureInfo>(config.Culture, ReactivePropertyMode.Default & ~ReactivePropertyMode.RaiseLatestValueOnSubscribe).WithSubscribe(x =>
            {
                config.Culture = x;
                this.electronService.ShowWarningAsync(SR.WARNING_LANGUAGE_APPLYING);
            }).AddTo(Disposables);

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
