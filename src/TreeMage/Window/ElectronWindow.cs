using ElectronNET.API;
using System.Diagnostics.CodeAnalysis;
using TreeMage.ViewModels;

namespace TreeMage.Window
{
    /// <summary>
    /// Electronのウィンドウを扱う基底クラスです。
    /// </summary>
    /// <typeparam name="TViewModel">ViewModelのクラス</typeparam>
    public abstract class ElectronWindow<TViewModel> : IWindow
        where TViewModel : ViewModelBase
    {
        private BrowserWindow? window;
        private TViewModel? viewModel;

        /// <summary>
        /// 現在表示されているかどうかを表す値を取得します。
        /// </summary>
        public bool IsShown { get; private set; }

        /// <summary>
        /// 対象のElectronウィンドウを取得します。
        /// </summary>
        protected internal BrowserWindow Window
        {
            get
            {
                VerifyWindowState();
                return window;
            }
        }

        BrowserWindow IWindow.Window => Window;

        /// <summary>
        /// ウェブ画面のViewModelを取得します。
        /// </summary>
        protected TViewModel ViewModel
        {
            get
            {
                VerifyViewModelState();
                return viewModel;
            }
        }

        /// <summary>
        /// <see cref="ElectronWindow{TViewModel}"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ElectronWindow()
        {
        }

        /// <summary>
        /// <see cref="window"/>の状態をチェックします。
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="window"/>が初期化されていない</exception>
        [MemberNotNull(nameof(window))]
        private void VerifyWindowState()
        {
            if (window is null) throw new InvalidOperationException("ウィンドウが初期化されていません");
        }

        /// <summary>
        /// <see cref="viewModel"/>の状態をチェックします。
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="viewModel"/>が初期化されていない</exception>
        [MemberNotNull(nameof(viewModel))]
        private void VerifyViewModelState()
        {
            if (viewModel is null) throw new InvalidOperationException("ViewModelが初期化されていません");
        }

        /// <summary>
        /// ウィンドウの生成を行います。
        /// </summary>
        /// <returns>生成されたウィンドウ</returns>
        /// <remarks><see cref="CreateElectronWindow"/>で呼び出されます</remarks>
        protected abstract Task<BrowserWindow> CreateWindowInternal();

        /// <summary>
        /// URLを生成します。
        /// </summary>
        /// <param name="relative">相対パス</param>
        /// <returns>URL</returns>
        protected string CreateUrl(string relative)
        {
            string baseUrl = $"http://localhost:{BridgeSettings.WebPort}";
            if (relative.StartsWith('/')) return baseUrl + relative;
            return baseUrl + '/' + relative;
        }

        /// <summary>
        /// Electronのウィンドウを立ち上げます。
        /// </summary>
        public async Task CreateElectronWindow()
        {
            if (IsShown) return;
            window = await CreateWindowInternal();

            IsShown = true;
            window.OnClosed += () => IsShown = false;
        }

        void IWindow.AttachViewModel(ViewModelBase viewModel)
        {
            if (viewModel is not TViewModel vm) throw new ArgumentException("無効な型が渡されました", nameof(viewModel));
            this.viewModel = vm;
        }

        /// <inheritdoc/>
        public void Close() => Window.Close();

        /// <summary>
        /// モーダルウィンドウとして自身にフォーカスさせます。
        /// </summary>
        public void ModalFocus()
        {
            Window.Focus();
            Electron.Shell.Beep();
        }
    }
}
