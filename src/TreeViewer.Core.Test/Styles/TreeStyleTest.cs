using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Styles
{
    public class TreeStyleTest
    {
        private readonly TreeStyle options;

        public TreeStyleTest()
        {
            options = new TreeStyle();
        }

        #region Ctors

        [Fact]
        public void Ctors()
        {
            var options = new TreeStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal(300, options.XScale);
                Assert.Equal(30, options.YScale);
                Assert.Equal(1, options.BranchThickness);
                Assert.True(options.ShowLeafLabels);
                Assert.Equal(20, options.LeafLabelsFontSize);
                Assert.True(options.ShowNodeValues);
                Assert.Equal(CladeValueType.Supports, options.NodeValueType);
                Assert.Equal(15, options.NodeValueFontSize);
                Assert.True(options.ShowBranchValues);
                Assert.Equal(CladeValueType.BranchLength, options.BranchValueType);
                Assert.Equal(15, options.BranchValueFontSize);
                Assert.True(options.ShowBranchDecorations);
                Assert.Empty(options.DecorationStyles);
                Assert.True(options.ShowScaleBar);
                Assert.Equal(0.1, options.ScaleBarValue);
                Assert.Equal(25, options.ScaleBarFontSize);
                Assert.Equal(5, options.ScaleBarThickness);
            });
        }

        #endregion Ctors
    }
}
