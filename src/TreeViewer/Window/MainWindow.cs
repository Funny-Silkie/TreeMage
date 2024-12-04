using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics.CodeAnalysis;

namespace TreeViewer.Window
{
    /// <summary>
    /// メインウィンドウを処理します。
    /// </summary>
    internal class MainWindow
    {
        private BrowserWindow? window;

        /// <summary>
        /// <see cref="MainWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainWindow()
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
        /// Electronのウィンドウを立ち上げます。
        /// </summary>
        public async Task CreateElectronWindow()
        {
            window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Show = false,
            });

            window.OnReadyToShow += window.Show;
            window.OnClosed += Electron.App.Quit;
        }

        /// <summary>
        /// メニューバーを初期化します。
        /// </summary>
        public void InitMenubar()
        {
            Electron.Menu.SetApplicationMenu([
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "ファイル",
                    Accelerator = "Alt+F",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "新規作成",
                            Accelerator = "Ctrl+N",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "開く",
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "保存",
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "名前を付けて保存",
                            Accelerator = "Ctrl+Shift+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "終了",
                            Accelerator = "Ctrl+W"
                        },
                    ],
                },
            ]);
        }
    }
}
