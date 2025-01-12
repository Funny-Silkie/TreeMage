using Reactive.Bindings;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.ProjectData;
using TreeViewer.Core.Trees;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.Data;
using TreeViewer.Services;
using TreeViewer.Settings;

namespace TreeViewer.Models
{
    /// <summary>
    /// メインのModelのクラスです。
    /// </summary>
    public partial class MainModel : ModelBase
    {
        private readonly UndoService undoService = new UndoService();
        private bool onUndoOperation = false;

        /// <summary>
        /// 読み込まれた系統樹一覧を取得します。
        /// </summary>
        public ReactiveCollection<Tree> Trees { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactiveProperty<Tree?> TargetTree { get; }

        /// <summary>
        /// 選択されているSVG要素のID一覧を取得します。
        /// </summary>
        public HashSet<CladeId> FocusedSvgElementIdList { get; }

        /// <summary>
        /// 開いているプロジェクトファイルのパスのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> ProjectPath { get; }

        /// <summary>
        /// ツリーの更新を通知します。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyTreeUpdated()
        {
            OnPropertyChanged(nameof(TargetTree));
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T> operation, Action<T> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            onUndoOperation = true;

            async Task AsyncOperation(T x)
            {
                onUndoOperation = true;

                operation.Invoke(x);

                onUndoOperation = false;
                await Task.CompletedTask;
            }
            async Task AsyncUndoOperation(T x)
            {
                onUndoOperation = true;

                undoOperation.Invoke(x);

                onUndoOperation = false;
                await Task.CompletedTask;
            }

            try
            {
                operation.Invoke(argument);

                undoService.AddOperation(AsyncUndoOperation, AsyncOperation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T, Tree> operation, Action<T, Tree> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            onUndoOperation = true;

            async Task AsyncOperation(T x)
            {
                onUndoOperation = true;

                operation.Invoke(x, tree);

                onUndoOperation = false;
                await Task.CompletedTask;
            }
            async Task AsyncUndoOperation(T x)
            {
                onUndoOperation = true;

                undoOperation.Invoke(x, tree);

                onUndoOperation = false;
                await Task.CompletedTask;
            }

            try
            {
                operation.Invoke(argument, tree);

                undoService.AddOperation(AsyncUndoOperation, AsyncOperation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public async Task OperateAsUndoable<T>(Func<T, Tree, Task> operation, Func<T, Tree, Task> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            onUndoOperation = true;

            async Task Operation(T argument)
            {
                onUndoOperation = true;

                await operation.Invoke(argument, tree);

                onUndoOperation = false;
            }

            async Task UndoOperation(T argument)
            {
                onUndoOperation = true;

                await undoOperation.Invoke(argument, tree);

                onUndoOperation = false;
            }

            try
            {
                await operation.Invoke(argument, tree);

                undoService.AddOperation(UndoOperation, Operation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// 対象の系統樹にスタイル情報を適用します。
        /// </summary>
        /// <param name="tree">適用する対象</param>
        public void ApplyTreeStyle(Tree tree)
        {
            tree.Style.XScale = XScale.Value;
            tree.Style.YScale = YScale.Value;
            tree.Style.BranchThickness = BranchThickness.Value;
            tree.Style.DefaultBranchLength = DefaultBranchLength.Value;
            tree.Style.ShowLeafLabels = ShowLeafLabels.Value;
            tree.Style.LeafLabelsFontSize = LeafLabelsFontSize.Value;
            tree.Style.ShowCladeLabels = ShowCladeLabels.Value;
            tree.Style.CladeLabelsFontSize = CladeLabelsFontSize.Value;
            tree.Style.CladeLabelsLineThickness = CladeLabelsLineThickness.Value;
            tree.Style.ShowNodeValues = ShowNodeValues.Value;
            tree.Style.NodeValueType = NodeValueType.Value;
            tree.Style.NodeValueFontSize = NodeValueFontSize.Value;
            tree.Style.ShowBranchValues = ShowBranchValues.Value;
            tree.Style.BranchValueType = BranchValueType.Value;
            tree.Style.BranchValueFontSize = BranchValueFontSize.Value;
            tree.Style.BranchValueHideRegexPattern = BranchValueHideRegexPattern.Value;
            tree.Style.ShowBranchDecorations = ShowBranchDecorations.Value;
            tree.Style.DecorationStyles = BranchDecorations.Select(x => x.Style).ToArray();
            tree.Style.ShowScaleBar = ShowScaleBar.Value;
            tree.Style.ScaleBarValue = ScaleBarValue.Value;
            tree.Style.ScaleBarFontSize = ScaleBarFontSize.Value;
            tree.Style.ScaleBarThickness = ScaleBarThickness.Value;
            tree.Style.CollapseType = CollapseType.Value;
            tree.Style.CollapsedConstantWidth = CollapsedConstantWidth.Value;
        }

        /// <summary>
        /// スタイル情報を読み取ります。
        /// </summary>
        /// <param name="tree">読み取るツリー</param>
        public void LoadTreeStyle(Tree tree)
        {
            XScale.Value = tree.Style.XScale;
            YScale.Value = tree.Style.YScale;
            BranchThickness.Value = tree.Style.BranchThickness;
            DefaultBranchLength.Value = tree.Style.DefaultBranchLength;
            ShowLeafLabels.Value = tree.Style.ShowLeafLabels;
            LeafLabelsFontSize.Value = tree.Style.LeafLabelsFontSize;
            ShowCladeLabels.Value = tree.Style.ShowCladeLabels;
            CladeLabelsFontSize.Value = tree.Style.CladeLabelsFontSize;
            CladeLabelsLineThickness.Value = tree.Style.CladeLabelsLineThickness;
            ShowNodeValues.Value = tree.Style.ShowNodeValues;
            NodeValueType.Value = tree.Style.NodeValueType;
            NodeValueFontSize.Value = tree.Style.NodeValueFontSize;
            ShowBranchValues.Value = tree.Style.ShowBranchValues;
            BranchValueType.Value = tree.Style.BranchValueType;
            BranchValueFontSize.Value = tree.Style.BranchValueFontSize;
            BranchValueHideRegexPattern.Value = tree.Style.BranchValueHideRegexPattern;
            ShowBranchDecorations.Value = tree.Style.ShowBranchDecorations;
            BranchDecorations.ClearOnScheduler();
            BranchDecorations.AddRangeOnScheduler(tree.Style.DecorationStyles.Select(x => new BranchDecorationModel(this, x)));
            ShowScaleBar.Value = tree.Style.ShowScaleBar;
            ScaleBarValue.Value = tree.Style.ScaleBarValue;
            ScaleBarFontSize.Value = tree.Style.ScaleBarFontSize;
            ScaleBarThickness.Value = tree.Style.ScaleBarThickness;
            CollapseType.Value = tree.Style.CollapseType;
            CollapsedConstantWidth.Value = tree.Style.CollapsedConstantWidth;
        }

        /// <summary>
        /// undoを行います。
        /// </summary>
        public async Task<bool> Undo() => await undoService.Undo();

        /// <summary>
        /// redoを行います。
        /// </summary>
        public async Task<bool> Redo() => await undoService.Redo();

        /// <summary>
        /// undoのキューをクリアします。
        /// </summary>
        public void ClearUndoQueue() => undoService.Clear();

        /// <summary>
        /// 指定したクレードを選択します。
        /// </summary>
        /// <param name="targetClades">選択するクレード</param>
        public void Focus(params IEnumerable<Clade> targetClades)
        {
            FocusedSvgElementIdList.Clear();

            bool found = false;
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
                found = true;
            }

            if (found) OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// 全ての要素を選択します。
        /// </summary>
        public void FocusAll()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Focus(tree.GetAllClades());
        }

        /// <summary>
        /// 全ての選択を解除します。
        /// </summary>
        public void UnfocusAll()
        {
            FocusedSvgElementIdList.Clear();

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// リルートを行います。
        /// </summary>
        /// <param name="clade">対象クレード</param>
        /// <param name="asRooted">Rootedな系統樹として処理するかどうかを表す値</param>
        public void Reroot(Clade clade, bool asRooted)
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
                NotifyTreeUpdated();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                NotifyTreeUpdated();
            }, (prev: tree, rerooted, targetIndex));
        }

        /// <summary>
        /// 姉妹の入れ替えを行います。
        /// </summary>
        /// <param name="target1">選択したクレード1</param>
        /// <param name="target2">選択したクレード2</param>
        public void SwapSisters(Clade target1, Clade target2)
        {
            if (target1.IsRoot || target2.IsRoot || target1 == target2) return;

            OperateAsUndoable((arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                NotifyTreeUpdated();
            }, (target1, target2));
        }

        /// <summary>
        /// サブツリーの抽出を行います。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        public void ExtractSubtree(Clade clade)
        {
            if (clade.IsRoot || clade.IsLeaf) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            var subtree = new Tree(clade.Clone(true));
            ApplyTreeStyle(subtree);

            OperateAsUndoable((arg, tree) =>
            {
                Trees.InsertOnScheduler(arg.prevIndex + 1, arg.subtree);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 2;
                TargetTree.Value = arg.subtree;
                NotifyTreeUpdated();

                EditMode.Value = TreeEditMode.Select;
                OnPropertyChanged(nameof(EditMode));
            }, (arg, tree) =>
            {
                Trees.RemoveAtOnScheduler(arg.prevIndex + 1);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 1;
                TargetTree.Value = tree;
                NotifyTreeUpdated();

                EditMode.Value = TreeEditMode.Subtree;
                OnPropertyChanged(nameof(EditMode));
            }, (subtree, prevIndex: TreeIndex.Value - 1));
        }

        /// <summary>
        /// 折り畳みを行います。
        /// </summary>
        public void CollapseClade()
        {
            if (FocusedSvgElementIdList.Count != 1 || SelectionTarget.Value is not SelectionMode.Node) return;

            CladeId id = FocusedSvgElementIdList.First();
            if (id.Suffix != CladeIdSuffix.Branch) return;

            Clade clade = id.Clade;
            if (clade.IsLeaf || clade.IsRoot) return;

            bool prevValue = clade.Style.Collapsed;

            OperateAsUndoable((arg, tree) =>
            {
                arg.clade.Style.Collapsed = !prevValue;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                arg.clade.Style.Collapsed = prevValue;

                NotifyTreeUpdated();
            }, (clade, prevValue));
        }

        /// <summary>
        /// 枝長で並び替えます。
        /// </summary>
        /// <param name="descending">降順ソートかどうかを表す値</param>
        public void OrderByBranchLength(bool descending)
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
                NotifyTreeUpdated();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                NotifyTreeUpdated();
            }, (prev: tree, next: oredered, targetIndex));
        }

        /// <summary>
        /// プロジェクトファイルを新規作成します。
        /// </summary>
        public void CreateNew()
        {
            ProjectPath.Value = null;
            UnfocusAll();

            TargetTree.Value = null;
            Trees.ClearOnScheduler();
            TreeIndex.Value = 1;
            undoService.Clear();

            NotifyTreeUpdated();
            OnPropertyChanged(nameof(Trees));
        }

        /// <summary>
        /// プロジェクトファイルを開きます。
        /// </summary>
        /// <param name="path">開くプロジェクトファイルのパス</param>
        public async Task OpenProject(string path)
        {
            ProjectPath.Value = path;

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

                undoService.Clear();

                NotifyTreeUpdated();
                OnPropertyChanged(nameof(Trees));
            }
            catch
            {
                ProjectPath.Value = null;
                throw;
            }
        }

        /// <summary>
        /// プロジェクトファイルを保存します。
        /// </summary>
        /// <param name="path">保存するプロジェクトファイルのパス</param>
        public async Task SaveProject(string path)
        {
            ProjectPath.Value = path;

            if (TargetTree.Value is not null) ApplyTreeStyle(TargetTree.Value);

            var projectData = new ProjectData()
            {
                Trees = [.. Trees],
            };

            await projectData.SaveAsync(path);
        }

        /// <summary>
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="path">読み込む系統樹ファイルのパス</param>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        public async Task ImportTree(string path, TreeFormat format)
        {
            using var reader = new StreamReader(path);
            Tree[] trees = await Tree.ReadAsync(reader, format);

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
                NotifyTreeUpdated();
            }
            else
                OperateAsUndoable(arg =>
                {
                    Trees.AddRangeOnScheduler(arg.trees);
                    OnPropertyChanged(nameof(MaxTreeIndex));

                    TargetTree.Value = trees[0];
                    TreeIndex.Value = arg.addedAt + 1;
                    NotifyTreeUpdated();
                }, arg =>
                {
                    for (int i = arg.addedAt; i < arg.addedAt + arg.trees.Length; i++) Trees.RemoveAtOnScheduler(i);
                    OnPropertyChanged(nameof(MaxTreeIndex));

                    TargetTree.Value = Trees[arg.prevIndex];
                    TreeIndex.Value = arg.prevIndex + 1;
                    NotifyTreeUpdated();
                }, (trees, addedAt: Trees.Count, prevIndex: TreeIndex.Value - 1));
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
        public async Task ExportWithExporter(string path, ExportType type)
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
