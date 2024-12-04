using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Core.Trees;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactivePropertySlim<Tree?> TargetTree { get; }

        /// <summary>
        /// 系統名を表示するかどうかを表す値を取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowLeaveLabels { get; }

        /// <summary>
        /// <see cref="HomeViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HomeViewModel()
        {
            TargetTree = new ReactivePropertySlim<Tree?>(Tree.CreateSample()).AddTo(Disposables);
            ShowLeaveLabels = new ReactivePropertySlim<bool>().AddTo(Disposables);
        }
    }
}
