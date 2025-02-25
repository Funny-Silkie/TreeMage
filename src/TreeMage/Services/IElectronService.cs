using ElectronNET.API;
using ElectronNET.API.Entities;
using TreeMage.Models;
using TreeMage.Resources;
using TreeMage.ViewModels;
using TreeMage.Window;

namespace TreeMage.Services
{
    /// <summary>
    /// Electron関連のサービスの基底を表します。
    /// </summary>
    public interface IElectronService
    {
        /// <summary>
        /// ウィンドウのタイトルを設定します。
        /// </summary>
        /// <param name="value">タイトル</param>
        string Title { set; }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        void Close();

        /// <summary>
        /// ViewModelをアタッチします。
        /// </summary>
        /// <param name="viewModel">アタッチするViewModel</param>
        /// <exception cref="ArgumentException"><paramref name="viewModel"/>の型が無効</exception>
        void AttachViewModel(ViewModelBase viewModel);

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        Task ShowErrorMessageAsync(string? message);

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="exception">例外</param>
        async Task ShowErrorMessageAsync(Exception exception)
        {
            if (exception is ModelException) await ShowErrorMessageAsync(exception.Message);
            else await ShowErrorMessageAsync(exception.ToString());
        }

        /// <summary>
        /// 確認用ダイアログを表示します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttons">ボタン一覧</param>
        /// <returns>OKが選択されたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        Task<bool> ShowVerifyDialogAsync(string message, string? title = null, string[]? buttons = null);

        /// <summary>
        /// 警告ダイアログを表示します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        Task ShowWarningAsync(string message, string? title = null);

        /// <summary>
        /// 単一のファイルを開くダイアログを開きます。
        /// </summary>
        /// <param name="filters">拡張子のフィルター</param>
        /// <returns>読み込むファイルパス，選択されなかった場合は<see langword="null"/></returns>
        Task<string?> ShowSingleFileOpenDialogAsync(params IEnumerable<(string[] extensions, string name)> filters);

        /// <summary>
        /// 単一のファイルを保存するダイアログを開きます。
        /// </summary>
        /// <param name="filters">拡張子のフィルター</param>
        /// <returns>出力するファイルパス，選択されなかった場合は<see langword="null"/></returns>
        Task<string?> ShowFileSaveDialogAsync(params IEnumerable<(string[] extensions, string name)> filters);
    }

    /// <summary>
    /// Electron関連のサービスの基底を表します。
    /// </summary>
    public class ElectronService : IElectronService
    {
        private readonly IWindow window;

        /// <inheritdoc/>
        public string Title
        {
            set => window.Window.SetTitle(value);
        }

        /// <summary>
        /// <see cref="ElectronService"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="window">対象のウィンドウ</param>
        public ElectronService(IWindow window)
        {
            this.window = window;
        }

        /// <inheritdoc/>
        public void AttachViewModel(ViewModelBase viewModel) => window.AttachViewModel(viewModel);

        /// <inheritdoc/>
        public void Close() => window.Close();

        /// <inheritdoc/>
        public async Task ShowErrorMessageAsync(string? message)
        {
            await Electron.Dialog.ShowMessageBoxAsync(window.Window, new MessageBoxOptions(message)
            {
                Title = SR.DIALOG_TITLE_ERROR,
                Type = MessageBoxType.error,
            });
        }

        /// <inheritdoc/>
        public async Task<bool> ShowVerifyDialogAsync(string message, string? title, string[]? buttons)
        {
            MessageBoxResult result = await Electron.Dialog.ShowMessageBoxAsync(window.Window, new MessageBoxOptions(message)
            {
                Message = message,
                Buttons = buttons ?? [SR.DIALOG_BUTTON_OK, SR.DIALOG_BUTTON_CANCEL],
                Title = title ?? SR.DIALOG_TITLE_VERIFYING,
                NoLink = true,
            });

            return result.Response == 0;
        }

        /// <inheritdoc/>
        public async Task ShowWarningAsync(string message, string? title)
        {
            await Electron.Dialog.ShowMessageBoxAsync(window.Window, new MessageBoxOptions(message)
            {
                Title = title ?? SR.DIALOG_TITLE_WARNING,
                Buttons = [SR.DIALOG_BUTTON_OK],
                NoLink = true,
            });
        }

        /// <inheritdoc/>
        public async Task<string?> ShowSingleFileOpenDialogAsync(IEnumerable<(string[] extensions, string name)> filters)
        {
            string[] pathes = await Electron.Dialog.ShowOpenDialogAsync(window.Window, new OpenDialogOptions()
            {
                Properties = [OpenDialogProperty.openFile, OpenDialogProperty.showHiddenFiles],
                Filters = filters.Select(x => new FileFilter()
                {
                    Extensions = x.extensions,
                    Name = x.name,
                }).ToArray(),
            });
            if (pathes.Length != 1) return null;
            return pathes[0];
        }

        /// <inheritdoc/>
        public async Task<string?> ShowFileSaveDialogAsync(IEnumerable<(string[] extensions, string name)> filters)
        {
            string path = await Electron.Dialog.ShowSaveDialogAsync(window.Window, new SaveDialogOptions()
            {
                Filters = filters.Select(x => new FileFilter()
                {
                    Extensions = x.extensions,
                    Name = x.name,
                }).ToArray(),
            });

            if (path.Length == 0) return null;
            return path;
        }
    }
}
