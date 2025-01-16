using TreeMage.Core.Trees;
using TreeMage.TestUtilities;

namespace TreeMage.Core.Drawing
{
    public class DrawingExtensionsTest
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

        public DrawingExtensionsTest()
        {
            tree = DummyData.CreateTree(out root, out leafA, out cladeB, out cladeBA, out leafBAA, out leafBAB, out cladeBB, out cladeBBA, out leafBBAA, out leafBBAB, out leafBBB, out leafC);
        }

        /// <summary>
        /// ダミーツリーの全てのクレードを取得します。
        /// </summary>
        /// <returns>ダミーツリーの全てのクレード</returns>
        private Clade[] GetAllClades() => [root, leafA, cladeB, cladeBA, leafBAA, leafBAB, cladeBB, cladeBBA, leafBBAA, leafBBAB, leafBBB, leafC];

        #region Static Methods

        [Fact]
        public void GetAllExternalNodes_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => DrawingExtensions.GetAllExternalNodes(null!));
        }

        [Fact]
        public void GetAllExternalNodes_AsPositive_OnNoCladeCollapsed()
        {
            IEnumerable<Clade> clades = tree.GetAllExternalNodes();

            Assert.Equal([leafA, leafBAA, leafBAB, leafBBAA, leafBBAB, leafBBB, leafC], clades);
        }

        [Fact]
        public void GetAllExternalNodes_AsPositive_OnCladeCollapsed()
        {
            cladeBB.Style.Collapsed = true;
            IEnumerable<Clade> clades = tree.GetAllExternalNodes();

            Assert.Equal([leafA, leafBAA, leafBAB, cladeBB, leafC], clades);
        }

        [Fact]
        public void GetIsHidden_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => DrawingExtensions.GetIsHidden(null!));
        }

        [Fact]
        public void GetIsHidden_AsPositive()
        {
            cladeBB.Style.Collapsed = true;

            Assert.Multiple(() =>
            {
                Assert.False(root.GetIsHidden());
                Assert.False(leafA.GetIsHidden());
                Assert.False(cladeB.GetIsHidden());
                Assert.False(cladeBA.GetIsHidden());
                Assert.False(leafBAA.GetIsHidden());
                Assert.False(leafBAB.GetIsHidden());
                Assert.False(cladeBB.GetIsHidden());
                Assert.True(cladeBBA.GetIsHidden());
                Assert.True(leafBBAA.GetIsHidden());
                Assert.True(leafBBAB.GetIsHidden());
                Assert.True(leafBBB.GetIsHidden());
                Assert.False(leafC.GetIsHidden());
            });
        }

        [Fact]
        public void GetIsExternal_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => DrawingExtensions.GetIsExternal(null!));
        }

        [Fact]
        public void GetIsExternal_AsPositive()
        {
            cladeBB.Style.Collapsed = true;

            Assert.Multiple(() =>
            {
                Assert.False(root.GetIsExternal());
                Assert.True(leafA.GetIsExternal());
                Assert.False(cladeB.GetIsExternal());
                Assert.False(cladeBA.GetIsExternal());
                Assert.True(leafBAA.GetIsExternal());
                Assert.True(leafBAB.GetIsExternal());
                Assert.True(cladeBB.GetIsExternal());
                Assert.False(cladeBBA.GetIsExternal());
                Assert.True(leafBBAA.GetIsExternal());
                Assert.True(leafBBAB.GetIsExternal());
                Assert.True(leafBBB.GetIsExternal());
                Assert.True(leafC.GetIsExternal());
            });
        }

        [Fact]
        public void GetDrawnBranchLength_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => DrawingExtensions.GetDrawnBranchLength(null!));
        }

        [Fact]
        public void GetDrawnBranchLength_AsPositive_WithRootClade()
        {
            root.BranchLength = double.NaN;
            Assert.Equal(0, root.GetDrawnBranchLength());
        }

        [Fact]
        public void GetDrawnBranchLength_AsPositive_WithNaNLengthAndTreeClade()
        {
            cladeBA.BranchLength = double.NaN;
            Assert.Equal(tree.Style.DefaultBranchLength, cladeBA.GetDrawnBranchLength());
        }

        [Fact]
        public void GetDrawnBranchLength_AsPositive_WithNaNLengthAndNoTreeClade()
        {
            var clade = new Clade()
            {
                BranchLength = double.NaN,
            };
            Assert.Equal(0, clade.GetDrawnBranchLength());
        }

        [Fact]
        public void GetDrawnBranchLength_AsPositive_WithNoNaNLengthClade()
        {
            cladeBA.BranchLength = 1;
            Assert.Equal(1, cladeBA.GetDrawnBranchLength());
        }

        #endregion Static Methods
    }
}
