namespace TreeViewer.Services
{
    /// <summary>
    /// Undo/Redoを扱うサービスを表します。
    /// </summary>
    public class UndoService
    {
        private readonly Stack<OperationEntry> undoOperations = [];
        private readonly Stack<OperationEntry> redoOperations = [];

        /// <summary>
        /// undo可能かどうかを表す値を取得します。
        /// </summary>
        public bool CanUndo => undoOperations.Count > 0;

        /// <summary>
        /// redo可能かどうかを表す値を取得します。
        /// </summary>
        public bool CanRedo => redoOperations.Count > 0;

        /// <summary>
        /// <see cref="UndoService"/>の新しいインスタンスを初期化します。
        /// </summary>
        public UndoService()
        {
        }

        /// <summary>
        /// undo処理を実行します。
        /// </summary>
        /// <returns>undo可能な処理が存在したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public async Task<bool> Undo()
        {
            if (!undoOperations.TryPop(out OperationEntry? operation)) return false;

            await operation.UndoAction.Invoke(operation.Argument);
            redoOperations.Push(operation);

            return true;
        }

        /// <summary>
        /// redo処理を実行します。
        /// </summary>
        /// <returns>redo可能な処理が存在したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public async Task<bool> Redo()
        {
            if (!redoOperations.TryPop(out OperationEntry? operation)) return false;

            await operation.RedoAction.Invoke(operation.Argument);
            undoOperations.Push(operation);

            return true;
        }

        /// <summary>
        /// 登録されている処理を全て削除します。
        /// </summary>
        public void Clear()
        {
            undoOperations.Clear();
            redoOperations.Clear();
        }

        /// <summary>
        /// 処理を追加します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="undoOperation">undo用の処理</param>
        /// <param name="redoOperation">redo用の処理</param>
        /// <param name="argument">処理の引数</param>
        public void AddOperation<T>(Func<T, Task> undoOperation, Func<T, Task> redoOperation, T argument)
        {
            undoOperations.Push(new OperationEntry(x => undoOperation.Invoke((T)x!), x => redoOperation.Invoke((T)x!), argument));
            redoOperations.Clear();
        }

        /// <summary>
        /// 保管しておく処理を表します。
        /// </summary>
        internal sealed class OperationEntry
        {
            /// <summary>
            /// Undo時の処理を取得します。
            /// </summary>
            public Func<object?, Task> UndoAction { get; }

            /// <summary>
            /// Redo時の処理を取得します。
            /// </summary>
            public Func<object?, Task> RedoAction { get; }

            /// <summary>
            /// 処理の引数を取得します。
            /// </summary>
            public object? Argument { get; }

            /// <summary>
            /// <see cref="OperationEntry"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="undoAction">Undo時の処理</param>
            /// <param name="redoAction">Rndo時の処理</param>
            /// <param name="argument">処理の引数</param>
            public OperationEntry(Func<object?, Task> undoAction, Func<object?, Task> redoAction, object? argument)
            {
                UndoAction = undoAction;
                RedoAction = redoAction;
                Argument = argument;
            }
        }
    }
}
