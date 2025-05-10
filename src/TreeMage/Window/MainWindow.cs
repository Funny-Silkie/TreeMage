using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics;
using TreeMage.Core.Exporting;
using TreeMage.Core.Trees.Parsers;
using TreeMage.Resources;
using TreeMage.Settings;
using TreeMage.ViewModels;

namespace TreeMage.Window
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
        /// ウィンドウサイズを取得します。
        /// </summary>
        public (int width, int height) Size { get; private set; }

        /// <summary>
        /// 最大化されているかどうかを表す値を取得します。
        /// </summary>
        public bool IsMaximized { get; private set; }

        /// <summary>
        /// <see cref="MainWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        private MainWindow()
        {
        }

        /// <inheritdoc/>
        protected override async Task<BrowserWindow> CreateWindowInternal()
        {
            Configurations config = await Configurations.LoadOrCreateAsync();
            Size = (config.MainWindowWidth, config.MainWindowHeight);

            BrowserWindow result = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Width = config.MainWindowWidth,
                Height = config.MainWindowHeight,
                Show = false,
                Title = "TreeMage",
            });
            await result.WebContents.Session.ClearCacheAsync();

            bool reloaded = false;
            result.OnReadyToShow += () =>
            {
                result.Show();
                if (!reloaded)
                {
                    result.LoadURL(CreateUrl("/"));
                    reloaded = true;
                }
                if (config.IsMaximized) result.Maximize();
            };
            result.OnResize += () => Task.Run(async () =>
            {
                if (await Window.IsMaximizedAsync()) return;
                int[] size = await Window.GetSizeAsync();
                Size = (size[0], size[1]);
            }).Wait();
            result.OnMaximize += () => IsMaximized = true;
            result.OnUnmaximize += () => IsMaximized = false;
            result.OnClose += () =>
            {
                Configurations config = Configurations.LoadOrCreate();
                config.MainWindowWidth = Size.width;
                config.MainWindowHeight = Size.height;
                config.IsMaximized = IsMaximized;
                config.Save();
            };
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
                    Label = SR.MENU_FILE,
                    Accelerator = "Alt+F",
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_NEW,
                            Click = () => ViewModel.CreateNewCommand.Execute(),
                            Accelerator = "Ctrl+N",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_OPEN,
                            Click = () => ViewModel.OpenProjectCommand.Execute(),
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_SAVE,
                            Click = () => ViewModel.SaveProjectCommand.Execute(false),
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_SAVE_AS,
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
                            Label = SR.MENU_IMPORT_TREE,
                            Submenu = [
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "Newick(&W)",
                                    Click = () => ViewModel.ImportTreeCommand.Execute(TreeFormat.Newick),
                                },
                                //new MenuItem()
                                //{
                                //    Type = MenuType.normal,
                                //    Label = "Nexus(&X)",
                                //    Click = () => ViewModel.ImportTreeCommand.Execute(TreeFormat.),
                                //},
                                //new MenuItem()
                                //{
                                //    Type = MenuType.normal,
                                //    Label = "PhyloXML(&P)",
                                //    Click = () => ViewModel.ImportTreeCommand.Execute(TreeFormat.),
                                //},
                            ],
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_EXPORT,
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
                                            Click = () => ViewModel.ExportTreeCommand.Execute(TreeFormat.Newick),
                                        },
                                        //new MenuItem()
                                        //{
                                        //    Type = MenuType.normal,
                                        //    Label = "Nexus(&X)",
                                        //    Click = () => ViewModel.ExportTreeCommand.Execute(TreeFormat.),
                                        //},
                                        //new MenuItem()
                                        //{
                                        //    Type = MenuType.normal,
                                        //    Label = "PhyloXML(&P)",
                                        //    Click = () => ViewModel.ExportTreeCommand.Execute(TreeFormat.),
                                        //},
                                    ],
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PNG(&B)",
                                    Click = () => ViewModel.ExportWithExporterCommand.Execute(ExportType.Png),
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "SVG(&S)",
                                    Click = () => ViewModel.ExportWithExporterCommand.Execute(ExportType.Svg),
                                },
                                new MenuItem()
                                {
                                    Type = MenuType.normal,
                                    Label = "PDF(&P)",
                                    Click = () => ViewModel.ExportWithExporterCommand.Execute(ExportType.Pdf),
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
                            Label = SR.MENU_EXIT,
                            Click = Close,
                            Accelerator = "Ctrl+W",
                        },
                    ],
                },

                // Edit
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = SR.MENU_EDIT,
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_UNDO,
                            Click = () => ViewModel.UndoCommand.Execute(),
                            Accelerator = "Ctrl+Z",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_REDO,
                            Click = () => ViewModel.RedoCommand.Execute(),
                            Accelerator = "Ctrl+Shift+Z",
                        },
                    ],
                },

                // Tree
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = SR.MENU_TREE,
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_FOCUS_ALL,
                            Click = () => ViewModel.FocusAllCommand.Execute(),
                            Accelerator = "Ctrl+A",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_UNFOCUS_ALL,
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
                            Label = SR.MENU_COLLAPSE,
                            Click = () => ViewModel.CollapseCommand.Execute(),
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_ORDER_BY_BRANCH_LENGTH,
                            Submenu = [
                                new MenuItem(){
                                    Type = MenuType.normal,
                                    Label = SR.MENU_ASCENDING,
                                    Click = () => ViewModel.OrderByBranchLengthCommand.Execute(false),
                                },
                                new MenuItem(){
                                    Type = MenuType.normal,
                                    Label = SR.MENU_DESCENDING,
                                    Click = () => ViewModel.OrderByBranchLengthCommand.Execute(true),
                                },
                            ],
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_CLEAR_BRANCH_LENGTH,
                            Click = () => ViewModel.ClearBranchLenghesCommand.Execute(),
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_CLEAR_SUPPORT_VALUE,
                            Click = () => ViewModel.ClearSupportsCommand.Execute(),
                        },
                    ],
                },

                // Window
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = SR.MENU_WINDOW,
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_EDIT_CONFIGURATION,
                            Click = () => ShowConfigWindow().Wait(),
                        },
                    ],
                },

                // Help
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = SR.MENU_HELP,
                    Submenu = [
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_SHOW_HELP,
                            Click = ShowHelp,
                            Accelerator = "F1",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = SR.MENU_VERSION_INFORMATION,
                            Click = () => ShowVersionWindow().Wait(),
                        },
                    ],
                },
#if DEBUG
                // Debug
                new MenuItem()
                {
                    Label = SR.MENU_DEBUG,
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
                FileName = "https://github.com/Funny-Silkie/TreeMage/blob/master/README.md",
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
