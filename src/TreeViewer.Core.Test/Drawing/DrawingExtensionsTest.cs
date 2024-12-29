using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing
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
            tree = TreeTest.CreateDummyTree(out root, out leafA, out cladeB, out cladeBA, out leafBAA, out leafBAB, out cladeBB, out cladeBBA, out leafBBAA, out leafBBAB, out leafBBB, out leafC);
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

        #endregion Static Methods
    }
}
