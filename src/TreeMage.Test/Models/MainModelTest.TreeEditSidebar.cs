using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;
using TreeMage.Data;

namespace TreeMage.Models
{
    public partial class MainModelTest
    {
        #region Layout

        [Fact]
        public async Task CollapseType_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.CollapseType, CladeCollapseType.Constant, (p, v) => Assert.Equal(v, tree.Style.CollapseType), (p, v) => Assert.Equal(v, tree.Style.CollapseType));
        }

        [Fact]
        public async Task CollapsedConstantWidth_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.CollapsedConstantWidth, 2, (p, v) => Assert.Equal(v, tree.Style.CollapsedConstantWidth), (p, v) => Assert.Equal(v, tree.Style.CollapsedConstantWidth));
        }

        #endregion Layout

        #region Tree

        [Fact]
        public async Task XScale_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.XScale, 100, (p, v) => Assert.Equal(v, tree.Style.XScale), (p, v) => Assert.Equal(v, tree.Style.XScale));
        }

        [Fact]
        public async Task YScale_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.YScale, 100, (p, v) => Assert.Equal(v, tree.Style.YScale), (p, v) => Assert.Equal(v, tree.Style.YScale));
        }

        [Fact]
        public async Task BranchThickness_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.BranchThickness, 10, (p, v) => Assert.Equal(v, tree.Style.BranchThickness), (p, v) => Assert.Equal(v, tree.Style.BranchThickness));
        }

        #endregion Tree

        #region Search

        [Fact]
        public void Search_OnTreeMissing()
        {
            model.TargetTree.Value = null;
            model.SearchQuery.Value = "hoge";
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Empty(model.FocusedSvgElementIdList);
            });
        }

        [Fact]
        public void Search_OnEmptyQuery()
        {
            model.SearchQuery.Value = string.Empty;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Empty(model.FocusedSvgElementIdList);
            });
        }

        [Fact]
        public void Search_OnCompatibleState_AsNotFound()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();

            model.SearchQuery.Value = "bbaa";
            model.SearchOnIgnoreCase.Value = false;
            model.SearchTarget.Value = TreeSearchTarget.Taxon;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Empty(model.FocusedSvgElementIdList);
                Assert.Empty(updatedProperties);
            });
        }

        [Fact]
        public async Task Search_OnCompatibleState_AsFindTaxon()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Node;
            updatedProperties.Clear();

            model.SearchQuery.Value = "BBAA";
            model.SearchTarget.Value = TreeSearchTarget.Taxon;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Equal(SelectionMode.Taxa, model.SelectionTarget.Value);
                CladeId clade = Assert.Single(model.FocusedSvgElementIdList);
                Assert.True(clade.Clade.IsLeaf);
                Assert.Equal("BBAA", clade.Clade.Taxon);
                Assert.Equal(["FocusedSvgElementIdList", "FocusedSvgElementIdList"], updatedProperties);
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        [Fact]
        public async Task Search_OnCompatibleState_AsFindSupport()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();

            model.SearchQuery.Value = "100/100";
            model.SearchTarget.Value = TreeSearchTarget.Supports;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Equal(SelectionMode.Node, model.SelectionTarget.Value);
                CladeId clade = Assert.Single(model.FocusedSvgElementIdList);
                Assert.False(clade.Clade.IsLeaf);
                Assert.Equal("100/100", clade.Clade.Supports);
                Assert.Equal(["FocusedSvgElementIdList", "FocusedSvgElementIdList"], updatedProperties);
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        [Fact]
        public async Task Search_OnCompatibleState_AsCaseInsensitive()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();

            model.SearchQuery.Value = "bbaa";
            model.SearchTarget.Value = TreeSearchTarget.Taxon;
            model.SearchOnIgnoreCase.Value = true;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Equal(SelectionMode.Taxa, model.SelectionTarget.Value);
                CladeId clade = Assert.Single(model.FocusedSvgElementIdList);
                Assert.True(clade.Clade.IsLeaf);
                Assert.Equal("BBAA", clade.Clade.Taxon);
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        [Fact]
        public async Task Search_OnCompatibleState_AsUseRegexAndCaseSensitive()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();

            model.SearchQuery.Value = "^BAA$";
            model.SearchTarget.Value = TreeSearchTarget.Taxon;
            model.SearchWithRegex.Value = true;
            model.SearchOnIgnoreCase.Value = false;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Equal(SelectionMode.Taxa, model.SelectionTarget.Value);
                CladeId clade = Assert.Single(model.FocusedSvgElementIdList);
                Assert.True(clade.Clade.IsLeaf);
                Assert.Equal("BAA", clade.Clade.Taxon);
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        [Fact]
        public async Task Search_OnCompatibleState_AsUseRegexAndCaseInsensitive()
        {
            model.FocusAll();
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();

            model.SearchQuery.Value = "^baa$";
            model.SearchTarget.Value = TreeSearchTarget.Taxon;
            model.SearchWithRegex.Value = true;
            model.SearchOnIgnoreCase.Value = true;
            model.Search();

            Assert.Multiple(() =>
            {
                Assert.Equal(SelectionMode.Taxa, model.SelectionTarget.Value);
                CladeId clade = Assert.Single(model.FocusedSvgElementIdList);
                Assert.True(clade.Clade.IsLeaf);
                Assert.Equal("BAA", clade.Clade.Taxon);
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        #endregion Search

        #region LeafLabels

        [Fact]
        public async Task ShowLeafLabels_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowLeafLabels, false, (p, v) => Assert.Equal(v, tree.Style.ShowLeafLabels), (p, v) => Assert.Equal(v, tree.Style.ShowLeafLabels));
        }

        [Fact]
        public async Task LeafLabelsFontSize_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.LeafLabelsFontSize, 30, (p, v) => Assert.Equal(v, tree.Style.LeafLabelsFontSize), (p, v) => Assert.Equal(v, tree.Style.LeafLabelsFontSize));
        }

        #endregion LeafLabels

        #region CladeLabels

        [Fact]
        public async Task ShowCladeLabels()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowCladeLabels, false, (p, v) => Assert.Equal(v, tree.Style.ShowCladeLabels), (p, v) => Assert.Equal(v, tree.Style.ShowCladeLabels));
        }

        [Fact]
        public async Task CladeLabelsFontSize_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.CladeLabelsFontSize, 30, (p, v) => Assert.Equal(v, tree.Style.CladeLabelsFontSize), (p, v) => Assert.Equal(v, tree.Style.CladeLabelsFontSize));
        }

        [Fact]
        public async Task CladeLabelLineThickness_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.CladeLabelsLineThickness, 3, (p, v) => Assert.Equal(v, tree.Style.CladeLabelsLineThickness), (p, v) => Assert.Equal(v, tree.Style.CladeLabelsLineThickness));
        }

        #endregion CladeLabels

        #region NodeValues

        [Fact]
        public async Task ShowNodeValues_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowNodeValues, true, (p, v) => Assert.Equal(v, tree.Style.ShowNodeValues), (p, v) => Assert.Equal(v, tree.Style.ShowNodeValues));
        }

        [Fact]
        public async Task NodeValueType_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.NodeValueType, CladeValueType.BranchLength, (p, v) => Assert.Equal(tree.Style.NodeValueType, v), (p, v) => Assert.Equal(tree.Style.NodeValueType, v));
        }

        [Fact]
        public async Task NodeValueFontSize_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.NodeValueFontSize, 1, (p, v) => Assert.Equal(tree.Style.NodeValueFontSize, v), (p, v) => Assert.Equal(tree.Style.NodeValueFontSize, v));
        }

        #endregion NodeValues

        #region BranchValues

        [Fact]
        public async Task ShowBranchValues_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowBranchValues, false, (p, v) => Assert.Equal(v, tree.Style.ShowBranchValues), (p, v) => Assert.Equal(v, tree.Style.ShowBranchValues));
        }

        [Fact]
        public async Task BranchValueType_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.BranchValueType, CladeValueType.BranchLength, (p, v) => Assert.Equal(tree.Style.BranchValueType, v), (p, v) => Assert.Equal(tree.Style.BranchValueType, v));
        }

        [Fact]
        public async Task BranchValueFontSize_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.BranchValueFontSize, 1, (p, v) => Assert.Equal(tree.Style.BranchValueFontSize, v), (p, v) => Assert.Equal(tree.Style.BranchValueFontSize, v));
        }

        [Fact]
        public async Task BranchValueHideRegexPattern_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.BranchValueHideRegexPattern, "hoge", (p, v) => Assert.Equal(tree.Style.BranchValueHideRegexPattern, v), (p, v) => Assert.Equal(tree.Style.BranchValueHideRegexPattern, v));
        }

        #endregion BranchValues

        #region BranchDecorations

        [Fact]
        public async Task ShowBranchDecorations_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowBranchDecorations, false, (p, v) => Assert.Equal(v, tree.Style.ShowBranchDecorations), (p, v) => Assert.Equal(v, tree.Style.ShowBranchDecorations));
        }

        [Fact]
        public async Task AddNewBranchDecoration()
        {
            BranchDecorationModel firstDecoration = model.BranchDecorations[0];
            model.AddNewBranchDecoration();

            Assert.Multiple(() =>
            {
                Assert.Equal(2, tree.Style.DecorationStyles.Length);
                Assert.Equal(2, model.BranchDecorations.Count);
                Assert.Same(firstDecoration.Style, tree.Style.DecorationStyles[0]);
                Assert.NotSame(firstDecoration.Style, tree.Style.DecorationStyles[1]);
                Assert.Same(firstDecoration, model.BranchDecorations[0]);
                Assert.NotSame(firstDecoration, model.BranchDecorations[1]);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.Single(model.BranchDecorations, firstDecoration);
                Assert.Single(tree.Style.DecorationStyles, firstDecoration.Style);
                Assert.True(undoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.Equal(2, tree.Style.DecorationStyles.Length);
                Assert.Equal(2, model.BranchDecorations.Count);
                Assert.Same(firstDecoration.Style, tree.Style.DecorationStyles[0]);
                Assert.NotSame(firstDecoration.Style, tree.Style.DecorationStyles[1]);
                Assert.Same(firstDecoration, model.BranchDecorations[0]);
                Assert.NotSame(firstDecoration, model.BranchDecorations[1]);
                Assert.True(redoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        #endregion BranchDecorations

        #region Scalebar

        [Fact]
        public async Task ShowScaleBar_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ShowScaleBar, false, (p, v) => Assert.Equal(v, tree.Style.ShowScaleBar), (p, v) => Assert.Equal(v, tree.Style.ShowScaleBar));
        }

        [Fact]
        public async Task ScaleBarValue_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ScaleBarValue, 3, (p, v) => Assert.Equal(v, tree.Style.ScaleBarValue), (p, v) => Assert.Equal(v, tree.Style.ScaleBarValue));
        }

        [Fact]
        public async Task ScaleBarFontSize_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ScaleBarFontSize, 1, (p, v) => Assert.Equal(v, tree.Style.ScaleBarFontSize), (p, v) => Assert.Equal(v, tree.Style.ScaleBarFontSize));
        }

        [Fact]
        public async Task ScaleBarThickness_Set()
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.ScaleBarThickness, 10, (p, v) => Assert.Equal(v, tree.Style.ScaleBarThickness), (p, v) => Assert.Equal(v, tree.Style.ScaleBarThickness));
        }

        #endregion Scalebar
    }
}
