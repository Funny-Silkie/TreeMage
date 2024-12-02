using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace TreeViewer.Core.Trees
{
    /// <summary>
    /// クレードを表します。
    /// </summary>
    public class Clade
    {
        /// <summary>
        /// 系統名を取得または設定します。
        /// </summary>
        public string? Taxon { get; set; }

        /// <summary>
        /// 評価値を取得または設定します。
        /// </summary>
        public string? Supports { get; set; }

        /// <summary>
        /// 枝長を取得または設定します。
        /// </summary>
        /// <remarks>枝長がない場合は<see cref="double.NaN"/>が割り当てられます</remarks>
        public double BranchLength { get; set; } = double.NaN;

        /// <summary>
        /// ルートであるかどうかを表す値を取得します。
        /// </summary>
        [MemberNotNullWhen(false, nameof(Parent))]
        public bool IsRoot => Parent is null;

        /// <summary>
        /// リーフであるかどうかを表す値を取得します。
        /// </summary>
        public bool IsLeaf => ChildrenEditable.Count == 0;

        /// <summary>
        /// 親要素を取得します。
        /// </summary>
        public Clade? Parent { get; internal set; }

        /// <summary>
        /// 子要素一覧を取得します。
        /// </summary>
        public ReadOnlyCollection<Clade> Children => _children ??= ChildrenEditable.AsReadOnly();

        private ReadOnlyCollection<Clade>? _children;

        /// <summary>
        /// 子要素一覧を取得します。
        /// </summary>
        internal List<Clade> ChildrenEditable { get; } = [];

        /// <summary>
        /// <see cref="Clade"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal Clade()
        {
        }
    }
}
