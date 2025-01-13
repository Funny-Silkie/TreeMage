using ElectronNET.API;
using ElectronNET.API.Entities;
using TreeViewer.ViewModels;

namespace TreeViewer.Window
{
    /// <summary>
    /// コンフィグ編集画面を表します。
    /// </summary>
    public class EditConfigWindow : ElectronWindow<EditConfigViewModel>
    {
        /// <summary>
        /// インスタンスを取得します。
        /// </summary>
        public static EditConfigWindow Instance { get; } = new EditConfigWindow();

        /// <summary>
        /// <see cref="EditConfigWindow"/>の新しいインスタンスを初期化します。
        /// </summary>
        private EditConfigWindow()
        {
        }

        /// <summary>
        /// ウィンドウが閉じられた際のコールバック
        /// </summary>
        public event Action OnClosed
        {
            add => Window.OnClosed += value;
            remove => Window.OnClosed -= value;
        }

        /// <inheritdoc/>
        protected override async Task<BrowserWindow> CreateWindowInternal()
        {
            BrowserWindow parent = MainWindow.Instance.Window;

            BrowserWindow result = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
            {
                Modal = true,
                Show = false,
                Title = "Edit Configurations",
                Width = 360,
                Height = 200,
            }, CreateUrl("edit-config"));

            parent.OnFocus += ModalFocus;
            result.OnClosed += () =>
            {
                parent.OnFocus -= ModalFocus;
                parent.Focus();
            };
            result.SetParentWindow(parent);
            result.SetMenu([
                new MenuItem()
                {
                    Type = MenuType.normal,
                    Label = "Exit(&X)",
                    Click = Close,
                    Accelerator = "Esc",
                },
            ]);
            result.SetMenuBarVisibility(false);
            result.Show();

            return result;
        }
    }
}
