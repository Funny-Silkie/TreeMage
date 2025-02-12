using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeMage.Data;
using TreeMage.Models;

namespace TreeMage.ViewModels
{
    /// <summary>
    /// スタイル編集用のサイドバーのViewModelを表します。
    /// </summary>
    public class StyleSidebarViewModel : ViewModelBase
    {
        /// <inheritdoc cref="StyleSidebarModel.IsEnable"/>
        public ReadOnlyReactivePropertySlim<bool> IsEnable { get; }

        /// <inheritdoc cref="StyleSidebarModel.SelectionTarget"/>
        public ReadOnlyReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        /// <inheritdoc cref="StyleSidebarModel.FocusedCount"/>
        public ReadOnlyReactivePropertySlim<int> FocusedCount { get; }

        /// <inheritdoc cref="StyleSidebarModel.FirstSelectedElement"/>
        public ReadOnlyReactivePropertySlim<CladeId> FirstSelectedElement { get; }

        /// <inheritdoc cref="StyleSidebarModel.LeafSelected"/>
        public ReadOnlyReactivePropertySlim<bool> LeafSelected { get; }

        /// <inheritdoc cref="StyleSidebarModel.BranchColor"/>
        public ReactivePropertySlim<string?> BranchColor { get; }

        /// <inheritdoc cref="StyleSidebarModel.LeafColor"/>
        public ReactivePropertySlim<string?> LeafColor { get; }

        /// <inheritdoc cref="StyleSidebarModel.CladeLabel"/>
        public ReactivePropertySlim<string?> CladeLabel { get; }

        /// <inheritdoc cref="StyleSidebarModel.ShadeColor"/>
        public ReactivePropertySlim<string?> ShadeColor { get; }

        /// <inheritdoc cref="StyleSidebarModel.LeafLabel"/>
        public ReactivePropertySlim<string?> LeafLabel { get; }

        /// <inheritdoc cref="StyleSidebarModel.Supports"/>
        public ReactivePropertySlim<string?> Supports { get; }

        /// <inheritdoc cref="StyleSidebarModel.YScale"/>
        public ReactivePropertySlim<double> YScale { get; }

        /// <summary>
        /// <see cref="StyleSidebarViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public StyleSidebarViewModel(MainModel mainModel, StyleSidebarModel styleSidebarmodel)
        {
            mainModel.PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);

            IsEnable = styleSidebarmodel.IsEnable.ToReadOnlyReactivePropertySlim()
                                                 .AddTo(Disposables);
            SelectionTarget = styleSidebarmodel.SelectionTarget.ToReadOnlyReactivePropertySlim()
                                                               .AddTo(Disposables);
            FocusedCount = styleSidebarmodel.FocusedCount.ToReadOnlyReactivePropertySlim()
                                                         .AddTo(Disposables);
            FirstSelectedElement = styleSidebarmodel.FirstSelectedElement.ToReadOnlyReactivePropertySlim()
                                                                         .AddTo(Disposables);
            LeafSelected = styleSidebarmodel.LeafSelected.ToReadOnlyReactivePropertySlim()
                                                         .AddTo(Disposables);
            CladeLabel = styleSidebarmodel.CladeLabel.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                     .AddTo(Disposables);
            ShadeColor = styleSidebarmodel.ShadeColor.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                     .AddTo(Disposables);
            BranchColor = styleSidebarmodel.BranchColor.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                       .AddTo(Disposables);
            LeafColor = styleSidebarmodel.LeafColor.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                   .AddTo(Disposables);
            LeafLabel = styleSidebarmodel.LeafLabel.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                   .AddTo(Disposables);
            Supports = styleSidebarmodel.Supports.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                 .AddTo(Disposables);
            YScale = styleSidebarmodel.YScale.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                             .AddTo(Disposables);
        }
    }
}
