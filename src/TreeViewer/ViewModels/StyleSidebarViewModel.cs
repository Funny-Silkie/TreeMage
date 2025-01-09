﻿using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Data;
using TreeViewer.Models;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// スタイル編集用のサイドバーのViewModelを表します。
    /// </summary>
    public class StyleSidebarViewModel : ViewModelBase
    {
        private readonly MainModel mainModel;
        private readonly StyleSidebarModel styleSidebarModel;
        private bool updating;

        /// <summary>
        /// 有効かどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsEnable { get; }

        /// <summary>
        /// 選択モードの値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        /// <summary>
        /// 選択中の要素の個数のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<int> FocusedCount { get; }

        /// <summary>
        /// 選択中の最初の要素のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<CladeId> FirstSelectedElement { get; }

        /// <summary>
        /// 色のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string?> Color { get; }

        /// <summary>
        /// クレード名のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string?> CladeLabel { get; }

        /// <summary>
        /// <see cref="StyleSidebarViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public StyleSidebarViewModel(MainModel mainModel, StyleSidebarModel styleSidebarModel)
        {
            updating = true;
            this.mainModel = mainModel;
            this.styleSidebarModel = styleSidebarModel;

            IsEnable = mainModel.EditMode.ObserveProperty(x => x.Value)
                                         .Select(x => x is TreeEditMode.Select)
                                         .ToReadOnlyReactivePropertySlim()
                                         .AddTo(Disposables);
            SelectionTarget = mainModel.SelectionTarget.ToReadOnlyReactivePropertySlim()
                                                       .AddTo(Disposables);
            FocusedCount = mainModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                    .Select(x => x.Count)
                                    .ToReadOnlyReactivePropertySlim()
                                    .AddTo(Disposables);
            FirstSelectedElement = mainModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                            .Select(x => x.FirstOrDefault())
                                            .ToReadOnlyReactivePropertySlim()
                                            .WithSubscribe(x => Update())
                                            .AddTo(Disposables);
            CladeLabel = new ReactivePropertySlim<string?>().WithSubscribe(v =>
            {
                if (updating) return;

                CladeId id = FirstSelectedElement.Value;
                if (id.Clade is null) return;

                this.mainModel.OperateAsUndoable((arg, tree) =>
                {
                    CladeLabel!.Value = arg.after;
                    arg.clade.Style.CladeLabel = arg.after;

                    this.mainModel.NotifyTreeUpdated();
                }, (arg, tree) =>
                {
                    CladeLabel!.Value = arg.before;
                    arg.clade.Style.CladeLabel = arg.before;

                    this.mainModel.NotifyTreeUpdated();
                }, (clade: id.Clade, before: id.Clade.Style.CladeLabel, after: string.IsNullOrEmpty(v) ? null : v));
            }).AddTo(Disposables);
            Color = new ReactivePropertySlim<string?>("black").WithSubscribe(OnColorChanged)
                                                              .AddTo(Disposables);

            updating = false;
        }

        /// <summary>
        /// <see cref="Color"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnColorChanged(string? value)
        {
            if (updating || value is null) return;

            (CladeStyle style, string before)[] targets = mainModel.FocusedSvgElementIdList.Select(x =>
            {
                CladeStyle style = x.Clade.Style;
                string before = SelectionTarget.Value switch
                {
                    SelectionMode.Node or SelectionMode.Clade => style.BranchColor,
                    SelectionMode.Taxa => style.LeafColor,
                    _ => "black",
                };
                return (style, before);
            }).ToArray();

            mainModel.OperateAsUndoable(arg =>
            {
                foreach ((CladeStyle style, string before) in arg.targets)
                    switch (arg.selectionTarget)
                    {
                        case SelectionMode.Node:
                        case SelectionMode.Clade:
                            style.BranchColor = arg.after;
                            break;

                        case SelectionMode.Taxa:
                            style.LeafColor = arg.after;
                            break;
                    }

                mainModel.NotifyTreeUpdated();
                Update();
            }, arg =>
            {
                foreach ((CladeStyle style, string before) in arg.targets)
                    switch (arg.selectionTarget)
                    {
                        case SelectionMode.Node:
                        case SelectionMode.Clade:
                            style.BranchColor = before;
                            break;

                        case SelectionMode.Taxa:
                            style.LeafColor = before;
                            break;
                    }

                Update();
                mainModel.NotifyTreeUpdated();
            }, (targets, after: value, selectionTarget: SelectionTarget.Value));
        }

        /// <summary>
        /// 表示内容の更新を行います。
        /// </summary>
        private void Update()
        {
            if (updating) return;
            updating = true;

            try
            {
                List<string> colors = mainModel.SelectionTarget.Value switch
                {
                    SelectionMode.Node or SelectionMode.Clade => mainModel.FocusedSvgElementIdList.Select(x => x.Clade.Style.BranchColor)
                                                                                              .Distinct()
                                                                                              .ToList(),
                    SelectionMode.Taxa => mainModel.FocusedSvgElementIdList.Select(x => x.Clade.Style.LeafColor)
                                                                       .Distinct()
                                                                       .ToList(),
                    _ => ["black"],
                };
                Color.Value = colors.Count == 1 ? colors[0] : null;
                CladeLabel.Value = null;

                if (mainModel.FocusedSvgElementIdList.Count == 1)
                {
                    Clade? clade = FirstSelectedElement.Value.Clade;
                    if (clade is null) return;

                    CladeLabel.Value = clade.Style.CladeLabel;
                }

                OnPropertyChanged(nameof(FirstSelectedElement));
            }
            catch (Exception e)
            {
                MainWindow.Instance.ShowErrorMessageAsync(e).Wait();
            }
            finally
            {
                updating = false;
            }
        }
    }
}
