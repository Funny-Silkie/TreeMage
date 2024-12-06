using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics.CodeAnalysis;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.ViewModels;

namespace TreeViewer.Window
{
    /// <summary>
    /// メインウィンドウを処理します。
    /// </summary>
    internal class MainWindow
    {
        private HomeViewModel? viewModel;
        private BrowserWindow? window;

        /// <summary>
        /// 対象のElectronウィンドウを取得します。
        /// </summary>
        private BrowserWindow Window
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
        private HomeViewModel ViewModel
        {
            get
            {
                VerifyViewModelState();
                return viewModel;
            }
        }

        /// <summary>
        /// インスタンスを取得します。
        /// </summary>
        public static MainWindow Instance { get; private set; } = new MainWindow();

        /// <summary>
        /// <see cref="MainWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        private MainWindow()
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
        public void SetViewModel(HomeViewModel viewModel)
        {
            this.viewModel = viewModel;
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
                    Label = "ファイル(&F)",
                    Accelerator = "Alt+F",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "新規作成(&N)",
                            Accelerator = "Ctrl+N",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "開く(&O)",
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "保存(&S)",
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "名前を付けて保存(&A)",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "インポート(&I)",
                            Submenu = [
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "Newick(&W)",
                                    Click = () => Import(TreeFormat.Newick).Wait(),
                                },
                                //new MenuItem()
                                //{
                                //    Type = MenuType.normal,
                                //    Label = "Nexus(&X)",
                                //    Click = () => Import(TreeFormat.).Wait(),
                                //},
                                //new MenuItem()
                                //{
                                //    Type = MenuType.normal,
                                //    Label = "PhyloXML(&P)",
                                //    Click = () => Import(TreeFormat.).Wait(),
                                //},
                            ],
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "エクスポート(&E)",
                            Submenu = [
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PNG",
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "SVG",
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PDF",
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "Adobe Illustrator",
                                },
                            ],
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "終了(&X)",
                            Click = CloseWindow,
                            Accelerator = "Ctrl+W",
                        },
                    ],
                },
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "編集(&E)",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Undo(&U)",
                            Accelerator = "Ctrl+Z",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Redo(&R)",
                            Accelerator = "Ctrl+Shift+Z",
                        },
                    ],
                },
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "ツリー(&T)",
                    Submenu = [
                    ],
                },
#if DEBUG
                new MenuItem()
                {
                    Label = "デバッグ(&D)",
                    Submenu = [
                        new MenuItem()
                        {
                            Role = MenuRole.toggledevtools,
                            Accelerator = "F12",
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.togglefullscreen,
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.zoom,
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.zoomin,
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.zoomout,
                        },
                    ],
                },
#endif
            ]);
        }

        /// <summary>
        /// インポート処理を行います。
        /// </summary>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        public async Task Import(TreeFormat format)
        {
            string[] pathes = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions()
            {
                Properties = [OpenDialogProperty.openFile, OpenDialogProperty.showHiddenFiles],
            });
            if (pathes.Length != 1) return;

            await ViewModel.ImportTree(pathes[0], format);
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        public void CloseWindow() => Window.Close();
    }
}
