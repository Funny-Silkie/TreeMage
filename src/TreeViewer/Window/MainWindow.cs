using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics.CodeAnalysis;
using TreeViewer.Core.Exporting;
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
                // File
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
                            Click = () => ViewModel.CreateNewCommand.Execute(),
                            Accelerator = "Ctrl+N",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "開く(&O)",
                            Click = () => ViewModel.OpenProjectCommand.Execute(),
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "保存(&S)",
                            Click = () => ViewModel.SaveProjectCommand.Execute(false),
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "名前を付けて保存(&A)",
                            Click = () => ViewModel.SaveProjectCommand.Execute(true),
                            Accelerator = "Ctrl+Shift+S",
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
                                    Label = "Tree(&T)",
                                    Submenu = [
                                        new MenuItem()
                                        {
                                            Type = MenuType.normal,
                                            Label = "Newick(&W)",
                                            Click = () => ExportAsTreeFile(TreeFormat.Newick).Wait(),
                                        },
                                        //new MenuItem()
                                        //{
                                        //    Type = MenuType.normal,
                                        //    Label = "Nexus(&X)",
                                        //    Click = () => Export(TreeFormat.).Wait(),
                                        //},
                                        //new MenuItem()
                                        //{
                                        //    Type = MenuType.normal,
                                        //    Label = "PhyloXML(&P)",
                                        //    Click = () => Export(TreeFormat.).Wait(),
                                        //},
                                    ],
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PNG(&B)",
                                    Click = () => ExportWithExporter(ExportType.Png).Wait(),
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "SVG(&S)",
                                    Click = () => ExportWithExporter(ExportType.Svg).Wait(),
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PDF(&P)",
                                    Click = () => ExportWithExporter(ExportType.Pdf).Wait(),
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

                // Edit
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

                // Tree
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "ツリー(&T)",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Focus all",
                            Click = () => FocusAll().Wait(),
                            Accelerator = "Ctrl+A",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Unfocus all",
                            Click = () => UnfocusAll().Wait(),
                            Accelerator = "Esc",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Reroot",
                            Click = () => Reroot().Wait(),
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Order by branch length",
                            Click = () => ViewModel.OrderByBranchLengthCommand.ExecuteAsync().Wait(),
                        },
                    ],
                },
#if DEBUG
                // Debug
                new MenuItem()
                {
                    Label = "デバッグ(&D)",
                    Submenu = [
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
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.reload,
                            Accelerator = "F5",
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.forcereload,
                            Accelerator = "Shift+F5",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.toggledevtools,
                            Accelerator = "F12",
                        },
                        new MenuItem()
                        {
                            Role = MenuRole.togglefullscreen,
                        },
                    ],
                },
#endif
            ]);
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

            await Electron.Dialog.ShowMessageBoxAsync(window, new MessageBoxOptions(message)
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
            string[] pathes = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions()
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
            string path = await Electron.Dialog.ShowSaveDialogAsync(window, new SaveDialogOptions()
            {
                Filters = filters,
            });

            if (path.Length == 0) return null;
            return path;
        }

        #region Menu Operations

        #region File

        /// <summary>
        /// インポート処理を行います。
        /// </summary>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        private async Task Import(TreeFormat format)
        {
            string? path = await ShowSingleFileOpenDialog();
            if (path is null) return;

            await ViewModel.ImportTree(path, format);
        }

        /// <summary>
        /// エクスポート処理を行います。
        /// </summary>
        /// <param name="format">出力する系統樹のフォーマット</param>
        private async Task ExportAsTreeFile(TreeFormat format)
        {
            string? path = await ShowFileSaveDialog();
            if (path is null) return;

            await ViewModel.ExportCurrentTreeAsTreeFile(path, format);
        }

        /// <summary>
        /// エクスポート処理を行います。
        /// </summary>
        /// <param name="exportType">出力するフォーマット</param>
        private async Task ExportWithExporter(ExportType exportType)
        {
            string? path = await ShowFileSaveDialog([new FileFilter()
            {
                Name = $"{exportType.ToString().ToUpper()} File",
                Extensions = [exportType.ToString().ToLower()],
            }]);
            if (path is null) return;

            await ViewModel.ExportWithExporterCommand.ExecuteAsync((path, exportType));
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        public void CloseWindow() => Window.Close();

        #endregion File

        #region Tree

        /// <summary>
        /// 全要素を選択します。
        /// </summary>
        private async Task FocusAll()
        {
            await ViewModel.FocusAllCommand.ExecuteAsync();
        }

        /// <summary>
        /// 選択を解除します。
        /// </summary>
        private async Task UnfocusAll()
        {
            await ViewModel.UnfocusAllCommand.ExecuteAsync();
        }

        /// <summary>
        /// リルートを行います。
        /// </summary>
        private async Task Reroot()
        {
            await ViewModel.RerootCommand.ExecuteAsync();
        }

        #endregion Tree

        #endregion Menu Operations
    }
}
