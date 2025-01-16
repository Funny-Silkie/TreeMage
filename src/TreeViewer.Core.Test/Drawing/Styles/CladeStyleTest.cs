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
                Assert.Null(style.ShadeColor);
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
            var applied = new CladeStyle()
            {
                BranchColor = "white",
                LeafColor = "none",
                Collapsed = true,
                CladeLabel = "clade",
                ShadeColor = "blue",
            };
            style.ApplyValues(applied);

            CustomizedAssertions.Equal(applied, style);
        }

        [Fact]
        public void Clone()
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";
            style.Collapsed = true;
            style.CladeLabel = "clade";
            style.ShadeColor = "blue";

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
            style.ShadeColor = "blue";

            var cloned = (CladeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
