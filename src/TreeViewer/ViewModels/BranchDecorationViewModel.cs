using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Core.Drawing.Styles;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// 枝の装飾UIのViewModelを表します。
    /// </summary>
    public class BranchDecorationViewModel : ViewModelBase
    {
        private readonly HomeViewModel homeViewModel;

        /// <summary>
        /// 対象の正規表現のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string> TargetRegexPattern { get; }

        /// <summary>
        /// 装飾の種類のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<BranchDecorationType> DecorationType { get; }

        /// <summary>
        /// 図形のサイズを表すプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> ShapeSize { get; }

        /// <summary>
        /// 図形の色を表すプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string> ShapeColor { get; }

        /// <summary>
        /// 表示するどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> Visible { get; }

        /// <summary>
        /// 自身を削除するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand DeleteSelfCommand { get; }

        /// <summary>
        /// <see cref="BranchDecorationViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="homeViewModel">親となる<see cref="HomeViewModel"/>のインスタンス</param>
        public BranchDecorationViewModel(HomeViewModel homeViewModel)
        {
            this.homeViewModel = homeViewModel;

            TargetRegexPattern = new ReactivePropertySlim<string>(string.Empty).AddTo(Disposables);
            DecorationType = new ReactivePropertySlim<BranchDecorationType>().AddTo(Disposables);
            ShapeSize = new ReactivePropertySlim<int>(5).AddTo(Disposables);
            ShapeColor = new ReactivePropertySlim<string>("#000000").AddTo(Disposables);
            Visible = new ReactivePropertySlim<bool>(true).AddTo(Disposables);

            DeleteSelfCommand = new AsyncReactiveCommand().WithSubscribe(DeleteSelf)
                                                          .AddTo(Disposables);
        }

        /// <summary>
        /// 自身を削除します。
        /// </summary>
        private void DeleteSelf()
        {
            homeViewModel.BranchDecorations.RemoveOnScheduler(this);
        }
    }
}
