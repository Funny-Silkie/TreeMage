using TreeViewer.TestUtilities.Assertions;
using TreeViewer.Core.Trees;

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
                Assert.Equal(5, options.CladeLabelLineThickness);
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
            var applied = new TreeStyle()
            {
                XScale = 10,
                YScale = 10,
                BranchThickness = 10,
                ShowLeafLabels = false,
                LeafLabelsFontSize = 1,
                ShowCladeLabels = false,
                CladeLabelsFontSize = 1,
                ShowNodeValues = false,
                CladeLabelLineThickness = 1,
                NodeValueFontSize = 1,
                NodeValueType = CladeValueType.BranchLength,
                ShowBranchValues = false,
                BranchValueFontSize = 1,
                BranchValueType = CladeValueType.Supports,
                BranchValueHideRegexPattern = "100/100",
                ShowBranchDecorations = false,
                DecorationStyles = [
                    new BranchDecorationStyle()
                    {
                        RegexPattern = "100/100",
                        ShapeSize = 1,
                        DecorationType = BranchDecorationType.OpenCircle,
                        ShapeColor = "red",
                    }],
                ShowScaleBar = false,
                ScaleBarValue = 3,
                ScaleBarFontSize = 50,
                ScaleBarThickness = 10,
                CollapseType = CladeCollapseType.Constant,
                CollapsedConstantWidth = 2,
            };
            style.ApplyValues(applied);

            CustomizedAssertions.Equal(applied, style);
        }

        [Fact]
        public void Clone()
        {
            style.XScale = 10;
            style.YScale = 10;
            style.BranchThickness = 10;
            style.ShowLeafLabels = false;
            style.LeafLabelsFontSize = 1;
            style.ShowCladeLabels = false;
            style.CladeLabelsFontSize = 1;
            style.CladeLabelLineThickness = 1;
            style.ShowNodeValues = false;
            style.NodeValueFontSize = 1;
            style.NodeValueType = CladeValueType.BranchLength;
            style.ShowBranchValues = false;
            style.BranchValueFontSize = 1;
            style.BranchValueType = CladeValueType.Supports;
            style.BranchValueHideRegexPattern = "100/100";
            style.ShowBranchDecorations = false;
            style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    RegexPattern = "100/100",
                    ShapeSize = 1,
                    DecorationType = BranchDecorationType.OpenCircle,
                    ShapeColor = "red",
                }];
            style.ShowScaleBar = false;
            style.ScaleBarValue = 3;
            style.ScaleBarFontSize = 50;
            style.ScaleBarThickness = 10;
            style.CollapseType = CladeCollapseType.Constant;
            style.CollapsedConstantWidth = 2;

            TreeStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            style.XScale = 10;
            style.YScale = 10;
            style.BranchThickness = 10;
            style.ShowLeafLabels = false;
            style.LeafLabelsFontSize = 1;
            style.ShowCladeLabels = false;
            style.CladeLabelsFontSize = 1;
            style.CladeLabelLineThickness = 1;
            style.ShowNodeValues = false;
            style.NodeValueFontSize = 1;
            style.NodeValueType = CladeValueType.BranchLength;
            style.ShowBranchValues = false;
            style.BranchValueFontSize = 1;
            style.BranchValueType = CladeValueType.Supports;
            style.BranchValueHideRegexPattern = "100/100";
            style.ShowBranchDecorations = false;
            style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    RegexPattern = "100/100",
                    ShapeSize = 1,
                    DecorationType = BranchDecorationType.OpenCircle,
                    ShapeColor = "red",
                }];
            style.ShowScaleBar = false;
            style.ScaleBarValue = 3;
            style.ScaleBarFontSize = 50;
            style.ScaleBarThickness = 10;
            style.CollapseType = CladeCollapseType.Constant;
            style.CollapsedConstantWidth = 2;

            var cloned = (TreeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
