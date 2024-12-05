using Reactive.Bindings.Disposables;
using System.ComponentModel;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ViewModelの基底クラスです。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 破棄する要素一覧を取得します。
        /// </summary>
        protected CompositeDisposable Disposables { get; }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// <see cref="ViewModelBase"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ViewModelBase()
        {
            Disposables = [];
        }

        /// <summary>
        /// プロパティが変更された際に実行されます。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        /// <param name="disposing">マネージドソースも破棄するかどうかを表す値</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposables.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
