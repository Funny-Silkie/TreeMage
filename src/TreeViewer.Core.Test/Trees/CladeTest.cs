namespace TreeViewer.Core.Trees
{
    public class CladeTest
    {
        //            2
        //      1 +------leafA    1
        // root---|   2         +---leafBA
        //        +------cladeB-|    3
        //                      +---------leafBB

        private readonly Clade root;
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade leafBA;
        private readonly Clade leafBB;

        public CladeTest()
        {
            root = new Clade()
            {
                Supports = "80/100",
                BranchLength = 1,
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

            root.ChildrenEditable.AddRange(leafA, cladeB);
            cladeB.ChildrenEditable.AddRange(leafBA, leafBB);
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Clade clade = null!;

            Exception? exception = Record.Exception(() => clade = new Clade());

            Assert.Multiple(() =>
            {
                Assert.Null(exception);
                Assert.Null(clade.Taxon);
                Assert.Null(clade.Supports);
                Assert.Equal(double.NaN, clade.BranchLength);
                Assert.Null(clade.Parent);
                Assert.Empty(clade.ChildrenEditable);
            });
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void IsRoot_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.True(root.IsRoot);
                Assert.False(leafA.IsRoot);
                Assert.False(cladeB.IsRoot);
                Assert.False(leafBA.IsRoot);
                Assert.False(leafBB.IsRoot);
            });
        }

        [Fact]
        public void IsLeaf_Get()
        {
            Assert.False(root.IsLeaf);
            Assert.True(leafA.IsLeaf);
            Assert.False(cladeB.IsLeaf);
            Assert.True(leafBA.IsLeaf);
            Assert.True(leafBB.IsLeaf);
        }

        [Fact]
        public void Children_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(root.ChildrenEditable, root.Children);
                Assert.Equal(leafA.ChildrenEditable, leafA.Children);
                Assert.Equal(cladeB.ChildrenEditable, cladeB.Children);
                Assert.Equal(leafBA.ChildrenEditable, leafBA.Children);
                Assert.Equal(leafBB.ChildrenEditable, leafBB.Children);
            });
        }

        #endregion Properties
    }
}
