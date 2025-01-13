using ElectronNET.API;
using TreeViewer.ViewModels;

namespace TreeViewer.Window
{
    /// <summary>
    /// ウィンドウを表します。
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// 対象のウィンドウを取得します。
        /// </summary>
        BrowserWindow Window { get; }

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
    }
}
