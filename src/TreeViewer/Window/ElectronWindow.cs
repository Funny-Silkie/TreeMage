using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics.CodeAnalysis;
using TreeViewer.ViewModels;

namespace TreeViewer.Window
{
    /// <summary>
    /// Electronのウィンドウを扱う基底クラスです。
    /// </summary>
    /// <typeparam name="TViewModel">ViewModelのクラス</typeparam>
    public abstract class ElectronWindow<TViewModel>
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

        /// <summary>
        /// ViewModelを設定します。
        /// </summary>
        /// <param name="viewModel">設定するViewModelのインスタンス</param>
        public void SetViewModel(TViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        public void CloseWindow() => Window.Close();

        /// <summary>
        /// モーダルウィンドウとして自身にフォーカスさせます。
        /// </summary>
        public void ModalFocus()
        {
            Window.Focus();
            Electron.Shell.Beep();
        }

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public async Task ShowErrorMessageAsync(string? message)
        {
            await Electron.Dialog.ShowMessageBoxAsync(Window, new MessageBoxOptions(message)
            {
                Title = "Error",
                Type = MessageBoxType.error,
            });
        }

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="exception">例外</param>
        public async Task ShowErrorMessageAsync(Exception exception)
        {
            string message =
#if DEBUG
                exception.ToString();
#else
                exception.Message;
#endif

            await ShowErrorMessageAsync(message);
        }

        /// <summary>
        /// 単一のファイルを開くダイアログを開きます。
        /// </summary>
        /// <param name="filters">拡張子のフィルター</param>
        /// <returns>読み込むファイルパス，選択されなかった場合は<see langword="null"/></returns>
        public async Task<string?> ShowSingleFileOpenDialog(FileFilter[]? filters = null)
        {
            string[] pathes = await Electron.Dialog.ShowOpenDialogAsync(Window, new OpenDialogOptions()
            {
                Properties = [OpenDialogProperty.openFile, OpenDialogProperty.showHiddenFiles],
                Filters = filters,
            });
            if (pathes.Length != 1) return null;
            return pathes[0];
        }

        /// <summary>
        /// 単一のファイルを保存するダイアログを開きます。
        /// </summary>
        /// <param name="filters">拡張子のフィルター</param>
        /// <returns>出力するファイルパス，選択されなかった場合は<see langword="null"/></returns>
        public async Task<string?> ShowFileSaveDialog(FileFilter[]? filters = null)
        {
            string path = await Electron.Dialog.ShowSaveDialogAsync(Window, new SaveDialogOptions()
            {
                Filters = filters,
            });

            if (path.Length == 0) return null;
            return path;
        }
    }
}
