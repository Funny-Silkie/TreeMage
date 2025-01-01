using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Runtime.CompilerServices;
using TreeViewer.Core.Trees;
using TreeViewer.Services;

namespace TreeViewer.Models
{
    /// <summary>
    /// メインのModelのクラスです。
    /// </summary>
    public class MainModel : ModelBase
    {
#warning TODO: privateに修正
        internal readonly UndoService undoService = new UndoService();
        internal bool onUndoOperation = false;

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

            #region TreeEditSidebar

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

            #endregion TreeEditSidebar
        }

#warning TODO: メソッド名をNotifyTreeChangedに変更

        /// <summary>
        /// 系統樹の再描画をトリガーします。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RequestRerenderTree()
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

        #region TreeEditSidebar

        #region Tree

        /// <summary>
        /// X軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> XScale { get; }

        /// <summary>
        /// Y軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> YScale { get; }

        /// <summary>
        /// 枝の太さのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> BranchThickness { get; }

        #endregion Tree

        #endregion TreeEditSidebar
    }
}
