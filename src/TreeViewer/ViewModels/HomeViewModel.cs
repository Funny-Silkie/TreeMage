using ElectronNET.API.Entities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.ProjectData;
using TreeViewer.Core.Trees;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.Data;
using TreeViewer.Models;
using TreeViewer.Settings;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : WindowViewModel<MainWindow, HomeViewModel>
    {
        private string? projectPath;
        private readonly MainModel model;

        /// <summary>
        /// スタイル編集用のViewModelを取得します。
        /// </summary>
        public StyleSidebarViewModel StyleSidebarViewModel { get; }

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
        public HashSet<CladeId> FocusedSvgElementIdList { get; }

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

        /// <summary>
        /// 検索ワードのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string> SearchQuery { get; }

        /// <summary>
        /// 検索対象を表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<TreeSearchTarget> SearchTarget { get; }

        /// <summary>
        /// 大文字・小文字を無視して検索を行うかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> SearchOnIgnoreCase { get; }

        /// <summary>
        /// 検索に正規表現を使うかどうかを表す値のプロパティを取得します。
        /// </summary>
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
            TreeIndex = new ReactivePropertySlim<int>(1).WithSubscribe(OnTreeIndexChanged)
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
            RerenderTreeCommand = new AsyncReactiveCommand().WithSubscribe(RequestRerenderTree).AddTo(Disposables);
            RerootCommand = new AsyncReactiveCommand<(Clade target, bool asRooted)>().WithSubscribe(x => Reroot(x.target, x.asRooted))
                                                                                     .AddTo(Disposables);
            SwapSisterCommand = new AsyncReactiveCommand<(Clade target1, Clade target2)>().WithSubscribe(x => SwapSisters(x.target1, x.target2))
                                                                                          .AddTo(Disposables);
            ExtractSubtreeCommand = new AsyncReactiveCommand<Clade>().WithSubscribe(ExtractSubtree)
                                                                     .AddTo(Disposables);

            CreateNewCommand = new AsyncReactiveCommand().WithSubscribe(CreateNew)
                                                         .AddTo(Disposables);
            OpenProjectCommand = new AsyncReactiveCommand().WithSubscribe(OpenProject)
                                                           .AddTo(Disposables);
            SaveProjectCommand = new AsyncReactiveCommand<bool>().WithSubscribe(SaveProject)
                                                                 .AddTo(Disposables);
            ExportWithExporterCommand = new AsyncReactiveCommand<(string path, ExportType type)>().WithSubscribe(async x => await ExportWithExporter(x.path, x.type))
                                                                                                  .AddTo(Disposables);
            UndoCommand = new AsyncReactiveCommand().WithSubscribe(model.undoService.Undo)
                                                    .AddTo(Disposables);
            RedoCommand = new AsyncReactiveCommand().WithSubscribe(model.undoService.Redo)
                                                    .AddTo(Disposables);

            FocusedSvgElementIdList = [];
            FocusAllCommand = new AsyncReactiveCommand().WithSubscribe(FocusAll)
                                                          .AddTo(Disposables);
            UnfocusAllCommand = new AsyncReactiveCommand().WithSubscribe(UnfocusAll)
                                                          .AddTo(Disposables);

            SelectionTarget = new ReactivePropertySlim<SelectionMode>(SelectionMode.Node).WithSubscribe(OnSelectionTargetChanged)
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

            SearchQuery = new ReactivePropertySlim<string>(string.Empty).AddTo(Disposables);
            SearchTarget = new ReactivePropertySlim<TreeSearchTarget>().AddTo(Disposables);
            SearchOnIgnoreCase = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            SearchWithRegex = new ReactivePropertySlim<bool>().AddTo(Disposables);
            SearchCommand = new AsyncReactiveCommand().WithSubscribe(Search)
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

            StyleSidebarViewModel = new StyleSidebarViewModel(this);

            model.undoService.Clear();
        }

        /// <summary>
        /// 系統樹の再描画をトリガーします。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RequestRerenderTree()
        {
            OnPropertyChanged(nameof(TargetTree));
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="model.undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T> operation, Action<T> undoOperation, T argument)
        {
            model.OperateAsUndoable(operation, undoOperation, argument);
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="model.undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T, Tree> operation, Action<T, Tree> undoOperation, T argument)
        {
            model.OperateAsUndoable(operation, undoOperation, argument);
        }

        /// <inheritdoc cref="MainModel.ApplyTreeStyle(Tree)"/>
        private void ApplyTreeStyle(Tree tree) => model.ApplyTreeStyle(tree);

        /// <inheritdoc cref="MainModel.LoadTreeStyle(Tree)"/>
        private void LoadTreeStyle(Tree tree) => model.LoadTreeStyle(tree);

        /// <summary>
        /// <see cref="TreeIndex"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnTreeIndexChanged(int value)
        {
            if (model.onUndoOperation) return;

            value--;
            if ((uint)value >= (uint)Trees.Count) return;

            Tree? prevTree = TargetTree.Value;
            Tree nextTree = Trees[value];
            if (prevTree is null)
            {
                TargetTree.Value = Trees[value];
                LoadTreeStyle(nextTree);
            }
            else
            {
                int prevIndex = Trees.IndexOf(prevTree);

                OperateAsUndoable(arg =>
                {
                    ApplyTreeStyle(arg.prevTree);

                    UnfocusAll();
                    TargetTree.Value = arg.nextTree;

                    LoadTreeStyle(arg.nextTree);

                    TreeIndex.Value = arg.nextIndex + 1;
                    RequestRerenderTree();
                }, arg =>
                {
                    ApplyTreeStyle(arg.nextTree);

                    UnfocusAll();
                    TargetTree.Value = arg.prevTree;

                    LoadTreeStyle(arg.prevTree);

                    TreeIndex.Value = arg.prevIndex + 1;
                    RequestRerenderTree();
                }, (prevTree, nextTree, prevIndex, nextIndex: value));
            }
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
                                Focus(id.Clade);
                            }
                            break;

                        case SelectionMode.Clade:
                            {
                                Clade target = id.Clade;
                                Focus(target.GetDescendants().Prepend(target));
                            }
                            break;

                        case SelectionMode.Taxa:
                            {
                                Clade target = id.Clade;
                                if (target.IsLeaf) Focus(target);
                                else Focus(target.GetDescendants().Where(x => x.IsLeaf));
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

        /// <summary>
        /// 指定したクレードを選択します。
        /// </summary>
        /// <param name="targetClades">選択するクレード</param>
        private void Focus(params IEnumerable<Clade> targetClades)
        {
            FocusedSvgElementIdList.Clear();

            foreach (Clade current in targetClades)
            {
                CladeIdSuffix idSuffix;
                switch (SelectionTarget.Value)
                {
                    case SelectionMode.Node:
                    case SelectionMode.Clade:
                        idSuffix = CladeIdSuffix.Branch;
                        break;

                    case SelectionMode.Taxa:
                        idSuffix = CladeIdSuffix.Leaf;
                        break;

                    default: continue;
                }

                FocusedSvgElementIdList.Add(current.GetId(idSuffix));
            }

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// 全ての要素を選択します。
        /// </summary>
        private void FocusAll()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Focus(tree.GetAllClades());
        }

        /// <summary>
        /// 全ての選択を解除します。
        /// </summary>
        private void UnfocusAll()
        {
            FocusedSvgElementIdList.Clear();

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// <see cref="SelectionTarget"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnSelectionTargetChanged(SelectionMode value)
        {
            if (FocusedSvgElementIdList.Count == 0) return;

            HashSet<Clade> selectedClades = FocusedSvgElementIdList.Select(x => x.Clade)
                                                                   .ToHashSet();

            switch (value)
            {
                case SelectionMode.Node:
                    Focus(selectedClades);
                    break;

                case SelectionMode.Clade:
                    Focus(selectedClades.SelectMany(x => x.GetDescendants().Prepend(x)));
                    break;

                case SelectionMode.Taxa:
                    Focus(selectedClades.SelectMany(x => x.GetDescendants().Prepend(x)));
                    break;
            }
        }

        /// <summary>
        /// リルートを行います。
        /// </summary>
        /// <param name="clade">対象クレード</param>
        /// <param name="asRooted">Rootedな系統樹として処理するかどうかを表す値</param>
        private void Reroot(Clade clade, bool asRooted)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null || clade.IsLeaf || clade.Tree != tree) return;

            Tree rerooted = tree.Rerooted(clade, asRooted);
            ApplyTreeStyle(rerooted);
            int targetIndex = TreeIndex.Value - 1;

            OperateAsUndoable(arg =>
            {
                TargetTree.Value = arg.rerooted;
                Trees[arg.targetIndex] = arg.rerooted;

                UnfocusAll();
                RequestRerenderTree();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                RequestRerenderTree();
            }, (prev: tree, rerooted, targetIndex));
        }

        /// <summary>
        /// 姉妹の入れ替えを行います。
        /// </summary>
        /// <param name="target1">選択したクレード1</param>
        /// <param name="target2">選択したクレード2</param>
        private void SwapSisters(Clade target1, Clade target2)
        {
            if (target1.IsRoot || target2.IsRoot || target1 == target2) return;

            OperateAsUndoable((arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                RequestRerenderTree();
            }, (target1, target2));
        }

        /// <summary>
        /// サブツリーの抽出を行います。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        private void ExtractSubtree(Clade clade)
        {
            if (clade.IsRoot || clade.IsLeaf) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            var subtree = new Tree(clade.Clone(true));
            ApplyTreeStyle(tree);

            OperateAsUndoable((arg, tree) =>
            {
                Trees.InsertOnScheduler(arg.prevIndex + 1, arg.subtree);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 2;
                TargetTree.Value = arg.subtree;
                RequestRerenderTree();

                EditMode.Value = TreeEditMode.Select;
                OnPropertyChanged(nameof(EditMode));
            }, (arg, tree) =>
            {
                Trees.RemoveAtOnScheduler(arg.prevIndex + 1);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 1;
                TargetTree.Value = tree;
                RequestRerenderTree();

                EditMode.Value = TreeEditMode.Subtree;
                OnPropertyChanged(nameof(EditMode));
            }, (subtree, prevIndex: TreeIndex.Value - 1));
        }

        /// <summary>
        /// 折り畳みを行います。
        /// </summary>
        private void CollapseClade()
        {
            if (FocusedSvgElementIdList.Count != 1 || SelectionTarget.Value is not SelectionMode.Node) return;

            try
            {
                CladeId id = FocusedSvgElementIdList.First();
                if (id.Suffix != CladeIdSuffix.Branch) return;

                Clade clade = id.Clade;
                if (clade.IsLeaf || clade.IsRoot) return;

                bool prevValue = clade.Style.Collapsed;

                OperateAsUndoable((arg, tree) =>
                {
                    arg.clade.Style.Collapsed = !prevValue;

                    RequestRerenderTree();
                }, (arg, tree) =>
                {
                    arg.clade.Style.Collapsed = prevValue;

                    RequestRerenderTree();
                }, (clade, prevValue));
            }
            catch (Exception e)
            {
                Window.ShowErrorMessageAsync(e).Wait();
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 枝長で並び替えます。
        /// </summary>
        /// <param name="descending">降順ソートかどうかを表す値</param>
        private void OrderByBranchLength(bool descending)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Tree oredered = tree.Clone();
            oredered.OrderByLength(descending);
            ApplyTreeStyle(oredered);

            int targetIndex = TreeIndex.Value - 1;

            OperateAsUndoable(arg =>
            {
                TargetTree.Value = arg.next;
                Trees[arg.targetIndex] = arg.next;

                UnfocusAll();
                RequestRerenderTree();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                RequestRerenderTree();
            }, (prev: tree, next: oredered, targetIndex));
        }

        /// <summary>
        /// 検索を実行します。
        /// </summary>
        private void Search()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            string query = SearchQuery.Value;
            if (string.IsNullOrEmpty(query)) return;

            Func<Clade, string?> cladeConverter;
            switch (SearchTarget.Value)
            {
                case TreeSearchTarget.Taxon:
                    SelectionTarget.Value = SelectionMode.Taxa;
                    cladeConverter = x => x.Taxon;
                    break;

                case TreeSearchTarget.Supports:
                    SelectionTarget.Value = SelectionMode.Node;
                    cladeConverter = x => x.Supports;
                    break;

                default: return;
            }

            Predicate<string> cladeSelection;
            if (SearchWithRegex.Value)
            {
                Regex regex;
                var option = RegexOptions.None;
                if (SearchOnIgnoreCase.Value) option |= RegexOptions.IgnoreCase;
                try
                {
                    regex = new Regex(query, option);
                }
                catch
                {
                    return;
                }
                cladeSelection = x => regex.IsMatch(x);
            }
            else
            {
                StringComparison stringComparison = SearchOnIgnoreCase.Value ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                cladeSelection = x => x.Contains(query, stringComparison);
            }

            IEnumerable<Clade> targetClades = tree.GetAllClades()
                                                  .Select(x => (clade: x, value: cladeConverter.Invoke(x)))
                                                  .Where(x => !string.IsNullOrEmpty(x.value) && cladeSelection.Invoke(x.value))
                                                  .Select(x => x.clade);
            Focus(targetClades);
        }

        /// <summary>
        /// プロジェクトファイルを新規作成します。
        /// </summary>
        private void CreateNew()
        {
            projectPath = null;
            UnfocusAll();

            TargetTree.Value = null;
            Trees.ClearOnScheduler();
            TreeIndex.Value = 1;
            model.undoService.Clear();

            RequestRerenderTree();
            OnPropertyChanged(nameof(Trees));
        }

        /// <summary>
        /// プロジェクトファイルを開きます。
        /// </summary>
        private async Task OpenProject()
        {
            string? path = await Window.ShowSingleFileOpenDialog([new FileFilter()
            {
                Name = "Tree viewer project file",
                Extensions = ["treeprj"],
            }]);
            if (path is null) return;

            projectPath = path;

            try
            {
                ProjectData data = await ProjectData.LoadAsync(path);

                UnfocusAll();
                Trees.ClearOnScheduler();
                Trees.AddRangeOnScheduler(data.Trees);

                TargetTree.Value = null;

                TreeIndex.Value = 1;
                if (data.Trees.Length > 0)
                {
                    Tree mainTree = data.Trees[0];
                    TargetTree.Value = mainTree;
                    LoadTreeStyle(mainTree);
                }

                model.undoService.Clear();

                RequestRerenderTree();
                OnPropertyChanged(nameof(Trees));
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
                projectPath = null;
            }
        }

        /// <summary>
        /// プロジェクトファイルを保存します。
        /// </summary>
        /// <param name="asNew">新しいファイルとして保存するかどうかを表す値</param>
        private async Task SaveProject(bool asNew)
        {
            if (asNew || projectPath is null)
            {
                string? selectedPath = await Window.ShowFileSaveDialog([new FileFilter()
                {
                    Name = "Tree viewer project file",
                    Extensions = ["treeprj"],
                }]);

                if (selectedPath is null) return;
                projectPath = selectedPath;
            }

            if (TargetTree.Value is not null) ApplyTreeStyle(TargetTree.Value);

            var projectData = new ProjectData()
            {
                Trees = [.. Trees],
            };

            try
            {
                await projectData.SaveAsync(projectPath);
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <summary>
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="path">読み込む系統樹ファイルのパス</param>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        public async Task ImportTree(string path, TreeFormat format)
        {
            try
            {
                using var reader = new StreamReader(path);
                Tree[] trees;
                try
                {
                    trees = await Tree.ReadAsync(reader, format);
                }
                catch (TreeFormatException e)
                {
                    await Window.ShowErrorMessageAsync("ツリーのフォーマットが無効です");
                    await Console.Out.WriteLineAsync(e.ToString());
                    return;
                }

                if (trees.Length == 0) return;
                Configurations config = await Configurations.LoadOrCreateAsync();

                for (int i = 0; i < trees.Length; i++)
                {
                    Tree tree = trees[i];

                    ApplyTreeStyle(tree);
                    switch (config.AutoOrderingMode)
                    {
                        case AutoOrderingMode.Ascending:
                            tree.OrderByLength(false);
                            break;

                        case AutoOrderingMode.Descending:
                            tree.OrderByLength(true);
                            break;
                    }
                }

                if (Trees.Count == 0)
                {
                    Trees.AddRangeOnScheduler(trees);

                    TargetTree.Value = trees[0];
                    RequestRerenderTree();
                }
                else
                    OperateAsUndoable(arg =>
                    {
                        Trees.AddRangeOnScheduler(arg.trees);
                        OnPropertyChanged(nameof(MaxTreeIndex));

                        TargetTree.Value = trees[0];
                        TreeIndex.Value = arg.addedAt + 1;
                        RequestRerenderTree();
                    }, arg =>
                    {
                        for (int i = arg.addedAt; i < arg.addedAt + arg.trees.Length; i++) Trees.RemoveAtOnScheduler(i);
                        OnPropertyChanged(nameof(MaxTreeIndex));

                        TargetTree.Value = Trees[arg.prevIndex];
                        TreeIndex.Value = arg.prevIndex + 1;
                        RequestRerenderTree();
                    }, (trees, addedAt: Trees.Count, prevIndex: TreeIndex.Value - 1));
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        /// <summary>
        /// 表示している系統樹を出力します。
        /// </summary>
        /// <param name="path">出力する系統樹ファイルのパス</param>
        /// <param name="format">出力する系統樹のフォーマット</param>
        public async Task ExportCurrentTreeAsTreeFile(string path, TreeFormat format)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            using var writer = new StreamWriter(path);
            await tree.WriteAsync(writer, format);
        }

        /// <summary>
        /// <see cref="IExporter"/>によるエクスポートを行います。
        /// </summary>
        /// <param name="path">出力先のファイルパス</param>
        /// <param name="type">エクスポートのフォーマット</param>
        private async Task ExportWithExporter(string path, ExportType type)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            ApplyTreeStyle(tree);

            IExporter exporter = IExporter.Create(type);
            using var stream = new FileStream(path, FileMode.Create);
            await exporter.ExportAsync(tree, stream, (await Configurations.LoadOrCreateAsync()).ToExportOptions());
        }
    }
}
