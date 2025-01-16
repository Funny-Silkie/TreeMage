using System.ComponentModel;

namespace TreeMage.Services
{
    /// <summary>
    /// Undo/Redoを扱うサービスを表します。
    /// </summary>
    public class UndoService : INotifyPropertyChanged
    {
        private readonly Stack<OperationEntry> undoOperations = [];
        private readonly Stack<OperationEntry> redoOperations = [];
        private int maxVersion;
        private int savedVersion;

        /// <summary>
        /// undo可能かどうかを表す値を取得します。
        /// </summary>
        public bool CanUndo => undoOperations.Count > 0;

        /// <summary>
        /// redo可能かどうかを表す値を取得します。
        /// </summary>
        public bool CanRedo => redoOperations.Count > 0;

        /// <summary>
        /// 現在のバージョンを取得します。
        /// </summary>
        private int CurrentVersion => undoOperations.TryPeek(out OperationEntry? entry) ? entry.Version : 0;

        /// <summary>
        /// 現在のバージョンが保存されているかどうかを表す値を取得します。
        /// </summary>
        public bool Saved => savedVersion == CurrentVersion;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// <see cref="UndoService"/>の新しいインスタンスを初期化します。
        /// </summary>
        public UndoService()
        {
        }

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名</param>
        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            OnPropertyChanged(nameof(Saved));

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
            OnPropertyChanged(nameof(Saved));

            return true;
        }

        /// <summary>
        /// 登録されている処理を全て削除します。
        /// </summary>
        public void Clear()
        {
            maxVersion = 0;
            savedVersion = 0;
            undoOperations.Clear();
            redoOperations.Clear();

            OnPropertyChanged(nameof(Saved));
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
            undoOperations.Push(new OperationEntry(x => undoOperation.Invoke((T)x!), x => redoOperation.Invoke((T)x!), argument, ++maxVersion));
            redoOperations.Clear();

            OnPropertyChanged(nameof(Saved));
        }

        /// <summary>
        /// 現在のバージョンで保存されていることを登録します。
        /// </summary>
        public void MarkAsSaved()
        {
            savedVersion = CurrentVersion;
            OnPropertyChanged(nameof(Saved));
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
            /// バージョンを取得します。
            /// </summary>
            public int Version { get; }

            /// <summary>
            /// <see cref="OperationEntry"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="undoAction">Undo時の処理</param>
            /// <param name="redoAction">Rndo時の処理</param>
            /// <param name="argument">処理の引数</param>
            /// <param name="version">バージョン</param>
            public OperationEntry(Func<object?, Task> undoAction, Func<object?, Task> redoAction, object? argument, int version)
            {
                UndoAction = undoAction;
                RedoAction = redoAction;
                Argument = argument;
                Version = version;
            }
        }
    }
}
