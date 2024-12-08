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
                CladeTest.CompareClades(CreateDummyTree().Root, trees[0].Root);
            });
        }

        #endregion Static Methods

        #region Instance Methods

        [Fact]
        public void Clone()
        {
            Tree cloned = tree.Clone();

            CladeTest.CompareClades(tree.Root, cloned.Root);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            var cloned = (Tree)((ICloneable)tree).Clone();

            CladeTest.CompareClades(tree.Root, cloned.Root);
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
        public void Reroot_AsPositive_WithRoot()
        {
            Tree cloned = tree.Clone();

            tree.Reroot(root);
            CladeTest.CompareClades(cloned.Root, tree.Root);
        }

        [Fact]
        public void Reroot_AsPositive_WithNonRoot()
        {
            tree.Reroot(cladeBB);

            Assert.Equal("(((A:2,C:1)30/45:2,(BAA:5,BAB:3)20/30:1)100/100:2,(BBAA:2,BBAB:1)85/95:1,BBB:3);", tree.ToString());
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
