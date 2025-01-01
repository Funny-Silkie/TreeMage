using TreeViewer.TestUtilities.Assertions;

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
                Assert.False(style.Collapsed);
                Assert.Null(style.CladeLabel);
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
                Collapsed = true,
                CladeLabel = "clade",
            });

            Assert.Multiple(() =>
            {
                Assert.Equal("white", style.BranchColor);
                Assert.Equal("none", style.LeafColor);
                Assert.True(style.Collapsed);
                Assert.Equal("clade", style.CladeLabel);
            });
        }

        [Fact]
        public void Clone()
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";
            style.Collapsed = true;
            style.CladeLabel = "clade";

            CladeStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";
            style.Collapsed = true;
            style.CladeLabel = "clade";

            var cloned = (CladeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
