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
        /// 葉が選択されているかどうかかを表す値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactiveProperty<bool> LeafSelected { get; }

        /// <summary>
        /// 枝色のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> BranchColor { get; }

        /// <summary>
        /// 葉ラベル色のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> LeafColor { get; }

        /// <summary>
        /// クレード名のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> CladeLabel { get; }

        /// <summary>
        /// シェードの色のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> ShadeColor { get; }

        /// <summary>
        /// 葉ラベルのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> LeafLabel { get; }

        /// <summary>
        /// サポート値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> Supports { get; }

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
            LeafSelected = mainModel.ObserveProperty(x => x.FocusedSvgElementIdList)
                                    .Select(x => x.Any(x => x.Clade.IsLeaf))
                                    .ToReadOnlyReactiveProperty()
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
            ShadeColor = new ReactiveProperty<string?>().WithSubscribe(v =>
            {
                if (updating || v is null) return;

                CladeId id = FirstSelectedElement.Value;
                if (id.Clade is null) return;

                this.mainModel.OperateAsUndoable((arg, tree) =>
                {
                    ShadeColor!.Value = arg.after;
                    arg.clade.Style.ShadeColor = arg.after;

                    this.mainModel.NotifyTreeUpdated();
                }, (arg, tree) =>
                {
                    ShadeColor!.Value = arg.before;
                    arg.clade.Style.ShadeColor = arg.before;

                    this.mainModel.NotifyTreeUpdated();
                }, (clade: id.Clade, before: id.Clade.Style.ShadeColor, after: string.IsNullOrEmpty(v) ? null : v));
            }).AddTo(Disposables);
            BranchColor = new ReactiveProperty<string?>("black").WithSubscribe(v =>
            {
                if (updating || v is null) return;

                (CladeStyle style, string before)[] targets = mainModel.FocusedSvgElementIdList.Select(x => (style: x.Clade.Style, before: x.Clade.Style.BranchColor))
                                                                                               .ToArray();

                mainModel.OperateAsUndoable(arg =>
                {
                    foreach ((CladeStyle style, string before) in arg.targets) style.BranchColor = arg.after;
                    Update();
                    mainModel.NotifyTreeUpdated();
                }, arg =>
                {
                    foreach ((CladeStyle style, string before) in arg.targets) style.BranchColor = before;
                    Update();
                    mainModel.NotifyTreeUpdated();
                }, (targets, after: v));
            }).AddTo(Disposables);
            LeafColor = new ReactiveProperty<string?>("black").WithSubscribe(v =>
            {
                if (updating || v is null) return;

                (CladeStyle style, string before)[] targets = mainModel.FocusedSvgElementIdList.Where(x => x.Clade.IsLeaf)
                                                                                               .Select(x => (style: x.Clade.Style, before: x.Clade.Style.LeafColor))
                                                                                               .ToArray();

                mainModel.OperateAsUndoable(arg =>
                {
                    foreach ((CladeStyle style, string before) in arg.targets) style.LeafColor = arg.after;
                    Update();
                    mainModel.NotifyTreeUpdated();
                }, arg =>
                {
                    foreach ((CladeStyle style, string before) in arg.targets) style.LeafColor = before;
                    Update();
                    mainModel.NotifyTreeUpdated();
                }, (targets, after: v, selectionTarget: SelectionTarget.Value));
            }).AddTo(Disposables);
            LeafLabel = new ReactiveProperty<string?>().WithSubscribe(v =>
            {
                if (updating) return;

                CladeId id = FirstSelectedElement.Value;
                if (id.Clade is null) return;

                this.mainModel.OperateAsUndoable((arg, tree) =>
                {
                    LeafLabel!.Value = arg.after;
                    arg.clade.Taxon = arg.after;

                    this.mainModel.NotifyTreeUpdated();
                }, (arg, tree) =>
                {
                    LeafLabel!.Value = arg.before;
                    arg.clade.Taxon = arg.before;

                    this.mainModel.NotifyTreeUpdated();
                }, (clade: id.Clade, before: id.Clade.Taxon, after: string.IsNullOrEmpty(v) ? null : v));
            }).AddTo(Disposables);
            Supports = new ReactiveProperty<string?>().WithSubscribe(v =>
            {
                if (updating) return;

                CladeId id = FirstSelectedElement.Value;
                if (id.Clade is null) return;

                this.mainModel.OperateAsUndoable((arg, tree) =>
                {
                    Supports!.Value = arg.after;
                    arg.clade.Supports = arg.after;

                    this.mainModel.NotifyTreeUpdated();
                }, (arg, tree) =>
                {
                    Supports!.Value = arg.before;
                    arg.clade.Supports = arg.before;

                    this.mainModel.NotifyTreeUpdated();
                }, (clade: id.Clade, before: id.Clade.Supports, after: string.IsNullOrEmpty(v) ? null : v));
            });

            updating = false;
            mainModel.ClearUndoQueue();
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
                List<string> branchColors = mainModel.FocusedSvgElementIdList.Select(x => x.Clade.Style.BranchColor)
                                                                             .Distinct()
                                                                             .ToList();
                BranchColor.Value = branchColors.Count == 1 ? branchColors[0] : null;
                List<string> leafColors = mainModel.FocusedSvgElementIdList.Where(x => x.Clade.IsLeaf)
                                                                           .Select(x => x.Clade.Style.LeafColor)
                                                                           .Distinct()
                                                                           .ToList();
                LeafColor.Value = leafColors.Count == 1 ? leafColors[0] : null;
                CladeLabel.Value = null;
                ShadeColor.Value = null;
                LeafLabel.Value = null;
                Supports.Value = null;

                if (mainModel.FocusedSvgElementIdList.Count == 1)
                {
                    Clade? clade = FirstSelectedElement.Value.Clade;
                    if (clade is null) return;

                    CladeLabel.Value = clade.Style.CladeLabel;
                    ShadeColor.Value = clade.Style.ShadeColor;

                    if (clade.IsLeaf) LeafLabel.Value = clade.Taxon;
                    else Supports.Value = clade.Supports;
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
