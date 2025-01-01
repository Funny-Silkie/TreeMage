using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.ViewModels;

namespace TreeViewer.Window
{
    /// <summary>
    /// メインウィンドウを処理します。
    /// </summary>
    public class MainWindow : ElectronWindow<HomeViewModel>
    {
        /// <summary>
        /// インスタンスを取得します。
        /// </summary>
        public static MainWindow Instance { get; } = new MainWindow();

        /// <summary>
        /// <see cref="MainWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        private MainWindow()
        {
        }

        /// <inheritdoc/>
        protected override async Task<BrowserWindow> CreateWindowInternal()
        {
            BrowserWindow result = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Width = 960,
                Height = 720,
                Show = false,
                Title = "TreeViewer",
            });
            await result.WebContents.Session.ClearCacheAsync();
            result.OnReadyToShow += result.Show;
            result.OnClosed += Electron.App.Quit;
            return result;
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
                    Label = "&File",
                    Accelerator = "Alt+F",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "New(&N)",
                            Click = () => ViewModel.CreateNewCommand.Execute(),
                            Accelerator = "Ctrl+N",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Open(&O)",
                            Click = () => ViewModel.OpenProjectCommand.Execute(),
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Save(&S)",
                            Click = () => ViewModel.SaveProjectCommand.Execute(false),
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Save As(&A)",
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
                            Label = "Import(&I)",
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
                            Label = "Export(&E)",
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
                            Label = "Exit(&X)",
                            Click = CloseWindow,
                            Accelerator = "Ctrl+W",
                        },
                    ],
                },

                // Edit
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "&Edit",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Undo(&U)",
                            Click = () => ViewModel.UndoCommand.Execute(),
                            Accelerator = "Ctrl+Z",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Redo(&R)",
                            Click = () => ViewModel.RedoCommand.Execute(),
                            Accelerator = "Ctrl+Shift+Z",
                        },
                    ],
                },

                // Tree
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "&Tree",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Focus all",
                            Click = () => ViewModel.FocusAllCommand.Execute(),
                            Accelerator = "Ctrl+A",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Unfocus all",
                            Click = () => ViewModel.UnfocusAllCommand.Execute(),
                            Accelerator = "Esc",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.separator,
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Order by branch length",
                            Submenu = [
                                new MenuItem(){
                                    Type = MenuType.normal,
                                    Label = "Ascending",
                                    Click = () => ViewModel.OrderByBranchLengthCommand.Execute(false),
                                },
                                new MenuItem(){
                                    Type = MenuType.normal,
                                    Label = "Descending",
                                    Click = () => ViewModel.OrderByBranchLengthCommand.Execute(true),
                                },
                            ],
                        },
                    ],
                },

                // Window
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "&Window",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Edit configuration(&C)",
                            Click = () => ShowConfigWindow().Wait(),
                        },
                    ],
                },

                // Help
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "&Help",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Show Help(&H)",
                            Click = ShowHelp,
                            Accelerator = "F1",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Version Information(&A)",
                            Click = () => ShowVersionWindow().Wait(),
                        },
                    ],
                },
#if DEBUG
                // Debug
                new MenuItem()
                {
                    Label = "&Debug",
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

            await ViewModel.ExportTreeCommand.ExecuteAsync((path, format));
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

        #endregion File

        #region Window

        /// <summary>
        /// コンフィグ用ウィンドウを開きます。
        /// </summary>
        private async Task ShowConfigWindow()
        {
            EditConfigWindow child = EditConfigWindow.Instance;

            await child.CreateElectronWindow();
            child.OnClosed += ViewModel.RerenderTreeCommand.Execute;
        }

        #endregion Window

        #region Help

        /// <summary>
        /// ヘルプの表示を行います。
        /// </summary>
        private void ShowHelp()
        {
            var info = new ProcessStartInfo()
            {
                FileName = "https://github.com/Funny-Silkie/TreeViewer/blob/master/README.md",
                UseShellExecute = true,
            };

            Process.Start(info);
        }

        /// <summary>
        /// バージョンウィンドウを開きます。
        /// </summary>
        private static async Task ShowVersionWindow()
        {
            VersionWindow child = VersionWindow.Instance;

            await child.CreateElectronWindow();
        }

        #endregion Help

        #endregion Menu Operations
    }
}
