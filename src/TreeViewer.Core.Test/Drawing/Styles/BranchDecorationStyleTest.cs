using TreeViewer.TestUtilities.Assertions;

namespace TreeViewer.Core.Drawing.Styles
{
    public partial class BranchDecorationStyleTest
    {
        private readonly BranchDecorationStyle style;

        public BranchDecorationStyleTest()
        {
            style = new BranchDecorationStyle()
            {
                RegexPattern = "100",
            };
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new BranchDecorationStyle();

            Assert.Multiple(() =>
            {
                Assert.True(style.Enabled);
                Assert.Null(style.Regex);
                Assert.Null(style.RegexPattern);
                Assert.Equal(5, style.ShapeSize);
                Assert.Equal(BranchDecorationType.ClosedCircle, style.DecorationType);
                Assert.Equal("black", style.ShapeColor);
            });
        }

        #endregion Ctors

        #region Properties

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RegexPattern_Set_AsNullOrEmpty(string? value)
        {
            style.RegexPattern = value;

            Assert.Multiple(() =>
            {
                Assert.Equal(value, style.RegexPattern);
                Assert.Null(style.Regex);
            });
        }

        [Fact]
        public void RegexPattern_Set_AsInValidRegex()
        {
            style.RegexPattern = "(";

            Assert.Multiple(() =>
            {
                Assert.Equal("(", style.RegexPattern);
                Assert.Null(style.Regex);
            });
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Fact]
        public void RegexPattern_Set_AsValidRegex()
        {
            style.RegexPattern = "50";

            Assert.Multiple(() =>
            {
                Assert.Equal("50", style.RegexPattern);
                Assert.Equal("50", style.Regex?.ToString());
            });
        }

        #endregion Properties

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
                Enabled = false,
                RegexPattern = "100",
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
            style.Enabled = false;
            style.RegexPattern = "100";
            style.ShapeSize = 10;
            style.DecorationType = BranchDecorationType.OpenedRectangle;
            style.ShapeColor = "red";

            BranchDecorationStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.Enabled = false;
            style.RegexPattern = "100";
            style.ShapeSize = 10;
            style.DecorationType = BranchDecorationType.OpenedRectangle;
            style.ShapeColor = "red";

            var cloned = (BranchDecorationStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
