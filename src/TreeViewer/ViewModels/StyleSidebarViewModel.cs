﻿using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Data;
using TreeViewer.Models;

namespace TreeViewer.ViewModels
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

        /// <inheritdoc cref="StyleSidebarModel.Color"/>
        public ReactivePropertySlim<string?> Color { get; }

        /// <inheritdoc cref="StyleSidebarModel.CladeLabel"/>
        public ReactivePropertySlim<string?> CladeLabel { get; }

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
            CladeLabel = styleSidebarmodel.CladeLabel.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                     .AddTo(Disposables);
            Color = styleSidebarmodel.Color.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                           .AddTo(Disposables);
        }
    }
}
