using System.Diagnostics;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees.Parsers;

namespace TreeViewer.Core.Trees
{
    /// <summary>
    /// 系統樹を表します。
    /// </summary>
    public class Tree : ICloneable
    {
        /// <summary>
        /// ルートとなる<see cref="Clade"/>のインスタンスを取得します。
        /// </summary>
        public Clade Root { get; private set; }

        /// <summary>
        /// スタイルを取得します。
        /// </summary>
        public TreeStyle Style { get; } = new TreeStyle();

        /// <summary>
        /// unrootedな樹形かどうかを表す値を取得します。
        /// </summary>
        public bool IsUnrooted => Root.ChildrenInternal.Count != 2;

        /// <summary>
        /// <see cref="Tree"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="root">ルートとなる<see cref="Clade"/>のインスタンスs</param>
        /// <exception cref="ArgumentNullException"><paramref name="root"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="root"/>がルートを表していないまたは別のツリーに既に属している</exception>
        public Tree(Clade root)
        {
            ArgumentNullException.ThrowIfNull(root);
            if (!root.IsRoot) throw new ArgumentException("ルートを表していません", nameof(root));
            if (root.Tree is not null) throw new ArgumentException("既に別のツリーに属しています", nameof(root));

            Root = root;
            root.TreeInternal = this;
        }

        /// <summary>
        /// 対象の<see cref="TextReader"/>から系統樹を読み込みます。
        /// </summary>
        /// <param name="reader">使用する<see cref="TextReader"/>のインスタンス</param>
        /// <param name="format">系統樹ファイルのフォーマット</param>
        /// <returns>読み込まれた系統樹一覧</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/>が無効</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>が<see langword="null"/></exception>
        /// <exception cref="TreeFormatException">系統樹のフォーマットが無効</exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="reader"/>が既に破棄されている</exception>
        public static Task<Tree[]> ReadAsync(TextReader reader, TreeFormat format)
        {
            ITreeParser parser = ITreeParser.CreateFromTargetFormat(format);
            return parser.ReadAsync(reader);
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public Tree Clone() => new Tree(Root.Clone(true));

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 属する全ての<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての<see cref="Clade"/>のインスタンス</returns>
        public IEnumerable<Clade> GetAllClades()
        {
            yield return Root;
            foreach (Clade currentClade in Root.GetDescendants()) yield return currentClade;
        }

        /// <summary>
        /// 全ての二分岐を表す<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての二分岐を表す<see cref="Clade"/>のインスタンス一覧</returns>
        public IEnumerable<Clade> GetAllBipartitions() => GetAllClades().Where(x => !x.IsLeaf);

        /// <summary>
        /// 全ての葉を表す<see cref="Clade"/>のインスタンス一覧を取得します。
        /// </summary>
        /// <returns>全ての葉を表す<see cref="Clade"/>のインスタンス一覧</returns>
        public IEnumerable<Clade> GetAllLeaves() => GetAllClades().Where(x => x.IsLeaf);

        /// <summary>
        /// 枝長を全て削除します。
        /// </summary>
        public void ClearAllBranchLengthes()
        {
            foreach (Clade currentClade in GetAllClades()) currentClade.BranchLength = double.NaN;
        }

        /// <summary>
        /// サポート値を全て削除します。
        /// </summary>
        public void ClearAllSupports()
        {
            foreach (Clade currentClade in GetAllBipartitions()) currentClade.Supports = null;
        }

        /// <summary>
        /// 指定したクレードのルートからのインデックスを取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <returns><paramref name="clade"/>のルートからのインデックス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="clade"/>がインスタンスに属していない</exception>
        internal int[] GetIndexes(Clade clade)
        {
            ArgumentNullException.ThrowIfNull(clade);
            if (clade.Tree != this) throw new ArgumentException("インスタンスに属していないクレードです", nameof(clade));

            var list = new List<int>();
            for (Clade current = clade; current.Parent is not null; current = current.Parent) list.Add(current.Parent.ChildrenInternal.IndexOf(current));

            list.Reverse();
            return [.. list];
        }

        /// <summary>
        /// リルートを行います。
        /// </summary>
        /// <param name="clade">リルート対象のクレード</param>
        /// <param name="asRooted">rootedなツリーとしてリルートするかどうかを表す値</param>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="clade"/>がインスタンスに属していない</exception>
        public void Reroot(Clade clade, bool asRooted)
        {
            ArgumentNullException.ThrowIfNull(clade);
            if (clade.Tree != this) throw new ArgumentException("インスタンスに属していないクレードです", nameof(clade));
            if (clade.IsLeaf) throw new ArgumentException("葉を起点にリルートはできません", nameof(clade));

            if (clade.IsRoot)
            {
                if (asRooted == IsUnrooted) throw new ArgumentException("根を起点にリルートできません", nameof(clade));
                return;
            }

            if (IsUnrooted)
            {
                if (asRooted)
                {
                    Clade rootChild1 = CreateClade(clade);
                    Root.TreeInternal = null;
                    rootChild1.BranchLength /= 2;

                    var rootChild2 = new Clade()
                    {
                        Supports = clade.Supports,
                        Taxon = clade.Taxon,
                        BranchLength = clade.BranchLength / 2,
                    };
                    rootChild2.Style.ApplyValues(clade.Style);

                    Clade[] cladeChildren = clade.ChildrenInternal.ToArray();
                    clade.ClearChildren();
                    foreach (Clade currentCladeChild in cladeChildren) rootChild2.AddChild(currentCladeChild);

                    clade.AddChild(rootChild1);
                    clade.AddChild(rootChild2);
                }
                else
                {
                    Clade child = CreateClade(clade);
                    Root.TreeInternal = null;
                    child.Parent = clade;
                    clade.ChildrenInternal.Insert(0, child);
                }

                clade.Parent = null;
                clade.BranchLength = double.NaN;
                clade.Supports = null;
                Root = clade;
                Root.TreeInternal = this;
            }
            else
            {
                // unrootedに変換してからreroot
                Debug.Assert(Root.ChildrenInternal.Count == 2);

                Root.ChildrenInternal[0].BranchLength *= 2;
                Clade rootChild2 = Root.ChildrenInternal[1];
                Root.RemoveChild(rootChild2);

                Clade[] child2Children = [.. rootChild2.ChildrenInternal];
                rootChild2.ClearChildren();
                foreach (Clade currentChild in child2Children) Root.AddChild(currentChild);

                Reroot(clade == rootChild2 ? Root.ChildrenInternal[0] : clade, asRooted);
            }

            // parent と sister を子要素とするクレードを生成
            static Clade CreateClade(Clade target)
            {
                var result = new Clade()
                {
                    Supports = target.Supports,
                    Taxon = target.Taxon,
                    BranchLength = target.BranchLength,
                };
                result.Style.ApplyValues(target.Style);

                if (!target.IsLeaf)
                {
                    if (!target.IsRoot && !target.Parent.IsRoot) result.AddChild(CreateClade(target.Parent));

                    foreach (Clade current in target.Parent!.ChildrenInternal.Where(x => x != target))
                    {
                        current.Parent = null;
                        result.AddChild(current);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// リルートされたツリーを生成します。
        /// </summary>
        /// <param name="clade">リルート対象のクレード</param>
        /// <param name="asRooted">rootedなツリーとしてリルートするかどうかを表す値</param>
        /// <returns>リルート後のツリー</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="clade"/>がインスタンスに属していない</exception>
        public Tree Rerooted(Clade clade, bool asRooted)
        {
            int[] indexes = GetIndexes(clade);

            if (clade.IsLeaf) throw new ArgumentException("葉を起点にリルートはできません", nameof(clade));
            Tree result = Clone();
            Clade nextRoot = result.Root;
            for (int i = 0; i < indexes.Length; i++) nextRoot = nextRoot.ChildrenInternal[indexes[i]];

            result.Reroot(nextRoot, asRooted);
            return result;
        }

        /// <summary>
        /// 指定した姉妹クレード同士を交換します。
        /// </summary>
        /// <param name="target1">入れ替えるクレード1</param>
        /// <param name="target2">入れ替えるクレード2</param>
        /// <exception cref="ArgumentNullException"><paramref name="target1"/>または<paramref name="target2"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item><paramref name="target1"/>と<paramref name="target2"/>が同じインスタンス</item>
        /// <item><paramref name="target1"/>または<paramref name="target2"/>がインスタンスに属していない</item>
        /// <item><paramref name="target1"/>と<paramref name="target2"/>が姉妹でない</item>
        /// </list>
        /// </exception>
        public void SwapSisters(Clade target1, Clade target2)
        {
            ArgumentNullException.ThrowIfNull(target1);
            ArgumentNullException.ThrowIfNull(target2);
            if (ReferenceEquals(target1, target2)) throw new ArgumentException("同じインスタンス同士を交換できません", nameof(target1));
            if (target1.Tree != this) throw new ArgumentException("インスタンスに属していないクレードです", nameof(target1));
            if (target2.Tree != this) throw new ArgumentException("インスタンスに属していないクレードです", nameof(target2));
            if (target1.Parent != target2.Parent) throw new ArgumentException("姉妹クレードではありません", nameof(target1));

            Debug.Assert(target1.Parent is not null);
            Debug.Assert(target2.Parent is not null);

            List<Clade> sisters = target1.Parent.ChildrenInternal;
            int index1 = sisters.IndexOf(target1);
            int index2 = sisters.IndexOf(target2);

            Debug.Assert((uint)index1 < sisters.Count);
            Debug.Assert((uint)index2 < sisters.Count);

            sisters[index2] = target1;
            sisters[index1] = target2;
        }

        /// <summary>
        /// 枝長を基に枝を並び替えます。
        /// </summary>
        /// <param name="descending">降順で並び替えるかどうかを表す値</param>
        public void OrderByLength(bool descending = true)
        {
            OrderByLengthOfClade(Root, descending);

            static void OrderByLengthOfClade(Clade clade, bool descending)
            {
                if (clade.IsLeaf) return;

                clade.ChildrenInternal.Sort((x, y) =>
                {
                    double[] xArray = GetLengthList(x, descending);
                    double[] yArray = GetLengthList(y, descending);

                    for (int i = 0; i < Math.Min(xArray.Length, yArray.Length); i++)
                    {
                        double xCurrent = xArray[i];
                        double yCurrent = yArray[i];
                        int comp = xCurrent.CompareTo(yCurrent);
                        if (comp != 0) return descending ? -comp : comp;
                    }

                    int result = xArray.Length.CompareTo(yArray.Length);
                    return descending ? -result : result;
                });

                foreach (Clade child in clade.ChildrenInternal) OrderByLengthOfClade(child, descending);
            }

            static double[] GetLengthList(Clade clade, bool descending)
            {
                IEnumerable<double> lengthCollection = clade.GetDescendants()
                                                            .Prepend(clade)
                                                            .Where(x => x.IsLeaf)
                                                            .Select(x => x.GetTotalBranchLength(0));
                lengthCollection = descending ? lengthCollection.OrderDescending() : lengthCollection.Order();
                return lengthCollection.ToArray();
            }
        }

        /// <summary>
        /// <see cref="TextWriter"/>を用いて系統樹を出力します。
        /// </summary>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="format">系統樹ファイルのフォーマット</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/>が無効</exception>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>が<see langword="null"/></exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public Task WriteAsync(TextWriter writer, TreeFormat format)
        {
            ITreeParser parser = ITreeParser.CreateFromTargetFormat(format);
            return parser.WriteAsync(writer, this);
        }

        /// <summary>
        /// インスタンスを表す文字列を取得します。
        /// </summary>
        /// <returns>インスタンスを表す文字列</returns>
        public override string ToString()
        {
            using var writer = new StringWriter();
            WriteAsync(writer, TreeFormat.Newick).Wait();
            return writer.ToString();
        }
    }
}
