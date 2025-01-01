using Reactive.Bindings;
using TreeViewer.Data;

namespace TreeViewer.Models
{
    public partial class MainModel
    {
        /// <summary>
        /// 編集モードのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<TreeEditMode> EditMode { get; }
    }
}
