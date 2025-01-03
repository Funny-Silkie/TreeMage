using TreeViewer.TestUtilities.Assertions;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.TestUtilities;

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
            tree = DummyData.CreateTree(out root, out leafA, out cladeB, out cladeBA, out leafBAA, out leafBAB, out cladeBB, out cladeBBA, out leafBBAA, out leafBBAB, out leafBBB, out leafC);
            tree.Style.ApplyValues(DummyData.CreateTreeStyle());
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

        #region Properties

        [Fact]
        public void IsUnrooted_Get_OnUnrooted()
        {
            Assert.True(tree.IsUnrooted);
        }

        [Fact]
        public void IsUnrooted_Get_OnRooted()
        {
            root.RemoveChild(leafA);

            Assert.False(tree.IsUnrooted);
        }

        #endregion Properties

        #region Static Methods

        [Fact]
        public async Task ReadAsync()
        {
            using var reader = new StringReader("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);");
            Tree[] trees = await Tree.ReadAsync(reader, TreeFormat.Newick);

            Assert.Multiple(() =>
            {
                Assert.Single(trees);
                CustomizedAssertions.Equal(DummyData.CreateTree().Root, trees[0].Root);
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
            Assert.Throws<ArgumentNullException>(() => tree.Reroot(null!, true));
        }

        [Fact]
        public void Reroot_WithOutsiderClade()
        {
            Assert.Throws<ArgumentException>(() => tree.Reroot(new Clade(), true));
        }

        [Fact]
        public void Reroot_WithLeaf()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafA, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBAA, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBAB, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBAA, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBAB, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafBBB, true));
                Assert.Throws<ArgumentException>(() => tree.Reroot(leafC, true));
            });
        }

        [Fact]
        public void Reroot_AsPositive_WithRoot_AsUnrootedToUnrooted()
        {
            Tree cloned = tree.Clone();

            tree.Reroot(root, false);
            CustomizedAssertions.Equal(cloned.Root, tree.Root);
        }

        [Fact]
        public void Reroot_AsPositive_WithRoot_AsUnrootedToRooted()
        {
            Assert.Throws<ArgumentException>(() => tree.Reroot(root, true));
        }

        [Fact]
        public void Reroot_AsPositive_WithRoot_AsRootedToUnrooted()
        {
            root.RemoveChild(leafC);

            Assert.Throws<ArgumentException>(() => tree.Reroot(root, false));
        }

        [Fact]
        public void Reroot_AsPositive_WithRoot_AsRootedToRooted()
        {
            root.RemoveChild(leafC);
            Tree cloned = tree.Clone();

            tree.Reroot(root, true);
            CustomizedAssertions.Equal(cloned.Root, tree.Root);
        }

        [Fact]
        public void Reroot_AsPositive_WithNonRoot_AsUnrootedToUnrooted()
        {
            tree.Reroot(this.cladeBB, false);

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
        public void Reroot_AsPositive_WithNonRoot_AsUnrootedToRooted()
        {
            tree.Reroot(cladeBB, true);

            /*
             * root
             * - cladeBB
             *   - cladeB
             *     - leafA
             *     - leafC
             *   - cladeBA
             *     - leafBAA
             *     - leafBAB
             * - cladeBB
             *   - cladeBBA
             *     - leafBBAA
             *     - leafBBAA
             *   - leafBBB
             */

            Clade root = tree.Root;
            CheckBipartition(tree, root, double.NaN, null, 2, null);

            Clade cladeBB_1 = root.ChildrenInternal[0];
            CheckBipartition(tree, cladeBB_1, 1, "100/100", 2, root);

            Clade cladeBB_2 = root.ChildrenInternal[1];
            CheckBipartition(tree, cladeBB_2, 1, "100/100", 2, root);

            Clade cladeB = cladeBB_1.ChildrenInternal[0];
            CheckBipartition(tree, cladeB, 2, "30/45", 2, cladeBB_1);

            Clade leafA = cladeB.ChildrenInternal[0];
            CheckLeaf(tree, leafA, 2, "A", cladeB);
            Clade leafC = cladeB.ChildrenInternal[1];
            CheckLeaf(tree, leafC, 1, "C", cladeB);

            Clade cladeBA = cladeBB_1.ChildrenInternal[1];
            CheckBipartition(tree, cladeBA, 1, "20/30", 2, cladeBB_1);

            Clade leafBAA = cladeBA.ChildrenInternal[0];
            CheckLeaf(tree, leafBAA, 5, "BAA", cladeBA);
            Clade leafBAB = cladeBA.ChildrenInternal[1];
            CheckLeaf(tree, leafBAB, 3, "BAB", cladeBA);

            Clade cladeBBA = cladeBB_2.ChildrenInternal[0];
            CheckBipartition(tree, cladeBBA, 1, "85/95", 2, cladeBB_2);

            Clade leafBBAA = cladeBBA.ChildrenInternal[0];
            CheckLeaf(tree, leafBBAA, 2, "BBAA", cladeBBA);
            Clade leafBBAB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(tree, leafBBAB, 1, "BBAB", cladeBBA);

            Clade leafBBB = cladeBB_2.ChildrenInternal[1];
            CheckLeaf(tree, leafBBB, 3, "BBB", cladeBB_2);

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
        public void Reroot_AsPositive_WithNonRoot_AsRootedToUnrooted()
        {
            tree.Reroot(this.cladeBB, true);
            tree.Reroot(this.cladeBBA, false);

            /*
             * root
             * - cladeBBA
             *   - cladeBB
             *     - cladeB
             *       - leafA
             *       - leafC
             *     - cladeBA
             *       - leafBAA
             *       - leafBAB
             *   - leafBBB
             * - leafBBAA
             * - leafBBAA
             */

            Clade root = tree.Root;
            CheckBipartition(tree, root, double.NaN, null, 3, null);

            Clade cladeBBA = root.ChildrenInternal[0];
            CheckBipartition(tree, cladeBBA, 1, "85/95", 2, root);

            Clade cladeBB = cladeBBA.ChildrenInternal[0];
            CheckBipartition(tree, cladeBB, 2, "100/100", 2, cladeBBA);

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

            Clade leafBBB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(tree, leafBBB, 3, "BBB", cladeBBA);

            Clade leafBBAA = root.ChildrenInternal[1];
            CheckLeaf(tree, leafBBAA, 2, "BBAA", root);
            Clade leafBBAB = root.ChildrenInternal[2];
            CheckLeaf(tree, leafBBAB, 1, "BBAB", root);

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
        public void Reroot_AsPositive_WithNonRoot_AsRootedToRooted()
        {
            tree.Reroot(this.cladeBB, true);
            tree.Reroot(cladeBBA, true);

            /*
             * root
             * - cladeBBA
             *   - cladeBB
             *     - cladeB
             *       - leafA
             *       - leafC
             *     - cladeBA
             *       - leafBAA
             *       - leafBAB
             *   - leafBBB
             * - cladeBBA
             *   - leafBBAA
             *   - leafBBAA
             */

            Clade root = tree.Root;
            CheckBipartition(tree, root, double.NaN, null, 2, null);

            Clade cladeBBA_1 = root.ChildrenInternal[0];
            CheckBipartition(tree, cladeBBA_1, 0.5, "85/95", 2, root);

            Clade cladeBBA_2 = root.ChildrenInternal[1];
            CheckBipartition(tree, cladeBBA_2, 0.5, "85/95", 2, root);

            Clade cladeBB = cladeBBA_1.ChildrenInternal[0];
            CheckBipartition(tree, cladeBB, 2, "100/100", 2, cladeBBA_1);

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

            Clade leafBBB = cladeBBA_1.ChildrenInternal[1];
            CheckLeaf(tree, leafBBB, 3, "BBB", cladeBBA_1);

            Clade leafBBAA = cladeBBA_2.ChildrenInternal[0];
            CheckLeaf(tree, leafBBAA, 2, "BBAA", cladeBBA_2);
            Clade leafBBAB = cladeBBA_2.ChildrenInternal[1];
            CheckLeaf(tree, leafBBAB, 1, "BBAB", cladeBBA_2);

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
            Assert.Throws<ArgumentNullException>(() => tree.Rerooted(null!, true));
        }

        [Fact]
        public void Rerooted_WithOutsiderClade()
        {
            Assert.Throws<ArgumentException>(() => tree.Rerooted(new Clade(), true));
        }

        [Fact]
        public void Rerooted_WithLeaf()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafA, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBAA, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBAB, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBAA, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBAB, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafBBB, true));
                Assert.Throws<ArgumentException>(() => tree.Rerooted(leafC, true));
            });
        }

        [Fact]
        public void Rerooted_AsPositive_WithRoot_AsUnrootedToUnrooted()
        {
            Tree cloned = tree.Clone();

            Tree rerooted = tree.Rerooted(root, false);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
                CustomizedAssertions.Equal(rerooted.Root, tree.Root);
            });
        }

        [Fact]
        public void Rerooted_AsPositive_WithRoot_AsUnrootedToRooted()
        {
            Assert.Throws<ArgumentException>(() => tree.Rerooted(root, true));
        }

        [Fact]
        public void Rerooted_AsPositive_WithRoot_AsRootedToUnrooted()
        {
            root.RemoveChild(leafC);

            Assert.Throws<ArgumentException>(() => tree.Rerooted(root, false));
        }

        [Fact]
        public void Rerooted_AsPositive_WithRoot_AsRootedToRooted()
        {
            root.RemoveChild(leafC);
            Tree cloned = tree.Clone();

            Tree rerooted = tree.Rerooted(root, true);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
                CustomizedAssertions.Equal(rerooted.Root, tree.Root);
            });
        }

        [Fact]
        public void Rerooted_AsPositive_WithNonRoot_AsUnrootedToUnrooted()
        {
            Tree cloned = tree.Clone();

            Tree rerooted = tree.Rerooted(this.cladeBB, false);

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
        public void Rerooted_AsPositive_WithNonRoot_AsUnrootedToRooted()
        {
            Tree cloned = tree.Clone();
            Tree rerooted = tree.Rerooted(cladeBB, true);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
            });

            /*
             * root
             * - cladeBB
             *   - cladeB
             *     - leafA
             *     - leafC
             *   - cladeBA
             *     - leafBAA
             *     - leafBAB
             * - cladeBB
             *   - cladeBBA
             *     - leafBBAA
             *     - leafBBAA
             *   - leafBBB
             */

            Clade root = rerooted.Root;
            CheckBipartition(rerooted, root, double.NaN, null, 2, null);

            Clade cladeBB_1 = root.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBB_1, 1, "100/100", 2, root);

            Clade cladeBB_2 = root.ChildrenInternal[1];
            CheckBipartition(rerooted, cladeBB_2, 1, "100/100", 2, root);

            Clade cladeB = cladeBB_1.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeB, 2, "30/45", 2, cladeBB_1);

            Clade leafA = cladeB.ChildrenInternal[0];
            CheckLeaf(rerooted, leafA, 2, "A", cladeB);
            Clade leafC = cladeB.ChildrenInternal[1];
            CheckLeaf(rerooted, leafC, 1, "C", cladeB);

            Clade cladeBA = cladeBB_1.ChildrenInternal[1];
            CheckBipartition(rerooted, cladeBA, 1, "20/30", 2, cladeBB_1);

            Clade leafBAA = cladeBA.ChildrenInternal[0];
            CheckLeaf(rerooted, leafBAA, 5, "BAA", cladeBA);
            Clade leafBAB = cladeBA.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBAB, 3, "BAB", cladeBA);

            Clade cladeBBA = cladeBB_2.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBBA, 1, "85/95", 2, cladeBB_2);

            Clade leafBBAA = cladeBBA.ChildrenInternal[0];
            CheckLeaf(rerooted, leafBBAA, 2, "BBAA", cladeBBA);
            Clade leafBBAB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBAB, 1, "BBAB", cladeBBA);

            Clade leafBBB = cladeBB_2.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBB, 3, "BBB", cladeBB_2);

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
        public void Rerooted_AsPositive_WithNonRoot_AsRootedToUnrooted()
        {
            tree.Reroot(this.cladeBB, true);

            Tree cloned = tree.Clone();
            Tree rerooted = tree.Rerooted(this.cladeBBA, false);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
            });

            /*
             * root
             * - cladeBBA
             *   - cladeBB
             *     - cladeB
             *       - leafA
             *       - leafC
             *     - cladeBA
             *       - leafBAA
             *       - leafBAB
             *   - leafBBB
             * - leafBBAA
             * - leafBBAA
             */

            Clade root = rerooted.Root;
            CheckBipartition(rerooted, root, double.NaN, null, 3, null);

            Clade cladeBBA = root.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBBA, 1, "85/95", 2, root);

            Clade cladeBB = cladeBBA.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBB, 2, "100/100", 2, cladeBBA);

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

            Clade leafBBB = cladeBBA.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBB, 3, "BBB", cladeBBA);

            Clade leafBBAA = root.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBAA, 2, "BBAA", root);
            Clade leafBBAB = root.ChildrenInternal[2];
            CheckLeaf(rerooted, leafBBAB, 1, "BBAB", root);

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
        public void Rerooted_AsPositive_WithNonRoot_AsRootedToRooted()
        {
            tree.Reroot(this.cladeBB, true);

            Tree cloned = tree.Clone();
            Tree rerooted = tree.Rerooted(cladeBBA, true);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.Equal(cloned, tree);
                Assert.NotSame(tree, rerooted);
            });

            /*
             * root
             * - cladeBBA
             *   - cladeBB
             *     - cladeB
             *       - leafA
             *       - leafC
             *     - cladeBA
             *       - leafBAA
             *       - leafBAB
             *   - leafBBB
             * - cladeBBA
             *   - leafBBAA
             *   - leafBBAA
             */

            Clade root = rerooted.Root;
            CheckBipartition(rerooted, root, double.NaN, null, 2, null);

            Clade cladeBBA_1 = root.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBBA_1, 0.5, "85/95", 2, root);

            Clade cladeBBA_2 = root.ChildrenInternal[1];
            CheckBipartition(rerooted, cladeBBA_2, 0.5, "85/95", 2, root);

            Clade cladeBB = cladeBBA_1.ChildrenInternal[0];
            CheckBipartition(rerooted, cladeBB, 2, "100/100", 2, cladeBBA_1);

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

            Clade leafBBB = cladeBBA_1.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBB, 3, "BBB", cladeBBA_1);

            Clade leafBBAA = cladeBBA_2.ChildrenInternal[0];
            CheckLeaf(rerooted, leafBBAA, 2, "BBAA", cladeBBA_2);
            Clade leafBBAB = cladeBBA_2.ChildrenInternal[1];
            CheckLeaf(rerooted, leafBBAB, 1, "BBAB", cladeBBA_2);

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
        public void SwapSisters_WithNullClade()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => tree.SwapSisters(null!, cladeB));
                Assert.Throws<ArgumentNullException>(() => tree.SwapSisters(cladeB, null!));
            });
        }

        [Fact]
        public void SwapSisters_WithSameClade()
        {
            Assert.Throws<ArgumentException>(() => tree.SwapSisters(cladeB, cladeB));
        }

        [Fact]
        public void SwapSisters_WithOutsiderClades()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => tree.SwapSisters(cladeB, new Clade()));
                Assert.Throws<ArgumentException>(() => tree.SwapSisters(new Clade(), cladeB));
            });
        }

        [Fact]
        public void SwapSisters_WithNonSisterClades()
        {
            Assert.Throws<ArgumentException>(() => tree.SwapSisters(cladeB, cladeBA));
        }

        [Fact]
        public void SwapSisters_AsPositive()
        {
            tree.SwapSisters(cladeBA, cladeBB);

            Assert.Multiple(() =>
            {
                Assert.Same(cladeB, cladeBA.Parent);
                Assert.Same(cladeB, cladeBB.Parent);
                Assert.Equal(cladeB.ChildrenInternal, [cladeBB, cladeBA]);
            });
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
