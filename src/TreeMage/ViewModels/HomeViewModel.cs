using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeMage.Core.Exporting;
using TreeMage.Core.Trees;
using TreeMage.Core.Trees.Parsers;
using TreeMage.Data;
using TreeMage.Models;
using TreeMage.Services;

namespace TreeMage.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        private readonly IElectronService electronService;
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
        /// ファイルがドロップされた際に実行されるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<string[]> FileDroppedCommand { get; }

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
                await electronService.ShowErrorMessageAsync(e);
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
        /// ツリーのインポートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<TreeFormat> ImportTreeCommand { get; }

        /// <summary>
        /// 系統樹を出力するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<TreeFormat> ExportTreeCommand { get; }

        /// <summary>
        /// エクスポートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<ExportType> ExportWithExporterCommand { get; }

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
        public HomeViewModel(MainModel model, IElectronService electronService)
        {
            this.electronService = electronService;
            electronService.AttachViewModel(this);
            this.model = model;
            model.PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);

            model.ProjectPath.CombineLatest(model.Saved)
                             .Subscribe(v =>
                             {
                                 string? projectPath = v.First;

                                 string title;
                                 if (v.Second)
                                 {
                                     if (string.IsNullOrEmpty(projectPath)) title = "TreeMage";
                                     else title = $"{Path.GetFileName(projectPath)} - TreeMage";
                                 }
                                 else
                                 {
                                     if (string.IsNullOrEmpty(projectPath)) title = "TreeMage*";
                                     else title = $"{Path.GetFileName(projectPath)}* - TreeMage";
                                 }
                                 electronService.Title = title;
                             });
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
            FileDroppedCommand = new AsyncReactiveCommand<string[]>().WithSubscribe(OnFileDropped)
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

            CreateNewCommand = new AsyncReactiveCommand().WithSubscribe(CreateNew)
                                                         .AddTo(Disposables);
            OpenProjectCommand = new AsyncReactiveCommand().WithSubscribe(OpenProject)
                                                           .AddTo(Disposables);
            SaveProjectCommand = new AsyncReactiveCommand<bool>().WithSubscribe(SaveProject)
                                                                 .AddTo(Disposables);
            ImportTreeCommand = new AsyncReactiveCommand<TreeFormat>().WithSubscribe(ImportTree)
                                                                      .AddTo(Disposables);
            ExportTreeCommand = new AsyncReactiveCommand<TreeFormat>().WithSubscribe(ExportAsTreeFile)
                                                                      .AddTo(Disposables);
            ExportWithExporterCommand = new AsyncReactiveCommand<ExportType>().WithSubscribe(ExportWithExporter)
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
                electronService.ShowErrorMessageAsync(e).Wait();
            }
        }

        /// <summary>
        /// ウィンドウを閉じられるかどうかを確認します。
        /// </summary>
        /// <returns>閉じられる場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public async Task<bool> VerifyCanClose()
        {
            if (model.Saved.Value) return true;
            return await electronService.ShowVerifyDialogAsync("Unsaved changes are detected. Are you sure?", buttons: ["Close anyway", "Cancel"]);
        }

        /// <summary>
        /// ファイルがドロップされたときに実行されます。
        /// </summary>
        /// <param name="pathes">読み込まれたファイルパス</param>
        private async Task OnFileDropped(string[] pathes)
        {
            try
            {
                await model.OpenFiles(pathes);
            }
            catch (Exception e)
            {
                await electronService.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
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
                await electronService.ShowErrorMessageAsync(e);
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
                await electronService.ShowErrorMessageAsync(e);
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
                await electronService.ShowErrorMessageAsync(e);
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
                await electronService.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.CreateNew"/>
        private async Task CreateNew()
        {
            if (!model.Saved.Value)
            {
                bool allowDiscard = await electronService.ShowVerifyDialogAsync("Unsaved changes are detected. Are you sure to discard these changes?");
                if (!allowDiscard) return;
            }

            model.CreateNew();
        }

        /// <inheritdoc cref="MainModel.OpenProject(string)"/>
        private async Task OpenProject()
        {
            string? path = await electronService.ShowSingleFileOpenDialogAsync((["treeprj"], "Tree viewer project file"));
            if (path is null) return;

            if (!model.Saved.Value)
            {
                bool allowDiscard = await electronService.ShowVerifyDialogAsync("Unsaved changes are detected. Are you sure to discard these changes?");
                if (!allowDiscard) return;
            }

            try
            {
                await model.OpenProject(path);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                await electronService.ShowErrorMessageAsync(e);
            }
        }

        /// <inheritdoc cref="MainModel.SaveProject(string)"/>
        private async Task SaveProject(bool asNew)
        {
            string path;
            if (asNew || model.ProjectPath.Value is null)
            {
                string? selectedPath = await electronService.ShowFileSaveDialogAsync((["treeprj"], "Tree viewer project file"));

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
                await electronService.ShowErrorMessageAsync(e);
            }
        }

        /// <summary>
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="format">系統樹のフォーマット</param>
        private async Task ImportTree(TreeFormat format)
        {
            try
            {
                string? path = await electronService.ShowSingleFileOpenDialogAsync();
                if (path is null) return;

                await model.ImportTree(path, format);
            }
            catch (TreeFormatException e)
            {
                await electronService.ShowErrorMessageAsync("ツリーのフォーマットが無効です");
                await Console.Out.WriteLineAsync(e.ToString());
                return;
            }
            catch (Exception e)
            {
                await electronService.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        /// <summary>
        /// エクスポート処理を行います。
        /// </summary>
        /// <param name="format">出力する系統樹のフォーマット</param>
        private async Task ExportAsTreeFile(TreeFormat format)
        {
            string? path = await electronService.ShowFileSaveDialogAsync();
            if (path is null) return;

            try
            {
                await model.ExportCurrentTreeAsTreeFile(path, format);
            }
            catch (Exception e)
            {
                await electronService.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        /// <summary>
        /// エクスポート処理を行います。
        /// </summary>
        /// <param name="exportType">出力するフォーマット</param>
        private async Task ExportWithExporter(ExportType exportType)
        {
            string? path = await electronService.ShowFileSaveDialogAsync(([exportType.ToString().ToLower()], $"{exportType.ToString().ToUpper()} File"));
            if (path is null) return;

            try
            {
                await model.ExportWithExporter(path, exportType);
            }
            catch (Exception e)
            {
                await electronService.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }
    }
}
