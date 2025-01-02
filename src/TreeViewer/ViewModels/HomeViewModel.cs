using ElectronNET.API.Entities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
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
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<int> MaxTreeIndex { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<Tree?> TargetTree { get; }

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
        /// 折り畳み処理のコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CollapseCommand { get; }

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

        /// <summary>
        /// 並び替えを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> OrderByBranchLengthCommand { get; }

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

        /// <summary>
        /// <see cref="HomeViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HomeViewModel(MainModel model) : base(MainWindow.Instance)
        {
            this.model = model;
            model.PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);

            TreeIndex = model.ToReactivePropertySlimAsSynchronized(x => x.TreeIndex.Value)
                             .AddTo(Disposables);
            EditMode = model.ToReactivePropertySlimAsSynchronized(x => x.EditMode.Value)
                            .AddTo(Disposables);
            MaxTreeIndex = model.MaxTreeIndex.ToReadOnlyReactivePropertySlim()
                                             .AddTo(Disposables);
            TargetTree = model.TargetTree.ToReadOnlyReactivePropertySlim()
                                         .AddTo(Disposables);
            SvgElementClickedCommand = new AsyncReactiveCommand<CladeId>().WithSubscribe(OnSvgElementClicked)
                                                                          .AddTo(Disposables);
            RerenderTreeCommand = new AsyncReactiveCommand().WithSubscribe(model.NotifyTreeUpdated).AddTo(Disposables);
            RerootCommand = new AsyncReactiveCommand<(Clade target, bool asRooted)>().WithSubscribe(Reroot)
                                                                                     .AddTo(Disposables);
            SwapSisterCommand = new AsyncReactiveCommand<(Clade target1, Clade target2)>().WithSubscribe(SwapSisters)
                                                                                          .AddTo(Disposables);
            OrderByBranchLengthCommand = new AsyncReactiveCommand<bool>().WithSubscribe(OrderByBranchLength)
                                                                         .AddTo(Disposables);
            CollapseCommand = new AsyncReactiveCommand().WithSubscribe(CollapseClade)
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
