using ElectronNET.API.Entities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.Trees;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.Data;
using TreeViewer.Models;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : WindowViewModel<MainWindow, HomeViewModel>
    {
        private readonly MainModel model;

        /// <summary>
        /// 読み込まれた系統樹一覧を取得します。
        /// </summary>
        private ReactiveCollection<Tree> Trees { get; }

        /// <summary>
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> MaxTreeIndex { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactivePropertySlim<Tree?> TargetTree { get; }

        /// <summary>
        /// SVG要素のクリック時に実行されるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<CladeId> SvgElementClickedCommand { get; }

        /// <summary>
        /// ツリーを強制的に再描画するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand RerenderTreeCommand { get; }

        /// <summary>
        /// rerootを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(Clade target, bool asRooted)> RerootCommand { get; }

        /// <summary>
        /// 姉妹同士の交換を行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(Clade target1, Clade target2)> SwapSisterCommand { get; }

        /// <summary>
        /// サブツリーの抽出を行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<Clade> ExtractSubtreeCommand { get; }

        #region Menu

        /// <summary>
        /// プロジェクトを新規作成するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CreateNewCommand { get; }

        /// <summary>
        /// プロジェクトファイルを開くコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand OpenProjectCommand { get; }

        /// <summary>
        /// プロジェクトファイルを保存するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> SaveProjectCommand { get; }

        /// <summary>
        /// 系統樹を出力するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(string path, TreeFormat format)> ExportTreeCommand { get; }

        /// <summary>
        /// エクスポートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(string path, ExportType type)> ExportWithExporterCommand { get; }

        /// <summary>
        /// undoを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand UndoCommand { get; }

        /// <summary>
        /// redoを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand RedoCommand { get; }

        #endregion Menu

        #region Focus

        /// <summary>
        /// 選択されているSVG要素のID一覧を取得します。
        /// </summary>
        public IReadOnlySet<CladeId> FocusedSvgElementIdList => model.FocusedSvgElementIdList;

        /// <summary>
        /// 全てを選択するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand FocusAllCommand { get; }

        /// <summary>
        /// 全選択を解除するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand UnfocusAllCommand { get; }

        #endregion Focus

        #region Header

        /// <summary>
        /// 対象の系統樹のインデックスのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> TreeIndex { get; }

        /// <summary>
        /// 編集モードのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<TreeEditMode> EditMode { get; }

        /// <summary>
        /// 選択モードを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        #endregion Header

        #region Sidebar

        #region Layout

        /// <inheritdoc cref="MainModel.CollapseType"/>
        public ReactivePropertySlim<CladeCollapseType> CollapseType { get; }

        /// <inheritdoc cref="MainModel.CollapsedConstantWidth"/>
        public ReactivePropertySlim<double> CollapsedConstantWidth { get; }

        /// <summary>
        /// 折り畳み処理のコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CollapseCommand { get; }

        /// <summary>
        /// 並び替えを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> OrderByBranchLengthCommand { get; }

        #endregion Layout

        #region Tree

        /// <inheritdoc cref="MainModel.XScale"/>
        public ReactivePropertySlim<int> XScale { get; }

        /// <inheritdoc cref="MainModel.YScale"/>
        public ReactivePropertySlim<int> YScale { get; }

        /// <inheritdoc cref="MainModel.BranchThickness"/>
        public ReactivePropertySlim<int> BranchThickness { get; }

        #endregion Tree

        #region Search

        /// <inheritdoc cref="MainModel.SearchQuery"/>
        public ReactivePropertySlim<string> SearchQuery { get; }

        /// <inheritdoc cref="MainModel.SearchTarget"/>
        public ReactivePropertySlim<TreeSearchTarget> SearchTarget { get; }

        /// <inheritdoc cref="MainModel.SearchOnIgnoreCase"/>
        public ReactivePropertySlim<bool> SearchOnIgnoreCase { get; }

        /// <inheritdoc cref="MainModel.SearchWithRegex"/>
        public ReactivePropertySlim<bool> SearchWithRegex { get; }

        /// <summary>
        /// 検索コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand SearchCommand { get; }

        #endregion Search

        #region LeafLabels

        /// <inheritdoc cref="MainModel.ShowLeafLabels"/>
        public ReactivePropertySlim<bool> ShowLeafLabels { get; }

        /// <inheritdoc cref="MainModel.LeafLabelsFontSize"/>
        public ReactivePropertySlim<int> LeafLabelsFontSize { get; }

        #endregion LeafLabels

        #region CladeLabels

        /// <inheritdoc cref="MainModel.ShowCladeLabels"/>
        public ReactivePropertySlim<bool> ShowCladeLabels { get; }

        /// <inheritdoc cref="MainModel.CladeLabelsFontSize"/>
        public ReactivePropertySlim<int> CladeLabelsFontSize { get; }

        /// <inheritdoc cref="MainModel.CladeLabelsLineThickness"/>
        public ReactivePropertySlim<int> CladeLabelsLineThickness { get; }

        #endregion CladeLabels

        #region NodeValues

        /// <inheritdoc cref="MainModel.ShowNodeValues"/>
        public ReactivePropertySlim<bool> ShowNodeValues { get; }

        /// <inheritdoc cref="MainModel.NodeValueType"/>
        public ReactivePropertySlim<CladeValueType> NodeValueType { get; }

        /// <inheritdoc cref="MainModel.NodeValueFontSize"/>
        public ReactivePropertySlim<int> NodeValueFontSize { get; }

        #endregion NodeValues

        #region BranchValues

        /// <inheritdoc cref="MainModel.ShowBranchValues"/>
        public ReactivePropertySlim<bool> ShowBranchValues { get; }

        /// <inheritdoc cref="MainModel.BranchValueType"/>
        public ReactivePropertySlim<CladeValueType> BranchValueType { get; }

        /// <inheritdoc cref="MainModel.BranchValueFontSize"/>
        public ReactivePropertySlim<int> BranchValueFontSize { get; }

        /// <inheritdoc cref="MainModel.BranchValueHideRegexPattern"/>
        public ReactivePropertySlim<string?> BranchValueHideRegexPattern { get; }

        #endregion BranchValues

        #region BranchDecorations

        /// <inheritdoc cref="MainModel.ShowBranchDecorations"/>
        public ReactivePropertySlim<bool> ShowBranchDecorations { get; }

        /// <inheritdoc cref="MainModel.BranchDecorations"/>
        public ReadOnlyReactiveCollection<BranchDecorationModel> BranchDecorations { get; }

        /// <summary>
        /// 装飾の追加コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand AddBranchDecorationCommand { get; }

        #endregion BranchDecorations

        #region Scalebar

        /// <inheritdoc cref="MainModel.ShowScaleBar"/>
        public ReactivePropertySlim<bool> ShowScaleBar { get; }

        /// <inheritdoc cref="MainModel.ScaleBarValue"/>
        public ReactivePropertySlim<double> ScaleBarValue { get; }

        /// <inheritdoc cref="MainModel.ScaleBarFontSize"/>
        public ReactivePropertySlim<int> ScaleBarFontSize { get; }

        /// <inheritdoc cref="MainModel.ScaleBarThickness"/>
        public ReactivePropertySlim<int> ScaleBarThickness { get; }

        #endregion Scalebar

        #endregion Sidebar

        /// <summary>
        /// <see cref="HomeViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HomeViewModel(MainModel model) : base(MainWindow.Instance)
        {
            this.model = model;
            model.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);

            Trees = model.Trees;
            TreeIndex = model.ToReactivePropertySlimAsSynchronized(x => x.TreeIndex.Value)
                             .AddTo(Disposables);
            EditMode = model.ToReactivePropertySlimAsSynchronized(x => x.EditMode.Value)
                            .AddTo(Disposables);
            MaxTreeIndex = model.ToReactivePropertySlimAsSynchronized(x => x.MaxTreeIndex.Value)
                                .AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = model.ToReactivePropertySlimAsSynchronized(x => x.TargetTree.Value)
                              .AddTo(Disposables);
            SvgElementClickedCommand = new AsyncReactiveCommand<CladeId>().WithSubscribe(OnSvgElementClicked)
                                                                          .AddTo(Disposables);
            RerenderTreeCommand = new AsyncReactiveCommand().WithSubscribe(model.NotifyTreeUpdated).AddTo(Disposables);
            RerootCommand = new AsyncReactiveCommand<(Clade target, bool asRooted)>().WithSubscribe(Reroot)
                                                                                     .AddTo(Disposables);
            SwapSisterCommand = new AsyncReactiveCommand<(Clade target1, Clade target2)>().WithSubscribe(SwapSisters)
                                                                                          .AddTo(Disposables);
            ExtractSubtreeCommand = new AsyncReactiveCommand<Clade>().WithSubscribe(ExtractSubtree)
                                                                     .AddTo(Disposables);

            CreateNewCommand = new AsyncReactiveCommand().WithSubscribe(model.CreateNew)
                                                         .AddTo(Disposables);
            OpenProjectCommand = new AsyncReactiveCommand().WithSubscribe(OpenProject)
                                                           .AddTo(Disposables);
            SaveProjectCommand = new AsyncReactiveCommand<bool>().WithSubscribe(SaveProject)
                                                                 .AddTo(Disposables);
            ExportTreeCommand = new AsyncReactiveCommand<(string path, TreeFormat format)>().WithSubscribe(model.ExportCurrentTreeAsTreeFile)
                                                                                            .AddTo(Disposables);
            ExportWithExporterCommand = new AsyncReactiveCommand<(string path, ExportType type)>().WithSubscribe(model.ExportWithExporter)
                                                                                                  .AddTo(Disposables);
            UndoCommand = new AsyncReactiveCommand().WithSubscribe(model.Undo)
                                                    .AddTo(Disposables);
            RedoCommand = new AsyncReactiveCommand().WithSubscribe(model.Redo)
                                                    .AddTo(Disposables);

            FocusAllCommand = new AsyncReactiveCommand().WithSubscribe(model.FocusAll)
                                                          .AddTo(Disposables);
            UnfocusAllCommand = new AsyncReactiveCommand().WithSubscribe(model.UnfocusAll)
                                                          .AddTo(Disposables);

            SelectionTarget = model.ToReactivePropertySlimAsSynchronized(x => x.SelectionTarget.Value)
                                   .AddTo(Disposables);

            CollapseType = model.ToReactivePropertySlimAsSynchronized(x => x.CollapseType.Value)
                                .AddTo(Disposables);
            CollapsedConstantWidth = model.ToReactivePropertySlimAsSynchronized(x => x.CollapsedConstantWidth.Value)
                                          .AddTo(Disposables);
            CollapseCommand = new AsyncReactiveCommand().WithSubscribe(CollapseClade)
                                                        .AddTo(Disposables);
            OrderByBranchLengthCommand = new AsyncReactiveCommand<bool>().WithSubscribe(OrderByBranchLength)
                                                                         .AddTo(Disposables);

            XScale = model.ToReactivePropertySlimAsSynchronized(x => x.XScale.Value)
                          .AddTo(Disposables);
            YScale = model.ToReactivePropertySlimAsSynchronized(x => x.YScale.Value)
                          .AddTo(Disposables);
            BranchThickness = model.ToReactivePropertySlimAsSynchronized(x => x.BranchThickness.Value)
                                   .AddTo(Disposables);

            SearchQuery = model.ToReactivePropertySlimAsSynchronized(x => x.SearchQuery.Value)
                               .AddTo(Disposables);
            SearchTarget = model.ToReactivePropertySlimAsSynchronized(x => x.SearchTarget.Value)
                                .AddTo(Disposables);
            SearchOnIgnoreCase = model.ToReactivePropertySlimAsSynchronized(x => x.SearchOnIgnoreCase.Value)
                                      .AddTo(Disposables);
            SearchWithRegex = model.ToReactivePropertySlimAsSynchronized(x => x.SearchWithRegex.Value)
                                   .AddTo(Disposables);
            SearchCommand = new AsyncReactiveCommand().WithSubscribe(model.Search)
                                                      .AddTo(Disposables);

            ShowLeafLabels = model.ToReactivePropertySlimAsSynchronized(x => x.ShowLeafLabels.Value)
                                  .AddTo(Disposables);
            LeafLabelsFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.LeafLabelsFontSize.Value)
                                      .AddTo(Disposables);

            ShowCladeLabels = model.ToReactivePropertySlimAsSynchronized(x => x.ShowCladeLabels.Value)
                                   .AddTo(Disposables);
            CladeLabelsFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.CladeLabelsFontSize.Value)
                                       .AddTo(Disposables);
            CladeLabelsLineThickness = model.ToReactivePropertySlimAsSynchronized(x => x.CladeLabelsLineThickness.Value)
                                            .AddTo(Disposables);

            ShowNodeValues = model.ToReactivePropertySlimAsSynchronized(x => x.ShowNodeValues.Value)
                                  .AddTo(Disposables);
            NodeValueType = model.ToReactivePropertySlimAsSynchronized(x => x.NodeValueType.Value)
                                 .AddTo(Disposables);
            NodeValueFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.NodeValueFontSize.Value)
                                     .AddTo(Disposables);

            ShowBranchValues = model.ToReactivePropertySlimAsSynchronized(x => x.ShowBranchValues.Value)
                                    .AddTo(Disposables);
            BranchValueType = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueType.Value)
                                   .AddTo(Disposables);
            BranchValueFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueFontSize.Value)
                                       .AddTo(Disposables);
            BranchValueHideRegexPattern = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueHideRegexPattern.Value)
                                               .AddTo(Disposables);

            ShowBranchDecorations = model.ToReactivePropertySlimAsSynchronized(x => x.ShowBranchDecorations.Value)
                                         .AddTo(Disposables);
            BranchDecorations = model.BranchDecorations.ToReadOnlyReactiveCollection()
                                                       .AddTo(Disposables);
            AddBranchDecorationCommand = new AsyncReactiveCommand().WithSubscribe(model.AddNewBranchDecoration)
                                                                   .AddTo(Disposables);

            ShowScaleBar = model.ToReactivePropertySlimAsSynchronized(x => x.ShowScaleBar.Value)
                                .AddTo(Disposables);
            ScaleBarValue = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarValue.Value)
                                 .AddTo(Disposables);
            ScaleBarFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarFontSize.Value)
                                    .AddTo(Disposables);
            ScaleBarThickness = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarThickness.Value)
                                     .AddTo(Disposables);

            model.ClearUndoQueue();
        }

        /// <summary>
        /// SVG要素がクリックされた際に実行されます。
        /// </summary>
        /// <param name="id">SVG要素のID</param>
        private void OnSvgElementClicked(CladeId id)
        {
            try
            {
                if (EditMode.Value is TreeEditMode.Select)
                {
                    switch (SelectionTarget.Value)
                    {
                        case SelectionMode.Node:
                            {
                                model.Focus(id.Clade);
                            }
                            break;

                        case SelectionMode.Clade:
                            {
                                Clade target = id.Clade;
                                model.Focus(target.GetDescendants().Prepend(target));
                            }
                            break;

                        case SelectionMode.Taxa:
                            {
                                Clade target = id.Clade;
                                if (target.IsLeaf) model.Focus(target);
                                else model.Focus(target.GetDescendants().Where(x => x.IsLeaf));
                            }
                            break;
                    }

                    OnPropertyChanged(nameof(FocusedSvgElementIdList));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Window.ShowErrorMessageAsync(e).Wait();
            }
        }

        /// <inheritdoc cref="MainModel.Reroot(Clade, bool)"/>
        private async Task Reroot(Clade clade, bool asRooted)
        {
            try
            {
                model.Reroot(clade, asRooted);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.SwapSisters(Clade, Clade)"/>
        private async Task SwapSisters(Clade target1, Clade target2)
        {
            try
            {
                model.SwapSisters(target1, target2);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.ExtractSubtree(Clade)"/>
        private async Task ExtractSubtree(Clade clade)
        {
            try
            {
                model.ExtractSubtree(clade);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.CollapseClade"/>
        private async Task CollapseClade()
        {
            try
            {
                model.CollapseClade();
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.OrderByBranchLength(bool)"/>
        private async Task OrderByBranchLength(bool descending)
        {
            try
            {
                model.OrderByBranchLength(descending);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.OpenProject(string)"/>
        private async Task OpenProject()
        {
            string? path = await Window.ShowSingleFileOpenDialog([new FileFilter()
            {
                Name = "Tree viewer project file",
                Extensions = ["treeprj"],
            }]);
            if (path is null) return;

            try
            {
                await model.OpenProject(path);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.SaveProject(string)"/>
        private async Task SaveProject(bool asNew)
        {
            string path;
            if (asNew || model.ProjectPath.Value is null)
            {
                string? selectedPath = await Window.ShowFileSaveDialog([new FileFilter()
                {
                    Name = "Tree viewer project file",
                    Extensions = ["treeprj"],
                }]);

                if (selectedPath is null) return;
                path = selectedPath;
            }
            else path = model.ProjectPath.Value;

            try
            {
                await model.SaveProject(path);
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.ImportTree(string, TreeFormat)"/>
        public async Task ImportTree(string path, TreeFormat format)
        {
            try
            {
                await model.ImportTree(path, format);
            }
            catch (TreeFormatException e)
            {
                await Window.ShowErrorMessageAsync("ツリーのフォーマットが無効です");
                await Console.Out.WriteLineAsync(e.ToString());
                return;
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }
    }
}
