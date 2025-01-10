using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Data;
using TreeViewer.Window;

namespace TreeViewer.Models
{
    /// <summary>
    /// スタイル編集サイドバー用のModelのクラスです。
    /// </summary>
    public class StyleSidebarModel : ModelBase
    {
        private readonly MainModel mainModel;
        private bool updating;

        /// <summary>
        /// 有効かどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsEnable { get; }

        /// <summary>
        /// 選択モードの値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactiveProperty<SelectionMode> SelectionTarget { get; }

        /// <summary>
        /// 選択中の要素の個数のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactiveProperty<int> FocusedCount { get; }

        /// <summary>
        /// 選択中の最初の要素のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactiveProperty<CladeId> FirstSelectedElement { get; }

        /// <summary>
        /// 色のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> Color { get; }

        /// <summary>
        /// クレード名のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> CladeLabel { get; }

        /// <summary>
        /// <see cref="StyleSidebarModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public StyleSidebarModel(MainModel mainModel)
        {
            updating = true;
            this.mainModel = mainModel;

            IsEnable = mainModel.EditMode.ObserveProperty(x => x.Value)
                                         .Select(x => x is TreeEditMode.Select)
                                         .ToReadOnlyReactiveProperty()
                                         .AddTo(Disposables);
            SelectionTarget = mainModel.SelectionTarget.ToReadOnlyReactiveProperty()
                                                       .AddTo(Disposables);
            FocusedCount = mainModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                    .Select(x => x.Count)
                                    .ToReadOnlyReactiveProperty()
                                    .AddTo(Disposables);
            FirstSelectedElement = mainModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                            .Select(x => x.FirstOrDefault())
                                            .ToReadOnlyReactiveProperty()
                                            .WithSubscribe(x => Update())
                                            .AddTo(Disposables);
            CladeLabel = new ReactiveProperty<string?>().WithSubscribe(v =>
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
            Color = new ReactiveProperty<string?>("black").WithSubscribe(OnColorChanged)
                                                              .AddTo(Disposables);

            updating = false;
            mainModel.ClearUndoQueue();
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
