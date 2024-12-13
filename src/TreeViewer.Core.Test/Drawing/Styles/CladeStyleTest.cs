using TreeViewer.Core.Assertions;

namespace TreeViewer.Core.Drawing.Styles
{
    public class CladeStyleTest
    {
        private readonly CladeStyle style;

        public CladeStyleTest()
        {
            style = new CladeStyle();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new CladeStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal("black", style.BranchColor);
                Assert.Equal("black", style.LeafColor);
            });
        }

        #endregion Ctors

        #region Methods

        [Fact]
        public void ApplyValues_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => style.ApplyValues(null!));
        }

        [Fact]
        public void ApplyValues_AsPositive()
        {
            style.ApplyValues(new CladeStyle()
            {
                BranchColor = "white",
                LeafColor = "none",
            });

            Assert.Multiple(() =>
            {
                Assert.Equal("white", style.BranchColor);
                Assert.Equal("none", style.LeafColor);
            });
        }

        [Fact]
        public void Clone()
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";

            CladeStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";

            var cloned = (CladeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
