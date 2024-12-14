using System.Runtime.CompilerServices;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ウィンドウを扱うViewModelの基底クラスです。
    /// </summary>
    /// <typeparam name="TWindow">扱うウィンドウの型</typeparam>
    /// <typeparam name="TViewModel"><typeparamref name="TWindow"/>に適用するViewModelの型</typeparam>
    public abstract class WindowViewModel<TWindow, TViewModel> : ViewModelBase
        where TWindow : ElectronWindow<TViewModel>
        where TViewModel : ViewModelBase
    {
        /// <summary>
        /// 扱うウィンドウを取得します。
        /// </summary>
        protected TWindow Window { get; }

        /// <summary>
        /// <see cref="WindowViewModel{TWindow}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="window">扱うウィンドウ</param>
        protected WindowViewModel(TWindow window)
        {
            Window = window;
            window.SetViewModel(Unsafe.As<TViewModel>(this));
        }
    }
}
