﻿using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Diagnostics;
using TreeMage.Core.Exporting;
using TreeMage.Core.Trees.Parsers;
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
            result.OnReadyToShow += () =>
            {
                result.Show();
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
                            Label = "Open Project(&O)",
                            Click = () => ViewModel.OpenProjectCommand.Execute(),
                            Accelerator = "Ctrl+O",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Save Project(&S)",
                            Click = () => ViewModel.SaveProjectCommand.Execute(false),
                            Accelerator = "Ctrl+S",
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Save as New Project(&A)",
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
                            Label = "Import Tree(&I)",
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
                            Label = "Exit(&X)",
                            Click = Close,
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
                            Label = "Collapse",
                            Click = () => ViewModel.CollapseCommand.Execute(),
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
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Clear branch lenghes",
                            Click = () => ViewModel.ClearBranchLenghesCommand.Execute(),
                        },
                        new MenuItem()
                        {
                            Type = MenuType.normal,
                            Label = "Clear support values",
                            Click = () => ViewModel.ClearSupportsCommand.Execute(),
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
