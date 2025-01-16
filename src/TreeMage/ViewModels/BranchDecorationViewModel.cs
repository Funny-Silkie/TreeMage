using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Models;

namespace TreeMage.ViewModels
{
    /// <summary>
    /// 枝の装飾UIのViewModelを表します。
    /// </summary>
    public class BranchDecorationViewModel : ViewModelBase
    {
        private readonly BranchDecorationModel model;

        /// <inheritdoc cref="BranchDecorationModel.Style"/>
        public BranchDecorationStyle Style => model.Style;

        /// <inheritdoc cref="BranchDecorationModel.TargetRegexPattern"/>
        public ReactivePropertySlim<string?> TargetRegexPattern { get; }

        /// <inheritdoc cref="BranchDecorationModel.DecorationType"/>
        public ReactivePropertySlim<BranchDecorationType> DecorationType { get; }

        /// <inheritdoc cref="BranchDecorationModel.ShapeSize"/>
        public ReactivePropertySlim<int> ShapeSize { get; }

        /// <inheritdoc cref="BranchDecorationModel.ShapeColor"/>
        public ReactivePropertySlim<string> ShapeColor { get; }

        /// <inheritdoc cref="BranchDecorationModel.Visible"/>
        public ReactivePropertySlim<bool> Visible { get; }

        /// <summary>
        /// 自身を削除するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand DeleteSelfCommand { get; }

        /// <summary>
        /// <see cref="BranchDecorationViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="homeViewModel">親となる<see cref="HomeViewModel"/>のインスタンス</param>
        /// <param name="model">対象のModel</param>
        public BranchDecorationViewModel(BranchDecorationModel model)
        {
            this.model = model;

            TargetRegexPattern = model.ToReactivePropertySlimAsSynchronized(x => x.TargetRegexPattern.Value)
                                      .AddTo(Disposables);
            DecorationType = model.ToReactivePropertySlimAsSynchronized(x => x.DecorationType.Value)
                                  .AddTo(Disposables);
            ShapeSize = model.ToReactivePropertySlimAsSynchronized(x => x.ShapeSize.Value)
                             .AddTo(Disposables);
            ShapeColor = model.ToReactivePropertySlimAsSynchronized(x => x.ShapeColor.Value)
                             .AddTo(Disposables);
            Visible = model.ToReactivePropertySlimAsSynchronized(x => x.Visible.Value)
                           .AddTo(Disposables);

            DeleteSelfCommand = new AsyncReactiveCommand().WithSubscribe(model.DeleteSelf)
                                                          .AddTo(Disposables);
        }
    }
}
