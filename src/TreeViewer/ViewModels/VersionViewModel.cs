using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// バージョン表示画面のViewModelのクラスです。
    /// </summary>
    public class VersionViewModel : WindowViewModel<VersionWindow, VersionViewModel>
    {
        /// <summary>
        /// <see cref="VersionViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public VersionViewModel() : base(VersionWindow.Instance)
        {
        }
    }
}
