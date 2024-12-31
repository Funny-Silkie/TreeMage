using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Data;

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
        /// 選択モードの値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        /// <summary>
        /// 選択中の要素の個数を取得します。
        /// </summary>
        public ReactivePropertySlim<int> FocusedCount { get; }

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
            this.homeViewModel = homeViewModel;

            SelectionTarget = homeViewModel.SelectionTarget.ToReadOnlyReactivePropertySlim()
                                                           .AddTo(Disposables);
            FocusedCount = new ReactivePropertySlim<int>(0).AddTo(Disposables);
            Color = new ReactivePropertySlim<string?>("black", ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(OnColorChanged)
                                                              .AddTo(Disposables);
        }

        /// <summary>
        /// <see cref="Color"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnColorChanged(string? value)
        {
            if (updating || value is null) return;

            homeViewModel.SetColorToFocusedObject(value);
        }

        /// <summary>
        /// 表示内容の更新を行います。
        /// </summary>
        public void Update()
        {
            updating = true;

            try
            {
                FocusedCount.Value = homeViewModel.FocusedSvgElementIdList.Count;

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
            finally
            {
                updating = false;
            }
        }
    }
}
