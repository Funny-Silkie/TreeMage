using TreeViewer.Core.Drawing;
using TreeViewer.Core.Trees;

namespace TreeViewer.Models
{
    public partial class MainModelTest
    {
        #region Layout

        [Fact]
        public async Task CollapseType_Set()
        {
            await PropertySetTest(model.CollapseType, CladeCollapseType.Constant, (p, v) => Assert.Equal(v, tree.Style.CollapseType), (p, v) => Assert.Equal(v, tree.Style.CollapseType));
        }

        [Fact]
        public async Task CollapsedConstantWidth_Set()
        {
            await PropertySetTest(model.CollapsedConstantWidth, 2, (p, v) => Assert.Equal(v, tree.Style.CollapsedConstantWidth), (p, v) => Assert.Equal(v, tree.Style.CollapsedConstantWidth));
        }

        #endregion Layout

        #region Tree

        [Fact]
        public async Task XScale_Set()
        {
            await PropertySetTest(model.XScale, 100, (p, v) => Assert.Equal(v, tree.Style.XScale), (p, v) => Assert.Equal(v, tree.Style.XScale));
        }

        [Fact]
        public async Task YScale_Set()
        {
            await PropertySetTest(model.YScale, 100, (p, v) => Assert.Equal(v, tree.Style.YScale), (p, v) => Assert.Equal(v, tree.Style.YScale));
        }

        [Fact]
        public async Task BranchThickness_Set()
        {
            await PropertySetTest(model.BranchThickness, 10, (p, v) => Assert.Equal(v, tree.Style.BranchThickness), (p, v) => Assert.Equal(v, tree.Style.BranchThickness));
        }

        #endregion Tree

        #region LeafLabels

        [Fact]
        public async Task ShowLeafLabels_Set()
        {
            await PropertySetTest(model.ShowLeafLabels, false, (p, v) => Assert.Equal(v, tree.Style.ShowLeafLabels), (p, v) => Assert.Equal(v, tree.Style.ShowLeafLabels));
        }

        [Fact]
        public async Task LeafLabelsFontSize_Set()
        {
            await PropertySetTest(model.LeafLabelsFontSize, 30, (p, v) => Assert.Equal(v, tree.Style.LeafLabelsFontSize), (p, v) => Assert.Equal(v, tree.Style.LeafLabelsFontSize));
        }

        #endregion LeafLabels

        #region CladeLabels

        [Fact]
        public async Task ShowCladeLabels()
        {
            await PropertySetTest(model.ShowCladeLabels, false, (p, v) => Assert.Equal(v, tree.Style.ShowCladeLabels), (p, v) => Assert.Equal(v, tree.Style.ShowCladeLabels));
        }

        [Fact]
        public async Task CladeLabelsFontSize_Set()
        {
            await PropertySetTest(model.CladeLabelsFontSize, 30, (p, v) => Assert.Equal(v, tree.Style.CladeLabelsFontSize), (p, v) => Assert.Equal(v, tree.Style.CladeLabelsFontSize));
        }

        [Fact]
        public async Task CladeLabelLineThickness_Set()
        {
            await PropertySetTest(model.CladeLabelsLineThickness, 3, (p, v) => Assert.Equal(v, tree.Style.CladeLabelsLineThickness), (p, v) => Assert.Equal(v, tree.Style.CladeLabelsLineThickness));
        }

        #endregion CladeLabels

        #region NodeValues

        [Fact]
        public async Task ShowNodeValues_Set()
        {
            await PropertySetTest(model.ShowNodeValues, true, (p, v) => Assert.Equal(v, tree.Style.ShowNodeValues), (p, v) => Assert.Equal(v, tree.Style.ShowNodeValues));
        }

        [Fact]
        public async Task NodeValueType_Set()
        {
            await PropertySetTest(model.NodeValueType, CladeValueType.BranchLength, (p, v) => Assert.Equal(tree.Style.NodeValueType, v), (p, v) => Assert.Equal(tree.Style.NodeValueType, v));
        }

        [Fact]
        public async Task NodeValueFontSize_Set()
        {
            await PropertySetTest(model.NodeValueFontSize, 1, (p, v) => Assert.Equal(tree.Style.NodeValueFontSize, v), (p, v) => Assert.Equal(tree.Style.NodeValueFontSize, v));
        }

        #endregion NodeValues

        #region BranchValues

        [Fact]
        public async Task ShowBranchValues_Set()
        {
            await PropertySetTest(model.ShowBranchValues, false, (p, v) => Assert.Equal(v, tree.Style.ShowBranchValues), (p, v) => Assert.Equal(v, tree.Style.ShowBranchValues));
        }

        [Fact]
        public async Task BranchValueType_Set()
        {
            await PropertySetTest(model.BranchValueType, CladeValueType.BranchLength, (p, v) => Assert.Equal(tree.Style.BranchValueType, v), (p, v) => Assert.Equal(tree.Style.BranchValueType, v));
        }

        [Fact]
        public async Task BranchValueFontSize_Set()
        {
            await PropertySetTest(model.BranchValueFontSize, 1, (p, v) => Assert.Equal(tree.Style.BranchValueFontSize, v), (p, v) => Assert.Equal(tree.Style.BranchValueFontSize, v));
        }

        [Fact]
        public async Task BranchValueHideRegexPattern_Set()
        {
            await PropertySetTest(model.BranchValueHideRegexPattern, "hoge", (p, v) => Assert.Equal(tree.Style.BranchValueHideRegexPattern, v), (p, v) => Assert.Equal(tree.Style.BranchValueHideRegexPattern, v));
        }

        #endregion BranchValues

        #region BranchDecorations

        [Fact]
        public async Task ShowBranchDecorations_Set()
        {
            await PropertySetTest(model.ShowBranchDecorations, false, (p, v) => Assert.Equal(v, tree.Style.ShowBranchDecorations), (p, v) => Assert.Equal(v, tree.Style.ShowBranchDecorations));
        }

        #endregion BranchDecorations

        #region Scalebar

        [Fact]
        public async Task ShowScaleBar_Set()
        {
            await PropertySetTest(model.ShowScaleBar, false, (p, v) => Assert.Equal(v, tree.Style.ShowScaleBar), (p, v) => Assert.Equal(v, tree.Style.ShowScaleBar));
        }

        [Fact]
        public async Task ScaleBarValue_Set()
        {
            await PropertySetTest(model.ScaleBarValue, 3, (p, v) => Assert.Equal(v, tree.Style.ScaleBarValue), (p, v) => Assert.Equal(v, tree.Style.ScaleBarValue));
        }

        [Fact]
        public async Task ScaleBarFontSize_Set()
        {
            await PropertySetTest(model.ScaleBarFontSize, 1, (p, v) => Assert.Equal(v, tree.Style.ScaleBarFontSize), (p, v) => Assert.Equal(v, tree.Style.ScaleBarFontSize));
        }

        [Fact]
        public async Task ScaleBarThickness_Set()
        {
            await PropertySetTest(model.ScaleBarThickness, 10, (p, v) => Assert.Equal(v, tree.Style.ScaleBarThickness), (p, v) => Assert.Equal(v, tree.Style.ScaleBarThickness));
        }

        #endregion Scalebar
    }
}
