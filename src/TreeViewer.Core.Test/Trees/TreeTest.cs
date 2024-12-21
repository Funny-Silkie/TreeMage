using TreeViewer.TestUtilities.Assertions;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees.Parsers;

namespace TreeViewer.Core.Trees
{
    public class TreeTest
    {
        private readonly Clade root;
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade cladeBA;
        private readonly Clade leafBAA;
        private readonly Clade leafBAB;
        private readonly Clade cladeBB;
        private readonly Clade cladeBBA;
        private readonly Clade leafBBAA;
        private readonly Clade leafBBAB;
        private readonly Clade leafBBB;
        private readonly Clade leafC;
        private readonly Tree tree;

        public TreeTest()
        {
            tree = CreateDummyTree(out root, out leafA, out cladeB, out cladeBA, out leafBAA, out leafBAB, out cladeBB, out cladeBBA, out leafBBAA, out leafBBAB, out leafBBB, out leafC);
        }

        /// <inheritdoc cref="CreateDummyTree(out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade)"/>
        public static Tree CreateDummyTree()
        {
            return CreateDummyTree(out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        }

        /// <summary>
        /// ダミーの系統樹を生成します。
        /// </summary>
        /// <param name="root">根を表す<see cref="Clade"/>インスタンス</param>
        /// <param name="leafA"></param>
        /// <param name="cladeB"></param>
        /// <param name="cladeBA"></param>
        /// <param name="leafBAA"></param>
        /// <param name="leafBAB"></param>
        /// <param name="cladeBB"></param>
        /// <param name="cladeBBA"></param>
        /// <param name="leafBBAA"></param>
        /// <param name="leafBBAB"></param>
        /// <param name="leafBBB"></param>
        /// <param name="leafC"></param>
        /// <returns>ダミーの系統樹</returns>
        /// <remarks>
        /// <code>
        ///          2                              5
        ///      +------leafA    1         +---------------leafBAA
        /// root-|             +---cladeBA-|20/30
        ///      |             |           +---------leafBAB
        ///      |   2         |                 3            2
        ///      +------cladeB-|30/45           1          +------leafBBAA
        ///      |             |   2          +---cladeBBA-|85/95
        ///      |             +------cladeBB-|100/100     +---leafBBAB
        ///      | 1                          |    3         1
        ///      +---leafC                    +---------leafBBB
        /// </code>
        /// </remarks>
        public static Tree CreateDummyTree(out Clade root,
                                           out Clade leafA,
                                           out Clade cladeB,
                                           out Clade cladeBA,
                                           out Clade leafBAA,
                                           out Clade leafBAB,
                                           out Clade cladeBB,
                                           out Clade cladeBBA,
                                           out Clade leafBBAA,
                                           out Clade leafBBAB,
                                           out Clade leafBBB,
                                           out Clade leafC)
        {
            root = new Clade();

            leafA = new Clade()
            {
                Taxon = "A",
                BranchLength = 2,
                Parent = root,
            };
            cladeB = new Clade()
            {
                Supports = "30/45",
                BranchLength = 2,
                Parent = root,
            };
            leafC = new Clade()
            {
                Taxon = "C",
                BranchLength = 1,
                Parent = root,
            };
            root.ChildrenInternal.AddRange(leafA, cladeB, leafC);

            cladeBA = new Clade()
            {
                Supports = "20/30",
                BranchLength = 1,
                Parent = cladeB,
            };
            cladeBB = new Clade()
            {
                BranchLength = 2,
                Supports = "100/100",
                Parent = cladeB,
            };
            cladeB.ChildrenInternal.AddRange(cladeBA, cladeBB);

            leafBAA = new Clade()
            {
                Taxon = "BAA",
                BranchLength = 5,
                Parent = cladeBA,
            };
            leafBAB = new Clade()
            {
                Taxon = "BAB",
                BranchLength = 3,
                Parent = cladeBA,
            };
            cladeBA.ChildrenInternal.AddRange(leafBAA, leafBAB);

            cladeBBA = new Clade()
            {
                BranchLength = 1,
                Supports = "85/95",
                Parent = cladeBB,
            };
            leafBBB = new Clade()
            {
                Taxon = "BBB",
                BranchLength = 3,
                Parent = cladeBB,
            };
            cladeBB.ChildrenInternal.AddRange(cladeBBA, leafBBB);

            leafBBAA = new Clade()
            {
                Taxon = "BBAA",
                BranchLength = 2,
                Parent = cladeBBA,
            };
            leafBBAB = new Clade()
            {
                Taxon = "BBAB",
                BranchLength = 1,
                Parent = cladeBBA,
            };
            cladeBBA.ChildrenInternal.AddRange(leafBBAA, leafBBAB);

            return new Tree(root);
        }

        #region Ctors

        [Fact]
        public void Ctor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Tree(null!));
        }

        [Fact]
        public void Ctor_WithNonRootClade()
        {
            root.TreeInternal = null;

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new Tree(cladeB));
                Assert.Throws<ArgumentException>(() => new Tree(leafA));
                Assert.Throws<ArgumentException>(() => new Tree(cladeBA));
                Assert.Throws<ArgumentException>(() => new Tree(leafBAA));
                Assert.Throws<ArgumentException>(() => new Tree(leafBAB));
                Assert.Throws<ArgumentException>(() => new Tree(cladeBB));
                Assert.Throws<ArgumentException>(() => new Tree(cladeBBA));
                Assert.Throws<ArgumentException>(() => new Tree(leafBBAA));
                Assert.Throws<ArgumentException>(() => new Tree(leafBBAB));
                Assert.Throws<ArgumentException>(() => new Tree(leafBBB));
                Assert.Throws<ArgumentException>(() => new Tree(leafC));
            });
        }

        [Fact]
        public void Ctor_WithCladeBelongingToOtherTree()
        {
            Assert.Throws<ArgumentException>(() => new Tree(root));
        }

        [Fact]
        public void Ctor_AsPositive()
        {
            root.TreeInternal = null;

            var tree = new Tree(root);

            Assert.Multiple(() =>
            {
                Assert.Same(root, tree.Root);
                Assert.Same(tree, root.Tree);
                CustomizedAssertions.Equal(new TreeStyle(), tree.Style);
            });
        }

        #endregion Ctors

        #region Static Methods

        [Fact]
        public async Task ReadAsync()
        {
            using var reader = new StringReader("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);");
            Tree[] trees = await Tree.ReadAsync(reader, TreeFormat.Newick);

            Assert.Multiple(() =>
            {
                Assert.Single(trees);
                CustomizedAssertions.Equal(CreateDummyTree().Root, trees[0].Root);
                CustomizedAssertions.Equal(new TreeStyle(), trees[0].Style);
            });
        }

        #endregion Static Methods

        #region Instance Methods

        [Fact]
        public void Clone()
        {
            Tree cloned = tree.Clone();

            CustomizedAssertions.Equal(tree, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            var cloned = (Tree)((ICloneable)tree).Clone();

            CustomizedAssertions.Equal(tree, cloned);
        }

        [Fact]
        public void GetAllClades()
        {
            IEnumerable<Clade> clades = tree.GetAllClades();

            Assert.Equal([root, leafA, cladeB, cladeBA, leafBAA, leafBAB, cladeBB, cladeBBA, leafBBAA, leafBBAB, leafBBB, leafC], clades);
        }

        [Fact]
        public void GetAllBipartitions()
        {
            IEnumerable<Clade> bipartitions = tree.GetAllBipartitions();

            Assert.Equal([root, cladeB, cladeBA, cladeBB, cladeBBA], bipartitions);
        }

        [Fact]
        public void GetAllLeaves()
        {
            IEnumerable<Clade> leaves = tree.GetAllLeaves();

            Assert.Equal([leafA, leafBAA, leafBAB, leafBBAA, leafBBAB, leafBBB, leafC], leaves);
        }

        [Fact]
        public void ClearAllBranchLengthes()
        {
            tree.ClearAllBranchLengthes();

            Assert.Multiple(() =>
            {
                foreach (Clade current in tree.GetAllClades()) Assert.Equal(double.NaN, current.BranchLength);
            });
        }

        [Fact]
        public void ClearAllSupports()
        {
            tree.ClearAllSupports();

            Assert.Multiple(() =>
            {
                foreach (Clade current in tree.GetAllBipartitions()) Assert.Null(current.Supports);
            });
        }

        [Fact]
        public void GetIndexes_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => tree.GetIndexes(null!));
        }

        [Fact]
        public void GetIndexes_WithOutsiderClade()
        {
            Assert.Throws<ArgumentException>(() => tree.GetIndexes(new Clade()));
        }

        [Fact]
        public void GetIndexes_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.Empty(tree.GetIndexes(root));
                Assert.Equal([0], tree.GetIndexes(leafA));
                Assert.Equal([1], tree.GetIndexes(cladeB));
                Assert.Equal([1, 0], tree.GetIndexes(cladeBA));
                Assert.Equal([1, 0, 0], tree.GetIndexes(leafBAA));
                Assert.Equal([1, 0, 1], tree.GetIndexes(leafBAB));
                Assert.Equal([1, 1], tree.GetIndexes(cladeBB));
                Assert.Equal([1, 1, 0], tree.GetIndexes(cladeBBA));
                Assert.Equal([1, 1, 0, 0], tree.GetIndexes(leafBBAA));
                Assert.Equal([1, 1, 0, 1], tree.GetIndexes(leafBBAB));
                Assert.Equal([1, 1, 1], tree.GetIndexes(leafBBB));
                Assert.Equal([2], tree.GetIndexes(leafC));
            });
        }

        [Fact]
        public void Reroot_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => tree.Reroot(null!));
        }

        [Fact]
        public void Reroot_WithOutsiderClade()
        {
            Assert.Throws<ArgumentException>(() => tree.Reroot(new Clade()));
        }

        [Fact]
        public void Reroot_WithLeaf()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafA));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBAA));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBAB));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBAA));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBAB));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBB));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafC));
            });
        }

        [Fact]
        public void Reroot_AsPositive_WithRoot()
        {
            Tree cloned = tree.Clone();

            tree.Reroot(root);
            CustomizedAssertions.Equal(cloned.Root, tree.Root);
        }

        [Fact]
        public void Reroot_AsPositive_WithNonRoot()
        {
            tree.Reroot(this.cladeBB);

            Clade root = tree.Root;
            CheckBipartition(tree, root, double.NaN, null, 3, null);

            Clade cladeBB = root.ChildrenInternal[0];
            CheckBipartition(tree, cladeBB, 2, "100/100", 2, root);

            Clade cladeB = cladeBB.ChildrenInternal[0];
            CheckBipartition(tree, cladeB, 2, "30/45", 2, cladeBB);

            Clade leafA = cladeB.ChildrenInternal[0];
            CheckLeaf(tree, leafA, 2, "A", cladeB);
            Clade leafC = cladeB.ChildrenInternal[1];
            CheckLeaf(tree, leafC, 1, "C", cladeB);

            Clade cladeBA = cladeBB.ChildrenInternal[1];
            CheckBipartition(tree, cladeBA, 1, "20/30", 2, cladeBB);

            Clade leafBAA = cladeBA.ChildrenInternal[0];
            CheckLeaf(tree, leafBAA, 5, "BAA", cladeBA);
            Clade leafBAB = cladeBA.ChildrenInternal[1];
            CheckLeaf(tree, leafBAB, 3, "BAB", cladeBA);

            Clade cladeBBA = root.ChildrenInternal[1];
            CheckBipartition(tree, cladeBBA, 1, "85/95", 2, root);

            Clade leafBBAA = cladeBBA.ChildrenInternal[0];
            CheckLeaf(tree, leafBBAA, 2, "BBAA", cladeBBA);
            Clade leafBBAB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(tree, leafBBAB, 1, "BBAB", cladeBBA);

            Clade leafBBB = root.ChildrenInternal[2];
            CheckLeaf(tree, leafBBB, 3, "BBB", root);

            static void CheckBipartition(Tree tree, Clade clade, double branchLength, string? supports, int childCount, Clade? parent)
            {
                Assert.Multiple(() =>
                {
                    Assert.Same(tree, clade.Tree);
                    if (clade.IsRoot) Assert.Same(tree, clade.TreeInternal);
                    else Assert.Null(clade.TreeInternal);
                    Assert.Equal(branchLength, clade.BranchLength);
                    Assert.Equal(supports, clade.Supports);
                    Assert.Null(clade.Taxon);
                    Assert.Equal(childCount, clade.ChildrenInternal.Count);
                    Assert.Equal(parent, clade.Parent);
                });
            }

            static void CheckLeaf(Tree tree, Clade clade, double branchLength, string? taxon, Clade parent)
            {
                Assert.Multiple(() =>
                {
                    Assert.Same(tree, clade.Tree);
                    Assert.Null(clade.TreeInternal);
                    Assert.Equal(branchLength, clade.BranchLength);
                    Assert.Null(clade.Supports);
                    Assert.Equal(taxon, clade.Taxon);
                    Assert.Empty(clade.ChildrenInternal);
                    Assert.Equal(parent, clade.Parent);
                });
            }
        }

        [Fact]
        public void Rerooted_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => tree.Rerooted(null!));
        }

        [Fact]
        public void Rerooted_WithOutsiderClade()
        {
            Assert.Throws<ArgumentException>(() => tree.Rerooted(new Clade()));
        }

        [Fact]
        public void Rerooted_WithLeaf()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafA));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBAA));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBAB));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBAA));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBAB));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBB));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafC));
            });
        }

        [Fact]
        public void Rerooted_AsPositive_WithRoot()
        {
            Tree cloned = tree.Clone();

            Tree rerooted = tree.Rerooted(root);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
                CustomizedAssertions.Equal(rerooted.Root, tree.Root);
            });
        }

        [Fact]
        public void Rerooted_AsPositive_WithNonRoot()
        {
            Tree cloned = tree.Clone();

            Tree rerooted = tree.Rerooted(this.cladeBB);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
            });

            Clade root = rerooted.Root;
            CheckBipartition(rerooted, root, double.NaN, null, 3, null);

            Clade cladeBB = root.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBB, 2, "100/100", 2, root);

            Clade cladeB = cladeBB.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeB, 2, "30/45", 2, cladeBB);

            Clade leafA = cladeB.ChildrenInternal[0];
            CheckLeaf(rerooted, leafA, 2, "A", cladeB);
            Clade leafC = cladeB.ChildrenInternal[1];
            CheckLeaf(rerooted, leafC, 1, "C", cladeB);

            Clade cladeBA = cladeBB.ChildrenInternal[1];
            CheckBipartition(rerooted, cladeBA, 1, "20/30", 2, cladeBB);

            Clade leafBAA = cladeBA.ChildrenInternal[0];
            CheckLeaf(rerooted, leafBAA, 5, "BAA", cladeBA);
            Clade leafBAB = cladeBA.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBAB, 3, "BAB", cladeBA);

            Clade cladeBBA = root.ChildrenInternal[1];
            CheckBipartition(rerooted, cladeBBA, 1, "85/95", 2, root);

            Clade leafBBAA = cladeBBA.ChildrenInternal[0];
            CheckLeaf(rerooted, leafBBAA, 2, "BBAA", cladeBBA);
            Clade leafBBAB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBAB, 1, "BBAB", cladeBBA);

            Clade leafBBB = root.ChildrenInternal[2];
            CheckLeaf(rerooted, leafBBB, 3, "BBB", root);

            static void CheckBipartition(Tree tree, Clade clade, double branchLength, string? supports, int childCount, Clade? parent)
            {
                Assert.Multiple(() =>
                {
                    Assert.Same(tree, clade.Tree);
                    if (clade.IsRoot) Assert.Same(tree, clade.TreeInternal);
                    else Assert.Null(clade.TreeInternal);
                    Assert.Equal(branchLength, clade.BranchLength);
                    Assert.Equal(supports, clade.Supports);
                    Assert.Null(clade.Taxon);
                    Assert.Equal(childCount, clade.ChildrenInternal.Count);
                    Assert.Equal(parent, clade.Parent);
                });
            }

            static void CheckLeaf(Tree tree, Clade clade, double branchLength, string? taxon, Clade parent)
            {
                Assert.Multiple(() =>
                {
                    Assert.Same(tree, clade.Tree);
                    Assert.Null(clade.TreeInternal);
                    Assert.Equal(branchLength, clade.BranchLength);
                    Assert.Null(clade.Supports);
                    Assert.Equal(taxon, clade.Taxon);
                    Assert.Empty(clade.ChildrenInternal);
                    Assert.Equal(parent, clade.Parent);
                });
            }
        }

        [Fact]
        public void OrderByLength_AsAscending()
        {
            tree.OrderByLength(false);

            Assert.Equal(["C", "A", "BBAB", "BBAA", "BBB", "BAB", "BAA"], tree.GetAllLeaves().Select(x => x.Taxon));
        }

        [Fact]
        public void OrderByLength_AsDescending()
        {
            tree.OrderByLength(true);

            Assert.Equal(["BAA", "BAB", "BBAA", "BBAB", "BBB", "A", "C"], tree.GetAllLeaves().Select(x => x.Taxon));
        }

        [Fact]
        public async Task WriteAsync()
        {
            using var writer = new StringWriter();
            await tree.WriteAsync(writer, TreeFormat.Newick);

            Assert.Equal("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);", writer.ToString());
        }

        [Fact]
        public void ToString_AsPositive()
        {
            Assert.Equal("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);", tree.ToString());
        }

        #endregion Instance Methods
    }
}
