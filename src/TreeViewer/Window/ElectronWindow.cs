﻿using ElectronNET.API;
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
        /// 対象のElectronウィンドウを取得します。
        /// </summary>
        protected BrowserWindow Window
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
        /// Electronのウィンドウを立ち上げます。
        /// </summary>
        public async Task CreateElectronWindow()
        {
            window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Width = 960,
                Height = 720,
                Show = false,
            });
            window.OnReadyToShow += window.Show;
            window.OnClosed += Electron.App.Quit;
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
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="exception">例外</param>
        public async Task ShowErrorMessage(Exception exception)
        {
            string message =
#if DEBUG
                exception.ToString();
#else
                exception.Message;
#endif

            await Electron.Dialog.ShowMessageBoxAsync(Window, new MessageBoxOptions(message)
            {
                Title = "Error",
                Type = MessageBoxType.error,
            });
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
