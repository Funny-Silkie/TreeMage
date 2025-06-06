﻿using ElectronNET.API;
using ElectronNET.API.Entities;
using TreeMage.Resources;
using TreeMage.ViewModels;

namespace TreeMage.Window
{
    /// <summary>
    /// バージョン表示画面を表します。
    /// </summary>
    public class VersionWindow : ElectronWindow<VersionViewModel>
    {
        /// <summary>
        /// インスタンスを取得します。
        /// </summary>
        public static VersionWindow Instance { get; } = new VersionWindow();

        /// <summary>
        /// <see cref="VersionWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        private VersionWindow()
        {
        }

        /// <inheritdoc/>
        protected override async Task<BrowserWindow> CreateWindowInternal()
        {
            BrowserWindow parent = MainWindow.Instance.Window;

            BrowserWindow result = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Modal = true,
                Show = false,
                Title = SR.WINDOW_TITLE_VERSIONS,
                Width = 390,
                Height = 180,
                Resizable = false,
            }, CreateUrl("versions"));

            result.SetParentWindow(parent);
            result.SetMenu([
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = SR.MENU_EXIT,
                    Click = Close,
                    Accelerator = "Esc",
                },
            ]);
            result.SetMenuBarVisibility(false);
            result.Show();
            result.OnResize += async () =>
            {
                int[] size = await result.GetSizeAsync();
                await Console.Out.WriteLineAsync(string.Join(", ", size));
            };

            return result;
        }
    }
}
