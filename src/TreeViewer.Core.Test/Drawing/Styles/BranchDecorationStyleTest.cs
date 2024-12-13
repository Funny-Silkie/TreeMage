using System.Text.RegularExpressions;
using TreeViewer.Core.Assertions;

namespace TreeViewer.Core.Drawing.Styles
{
    public partial class BranchDecorationStyleTest
    {
        private readonly BranchDecorationStyle style;

        public BranchDecorationStyleTest()
        {
            style = new BranchDecorationStyle();
        }

        [GeneratedRegex("hoge")]
        private static partial Regex GetDummyRegex();

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new BranchDecorationStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal("100", style.Regex.ToString());
                Assert.Equal(5, style.ShapeSize);
                Assert.Equal(BranchDecorationType.ClosedCircle, style.DecorationType);
                Assert.Equal("black", style.ShapeColor);
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
            var applied = new BranchDecorationStyle()
            {
                Regex = GetDummyRegex(),
                ShapeSize = 10,
                DecorationType = BranchDecorationType.OpenedRectangle,
                ShapeColor = "red",
            };

            style.ApplyValues(applied);

            CustomizedAssertions.Equal(applied, style);
        }

        [Fact]
        public void Clone()
        {
            style.Regex = GetDummyRegex();
            style.ShapeSize = 10;
            style.DecorationType = BranchDecorationType.OpenedRectangle;
            style.ShapeColor = "red";

            BranchDecorationStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.Regex = GetDummyRegex();
            style.ShapeSize = 10;
            style.DecorationType = BranchDecorationType.OpenedRectangle;
            style.ShapeColor = "red";

            var cloned = (BranchDecorationStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
