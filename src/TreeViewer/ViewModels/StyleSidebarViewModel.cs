using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Data;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// スタイル編集用のサイドバーのViewModelを表します。
    /// </summary>
    public class StyleSidebarViewModel : ViewModelBase
    {
        private readonly HomeViewModel homeViewModel;
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
        /// <see cref="StyleSidebarViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="homeViewModel">親となる<see cref="HomeViewModel"/>のインスタンス</param>
        public StyleSidebarViewModel(HomeViewModel homeViewModel)
        {
            updating = true;
            this.homeViewModel = homeViewModel;

            IsEnable = homeViewModel.EditMode.ObserveProperty(x => x.Value)
                                             .Select(x => x is TreeEditMode.Select)
                                             .ToReadOnlyReactivePropertySlim()
                                             .AddTo(Disposables);
            SelectionTarget = homeViewModel.SelectionTarget.ToReadOnlyReactivePropertySlim()
                                                           .AddTo(Disposables);
            FocusedCount = homeViewModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                        .Select(x => x.Count)
                                        .ToReadOnlyReactivePropertySlim()
                                        .AddTo(Disposables);
            FirstSelectedElement = homeViewModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                                .Select(x => x.FirstOrDefault())
                                                .ToReadOnlyReactivePropertySlim()
                                                .WithSubscribe(x => Update())
                                                .AddTo(Disposables);
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

            (CladeStyle style, string before)[] targets = homeViewModel.FocusedSvgElementIdList.Select(x =>
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

            homeViewModel.OperateAsUndoable(arg =>
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

                homeViewModel.RerenderTreeCommand.Execute();
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
                homeViewModel.RerenderTreeCommand.Execute();
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
                List<string> colors = homeViewModel.SelectionTarget.Value switch
                {
                    SelectionMode.Node or SelectionMode.Clade => homeViewModel.FocusedSvgElementIdList.Select(x => x.Clade.Style.BranchColor)
                                                                                                      .Distinct()
                                                                                                      .ToList(),
                    SelectionMode.Taxa => homeViewModel.FocusedSvgElementIdList.Select(x => x.Clade.Style.LeafColor)
                                                                               .Distinct()
                                                                               .ToList(),
                    _ => ["black"],
                };
                Color.Value = colors.Count == 1 ? colors[0] : null;
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
