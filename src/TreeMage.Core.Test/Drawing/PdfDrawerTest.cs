using TreeMage.Core.Exporting;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities;

namespace TreeMage.Core.Drawing
{
    public class PdfDrawerTest
    {
        private readonly Tree tree;
        private readonly PdfDrawer drawer;
        private readonly ExportOptions exportOptions;

        public PdfDrawerTest()
        {
            tree = DummyData.CreateTree();
            tree.Style.XScale = 30;
            tree.Style.ScaleBarValue = 1;
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.CladeLabel = "hoge";
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.ShadeColor = "lightblue";
            drawer = new PdfDrawer();
            exportOptions = new ExportOptions();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new PdfDrawer());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Document_Get_OnDefault()
        {
            Assert.Throws<InvalidOperationException>(() => _ = drawer.Document);
        }

        #endregion Properties

        #region Methods

        [Fact]
        public void Draw_WithNullTree()
        {
            Assert.Throws<ArgumentNullException>(() => ((ITreeDrawer)drawer).Draw(null!, exportOptions));
        }

        [Fact]
        public void Draw_WithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => ((ITreeDrawer)drawer).Draw(tree, null!));
        }

        [Fact]
        public void Draw_AsPositive()
        {
            Exception? exception = Record.Exception(() => ((ITreeDrawer)drawer).Draw(tree, exportOptions));

            Assert.Multiple(() =>
            {
                Assert.Null(exception);
                Assert.NotNull(drawer.Document);
            });
        }

        #endregion Methods
    }
}
