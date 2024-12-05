namespace TreeViewer.Core.Trees
{
    public class TreeTest
    {
        //          2
        //      +------leafA    1
        // root-|   2         +---leafBA
        //      +------cladeB-|    3
        //                    +---------leafBB

        private readonly Clade root;
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade leafBA;
        private readonly Clade leafBB;
        private readonly Tree tree;

        public TreeTest()
        {
            root = new Clade()
            {
                Supports = "80/100",
                BranchLength = 0,
            };
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
            leafBA = new Clade()
            {
                Taxon = "BA",
                BranchLength = 1,
                Parent = cladeB,
            };
            leafBB = new Clade()
            {
                Taxon = "BB",
                BranchLength = 3,
                Parent = cladeB,
            };

            root.ChildrenInternal.AddRange(leafA, cladeB);
            cladeB.ChildrenInternal.AddRange(leafBA, leafBB);

            tree = new Tree(root);
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
                Assert.Throws<ArgumentException>(() => new Tree(leafBA));
                Assert.Throws<ArgumentException>(() => new Tree(leafBB));
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

        #region Methods

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

            Assert.Equal([root, leafA, cladeB, leafBA, leafBB], clades);
        }

        [Fact]
        public void GetAllBipartitions()
        {
            IEnumerable<Clade> bipartitions = tree.GetAllBipartitions();

            Assert.Equal([root, cladeB], bipartitions);
        }

        [Fact]
        public void GetAllLeaves()
        {
            IEnumerable<Clade> leaves = tree.GetAllLeaves();

            Assert.Equal([leafA, leafBA, leafBB], leaves);
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

        #endregion Methods
    }
}
