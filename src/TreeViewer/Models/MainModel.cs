using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Trees;
using TreeViewer.Data;
using TreeViewer.Services;

namespace TreeViewer.Models
{
    /// <summary>
    /// メインのModelのクラスです。
    /// </summary>
    public partial class MainModel : ModelBase
    {
#warning TODO: privateに修正
        internal UndoService undoService { get; } = new UndoService();
        internal bool onUndoOperation { get; private set; } = false;

        /// <summary>
        /// 読み込まれた系統樹一覧を取得します。
        /// </summary>
        public ReactiveCollection<Tree> Trees { get; }

        /// <summary>
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> MaxTreeIndex { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactiveProperty<Tree?> TargetTree { get; }

        /// <summary>
        /// <see cref="MainModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainModel()
        {
            Trees = new ReactiveCollection<Tree>().AddTo(Disposables);
            MaxTreeIndex = new ReactiveProperty<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = new ReactiveProperty<Tree?>().AddTo(Disposables);

            #region Header

            EditMode = new ReactiveProperty<TreeEditMode>().AddTo(Disposables);
            EditMode.Zip(EditMode.Skip(1), (x, y) => (before: x, after: y)).Subscribe(v => OperateAsUndoable((arg, tree) =>
            {
                EditMode.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                EditMode.Value = arg.before;

                RequestRerenderTree();
            }, v));

            #endregion Header

            #region TreeEditSidebar

            #region Layout

            CollapseType = new ReactiveProperty<CladeCollapseType>(CladeCollapseType.TopMax).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapseType = arg.after;
                CollapseType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CollapseType = arg.before;
                CollapseType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CollapseType ?? CladeCollapseType.TopMax, after: v))).AddTo(Disposables);
            CollapsedConstantWidth = new ReactiveProperty<double>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.after;
                CollapsedConstantWidth!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.before;
                CollapsedConstantWidth!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CollapsedConstantWidth ?? 1, after: v))).AddTo(Disposables);

            #endregion Layout

            #region Tree

            XScale = new ReactiveProperty<int>(300).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.XScale = arg.after;
                XScale!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.XScale = arg.before;
                XScale!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.XScale ?? 0, after: v))).AddTo(Disposables);
            YScale = new ReactiveProperty<int>(30).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.YScale = arg.after;
                YScale!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.YScale = arg.before;
                YScale!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.YScale ?? 0, after: v))).AddTo(Disposables);
            BranchThickness = new ReactiveProperty<int>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchThickness = arg.after;
                BranchThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchThickness = arg.before;
                BranchThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchThickness ?? 0, after: v))).AddTo(Disposables);

            #endregion Tree

            #region LeafLabels

            ShowLeafLabels = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.after;
                ShowLeafLabels!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.before;
                ShowLeafLabels!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowLeafLabels ?? false, after: v))).AddTo(Disposables);
            LeafLabelsFontSize = new ReactiveProperty<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.after;
                LeafLabelsFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.before;
                LeafLabelsFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.LeafLabelsFontSize ?? 0, after: v))).AddTo(Disposables);

            #endregion LeafLabels

            #region CladeLabels

            ShowCladeLabels = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.after;
                ShowCladeLabels!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.before;
                ShowCladeLabels!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowCladeLabels ?? false, after: v))).AddTo(Disposables);
            CladeLabelsFontSize = new ReactiveProperty<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.after;
                CladeLabelsFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.before;
                CladeLabelsFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CladeLabelsFontSize ?? 0, after: v))).AddTo(Disposables);
            CladeLabelsLineThickness = new ReactiveProperty<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.after;
                CladeLabelsLineThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.before;
                CladeLabelsLineThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CladeLabelLineThickness ?? 0, after: v)));

            #endregion CladeLabels

            #region NodeValues

            ShowNodeValues = new ReactiveProperty<bool>(false).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.after;
                ShowNodeValues!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.before;
                ShowNodeValues!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowNodeValues ?? false, after: v))).AddTo(Disposables);
            NodeValueType = new ReactiveProperty<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueType = arg.after;
                NodeValueType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueType = arg.before;
                NodeValueType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.NodeValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            NodeValueFontSize = new ReactiveProperty<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.after;
                NodeValueFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.before;
                NodeValueFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.NodeValueFontSize ?? 0, after: v))).AddTo(Disposables);

            #endregion NodeValues

            #region BranchValues

            ShowBranchValues = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.after;
                ShowBranchValues!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.before;
                ShowBranchValues!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowBranchValues ?? false, after: v))).AddTo(Disposables);
            BranchValueType = new ReactiveProperty<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueType = arg.after;
                BranchValueType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueType = arg.before;
                BranchValueType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            BranchValueFontSize = new ReactiveProperty<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.after;
                BranchValueFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.before;
                BranchValueFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueFontSize ?? 0, after: v))).AddTo(Disposables);
            BranchValueHideRegexPattern = new ReactiveProperty<string?>().WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.after;
                BranchValueHideRegexPattern!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.before;
                BranchValueHideRegexPattern!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueHideRegexPattern, after: v))).AddTo(Disposables);

            #endregion BranchValues

            #region BranchDecorations

            ShowBranchDecorations = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.after;
                ShowBranchDecorations!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.before;
                ShowBranchDecorations!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowBranchDecorations ?? false, after: v))).AddTo(Disposables);
            BranchDecorations = new ReactiveCollection<BranchDecorationModel>().AddTo(Disposables);

            #endregion BranchDecorations

            #region ScaleBar

            ShowScaleBar = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.after;
                ShowScaleBar!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.before;
                ShowScaleBar!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowScaleBar ?? false, after: v))).AddTo(Disposables);
            ScaleBarValue = new ReactiveProperty<double>(0.1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.after;
                ScaleBarValue!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.before;
                ScaleBarValue!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarValue ?? 0.1, after: v))).AddTo(Disposables);
            ScaleBarFontSize = new ReactiveProperty<int>(25).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.after;
                ScaleBarFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.before;
                ScaleBarFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarFontSize ?? 0, after: v))).AddTo(Disposables);
            ScaleBarThickness = new ReactiveProperty<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.after;
                ScaleBarThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.before;
                ScaleBarThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarThickness ?? 0, after: v))).AddTo(Disposables);

            #endregion ScaleBar

            #endregion TreeEditSidebar
        }

#warning TODO: メソッド名をNotifyTreeChangedに変更

        /// <summary>
        /// 系統樹の再描画をトリガーします。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RequestRerenderTree()
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
        /// undo/redo可能な処理を実行し，<see cref="model.undoService"/>に登録します。
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
            tree.Style.ShowLeafLabels = ShowLeafLabels.Value;
            tree.Style.LeafLabelsFontSize = LeafLabelsFontSize.Value;
            tree.Style.ShowCladeLabels = ShowCladeLabels.Value;
            tree.Style.CladeLabelsFontSize = CladeLabelsFontSize.Value;
            tree.Style.CladeLabelLineThickness = CladeLabelsLineThickness.Value;
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
            ShowLeafLabels.Value = tree.Style.ShowLeafLabels;
            LeafLabelsFontSize.Value = tree.Style.LeafLabelsFontSize;
            ShowCladeLabels.Value = tree.Style.ShowCladeLabels;
            CladeLabelsFontSize.Value = tree.Style.CladeLabelsFontSize;
            CladeLabelsLineThickness.Value = tree.Style.CladeLabelLineThickness;
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
    }
}
