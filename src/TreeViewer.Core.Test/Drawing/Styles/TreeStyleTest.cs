using System.Text.RegularExpressions;
using TreeViewer.Core.Assertions;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing.Styles
{
    public partial class TreeStyleTest
    {
        private readonly TreeStyle style;

        public TreeStyleTest()
        {
            style = new TreeStyle();
        }

        [GeneratedRegex("100/100")]
        private static partial Regex GetDummyDecorationRegex();

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
                ShowNodeValues = false,
                NodeValueFontSize = 1,
                NodeValueType = CladeValueType.BranchLength,
                ShowBranchValues = false,
                BranchValueFontSize = 1,
                BranchValueType = CladeValueType.Supports,
                ShowBranchDecorations = false,
                DecorationStyles = [
                    new BranchDecorationStyle()
                    {
                        Regex = GetDummyDecorationRegex(),
                        ShapeSize = 1,
                        DecorationType = BranchDecorationType.OpenCircle,
                        ShapeColor = "red",
                    }],
                ShowScaleBar = false,
                ScaleBarValue = 3,
                ScaleBarFontSize = 50,
                ScaleBarThickness = 10,
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
            style.ShowNodeValues = false;
            style.NodeValueFontSize = 1;
            style.NodeValueType = CladeValueType.BranchLength;
            style.ShowBranchValues = false;
            style.BranchValueFontSize = 1;
            style.BranchValueType = CladeValueType.Supports;
            style.ShowBranchDecorations = false;
            style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    Regex = GetDummyDecorationRegex(),
                    ShapeSize = 1,
                    DecorationType = BranchDecorationType.OpenCircle,
                    ShapeColor = "red",
                }];
            style.ShowScaleBar = false;
            style.ScaleBarValue = 3;
            style.ScaleBarFontSize = 50;
            style.ScaleBarThickness = 10;

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
            style.ShowNodeValues = false;
            style.NodeValueFontSize = 1;
            style.NodeValueType = CladeValueType.BranchLength;
            style.ShowBranchValues = false;
            style.BranchValueFontSize = 1;
            style.BranchValueType = CladeValueType.Supports;
            style.ShowBranchDecorations = false;
            style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    Regex = GetDummyDecorationRegex(),
                    ShapeSize = 1,
                    DecorationType = BranchDecorationType.OpenCircle,
                    ShapeColor = "red",
                }];
            style.ShowScaleBar = false;
            style.ScaleBarValue = 3;
            style.ScaleBarFontSize = 50;
            style.ScaleBarThickness = 10;

            var cloned = (TreeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Ctors
    }
}
