using TreeViewer.TestUtilities.Assertions;
using TreeViewer.Core.Trees;
using TreeViewer.TestUtilities;

namespace TreeViewer.Core.Drawing.Styles
{
    public partial class TreeStyleTest
    {
        private readonly TreeStyle style;

        public TreeStyleTest()
        {
            style = new TreeStyle()
            {
                BranchValueHideRegexPattern = "hoge",
            };
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var options = new TreeStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal(300, options.XScale);
                Assert.Equal(30, options.YScale);
                Assert.Equal(1, options.BranchThickness);
                Assert.True(options.ShowLeafLabels);
                Assert.Equal(20, options.LeafLabelsFontSize);
                Assert.True(options.ShowCladeLabels);
                Assert.Equal(20, options.CladeLabelsFontSize);
                Assert.Equal(5, options.CladeLabelsLineThickness);
                Assert.True(options.ShowNodeValues);
                Assert.Equal(CladeValueType.Supports, options.NodeValueType);
                Assert.Equal(15, options.NodeValueFontSize);
                Assert.True(options.ShowBranchValues);
                Assert.Equal(CladeValueType.BranchLength, options.BranchValueType);
                Assert.Equal(15, options.BranchValueFontSize);
                Assert.Null(options.BranchValueHideRegex);
                Assert.Null(options.BranchValueHideRegexPattern);
                Assert.True(options.ShowBranchDecorations);
                Assert.Empty(options.DecorationStyles);
                Assert.True(options.ShowScaleBar);
                Assert.Equal(0.1, options.ScaleBarValue);
                Assert.Equal(25, options.ScaleBarFontSize);
                Assert.Equal(5, options.ScaleBarThickness);
                Assert.Equal(CladeCollapseType.TopMax, options.CollapseType);
                Assert.Equal(1, options.CollapsedConstantWidth);
            });
        }

        #endregion Ctors

        #region Properties

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LeafLabelHideRegexPattern_Set_AsNullOrEmpty(string? value)
        {
            style.BranchValueHideRegexPattern = value;

            Assert.Multiple(() =>
            {
                Assert.Equal(value, style.BranchValueHideRegexPattern);
                Assert.Null(style.BranchValueHideRegex);
            });
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Fact]
        public void LeafLabelHideRegexPattern_Set_AsInvalidRegex()
        {
            style.BranchValueHideRegexPattern = "(";

            Assert.Multiple(() =>
            {
                Assert.Equal("(", style.BranchValueHideRegexPattern);
                Assert.Null(style.BranchValueHideRegex);
            });
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Fact]
        public void LeafLabelHideRegexPattern_Set_AsValidRegex()
        {
            style.BranchValueHideRegexPattern = "50";

            Assert.Multiple(() =>
            {
                Assert.Equal("50", style.BranchValueHideRegexPattern);
                Assert.Equal("50", style.BranchValueHideRegex?.ToString());
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
            TreeStyle applied = DummyData.CreateTreeStyle();
            style.ApplyValues(applied);

            CustomizedAssertions.Equal(applied, style);
        }

        [Fact]
        public void Clone()
        {
            style.ApplyValues(DummyData.CreateTreeStyle());

            TreeStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.ApplyValues(DummyData.CreateTreeStyle());

            var cloned = (TreeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
