using ElectronNET.API;
using ElectronNET.API.Entities;
using TreeViewer.ViewModels;
using TreeViewer.Window;

namespace TreeViewer.Services
{
    /// <summary>
    /// Electron関連のサービスの基底を表します。
    /// </summary>
    public interface IElectronService
    {
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
        public Task<string?> ShowSingleFileOpenDialogAsync(params IEnumerable<(string[] extensions, string name)> filters);

        /// <summary>
        /// 単一のファイルを保存するダイアログを開きます。
        /// </summary>
        /// <param name="filters">拡張子のフィルター</param>
        /// <returns>出力するファイルパス，選択されなかった場合は<see langword="null"/></returns>
        public Task<string?> ShowFileSaveDialogAsync(params IEnumerable<(string[] extensions, string name)> filters);
    }

    /// <summary>
    /// Electron関連のサービスの基底を表します。
    /// </summary>
    public class ElectronService : IElectronService
    {
        private readonly IWindow window;

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
                Title = "Error",
                Type = MessageBoxType.error,
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
