using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TreeViewer.Core.Styles;

namespace TreeViewer.Core.Trees
{
    /// <summary>
    /// クレードを表します。
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class Clade : ICloneable
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
        public bool IsLeaf => ChildrenInternal.Count == 0;

        /// <summary>
        /// 親要素を取得します。
        /// </summary>
        public Clade? Parent { get; internal set; }

        /// <summary>
        /// スタイルを取得します。
        /// </summary>
        public CladeStyle Style { get; } = new CladeStyle();

        /// <summary>
        /// 子要素一覧を取得します。
        /// </summary>
        public ReadOnlyCollection<Clade> Children => _children ??= ChildrenInternal.AsReadOnly();

        private ReadOnlyCollection<Clade>? _children;

        /// <summary>
        /// 子要素一覧を取得します。
        /// </summary>
        internal List<Clade> ChildrenInternal { get; } = [];

        /// <summary>
        /// インスタンスの所属するツリーを取得します。
        /// </summary>
        public Tree? Tree => FindRoot().TreeInternal;

        /// <summary>
        /// インスタンスの所属するツリーを取得または設定します。
        /// </summary>
        internal Tree? TreeInternal { get; set; }

        /// <summary>
        /// <see cref="Clade"/>の新しいインスタンスを初期化します。
        /// </summary>
        public Clade()
        {
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <param name="onlyDescendants">子孫クレードのみ複製するかどうかを表す値</param>
        /// <returns>インスタンスの複製</returns>
        /// <remarks>戻り値の<see cref="Tree"/>は常に<see langword="null"/>です</remarks>
        public Clade Clone(bool onlyDescendants = false)
        {
            if (onlyDescendants)
            {
                var result = new Clade()
                {
                    Taxon = Taxon,
                    Supports = Supports,
                    BranchLength = BranchLength,
                };

                foreach (Clade currentChild in ChildrenInternal) result.AddChild(currentChild.Clone(true));

                return result;
            }
            else
            {
                var indexes = new Stack<int>();
                for (Clade currentClade = this; !currentClade.IsRoot; currentClade = currentClade.Parent) indexes.Push(currentClade.Parent.ChildrenInternal.IndexOf(currentClade));

                Clade root = FindRoot().Clone(true);

                Clade result = root;
                while (indexes.TryPop(out int index)) result = result.ChildrenInternal[index];

                return result;
            }
        }

        object ICloneable.Clone() => Clone(false);

        /// <summary>
        /// 子クレードを追加します。
        /// </summary>
        /// <param name="child">追加する子クレード</param>
        /// <exception cref="ArgumentNullException"><paramref name="child"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="child"/>が既に別のクレードに属している</exception>
        public void AddChild(Clade child)
        {
            ArgumentNullException.ThrowIfNull(child);
            if (child.Parent is not null) throw new ArgumentException("既に別のクレードに属しています", nameof(child));

            ChildrenInternal.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// 子クレードを全て削除します。
        /// </summary>
        public void ClearChildren()
        {
            foreach (Clade child in ChildrenInternal) child.Parent = null;
            ChildrenInternal.Clear();
        }

        /// <summary>
        /// 子クレードを削除します。
        /// </summary>
        /// <param name="child">削除する子クレード</param>
        /// <returns><paramref name="child"/>を削除できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="child"/>が<see langword="null"/></exception>
        public bool RemoveChild(Clade child)
        {
            ArgumentNullException.ThrowIfNull(child);

            if (!ChildrenInternal.Remove(child)) return false;

            child.Parent = null;
            return true;
        }

        /// <summary>
        /// インスタンスの属するクレードのルートを取得します。
        /// </summary>
        /// <returns>インスタンスの属するクレードのルート</returns>
        public Clade FindRoot()
        {
            Clade result = this;
            while (true)
            {
                if (result.Parent is null) return result;
                result = result.Parent;
            }
        }

        /// <summary>
        /// 子要素以下の<see cref="Clade"/>を再帰的に取得します。
        /// </summary>
        /// <returns>子要素以下の全<see cref="Clade"/>インスタンス</returns>
        public IEnumerable<Clade> GetDescendants()
        {
            if (ChildrenInternal.Count == 0) yield break;

            foreach (Clade currentChild in ChildrenInternal)
            {
                yield return currentChild;
                foreach (Clade currentDescendant in currentChild.GetDescendants()) yield return currentDescendant;
            }
        }

        /// <summary>
        /// ルートからの枝長の合計を取得します。
        /// </summary>
        /// <param name="nanDummy">枝長が<see cref="double.NaN"/>だった際に用いるダミーの値</param>
        /// <returns>ルートからの枝長の合計</returns>
        public double GetTotalBranchLength(double nanDummy = double.NaN)
        {
            double result = 0;
            for (Clade clade = this; clade.Parent is not null; clade = clade.Parent) result += double.IsNaN(clade.BranchLength) ? nanDummy : clade.BranchLength;

            return result;
        }

        /// <summary>
        /// インスタンスと子要素以下の葉の数を取得します。
        /// </summary>
        /// <returns>インスタンスと子要素以下の葉の数</returns>
        public int GetLeavesCount()
        {
            if (IsLeaf) return 1;

            int result = 0;
            foreach (Clade current in GetDescendants())
                if (current.IsLeaf)
                    result++;
            return result;
        }

        /// <summary>
        /// デバッグビューで表示する内容を取得します。
        /// </summary>
        /// <returns>デバッグビューで表示する内容</returns>
        private string GetDebuggerDisplay()
        {
            if (IsRoot) return $"Root (BranchLength={BranchLength}, Supports={Supports})";
            if (IsLeaf) return $"Leaf (Name={Taxon ?? "<null>"}, BranchLength={BranchLength})";
            return $"Clade (BranchLength={BranchLength}, Supports={Supports})";
        }
    }
}
