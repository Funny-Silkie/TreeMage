using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;
using TreeMage.Data;

namespace TreeMage.Models
{
    public partial class MainModelTest
    {
        #region Ctors

        [Fact]
        public async Task Ctor()
        {
            var model = new MainModel();

            Assert.Multiple(() =>
            {
                Assert.Empty(model.Trees);
                Assert.Null(model.TargetTree.Value);
                Assert.Empty(model.FocusedSvgElementIdList);
                Assert.Null(model.ProjectPath.Value);
                Assert.True(model.Saved.Value);

                #region Header

                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(0, model.MaxTreeIndex.Value);
                Assert.Equal(TreeEditMode.Select, model.EditMode.Value);
                Assert.Equal(SelectionMode.Node, model.SelectionTarget.Value);

                #endregion Header

                #region TreeEditSidebar

                Assert.Equal(CladeCollapseType.TopMax, model.CollapseType.Value);
                Assert.Equal(1, model.CollapsedConstantWidth.Value);

                Assert.Equal(300, model.XScale.Value);
                Assert.Equal(30, model.YScale.Value);
                Assert.Equal(1, model.BranchThickness.Value);
                Assert.Equal(0.1, model.DefaultBranchLength.Value);

                Assert.Empty(model.SearchQuery.Value);
                Assert.Equal(TreeSearchTarget.Taxon, model.SearchTarget.Value);
                Assert.False(model.SearchOnIgnoreCase.Value);
                Assert.False(model.SearchWithRegex.Value);

                Assert.True(model.ShowLeafLabels.Value);
                Assert.Equal(20, model.LeafLabelsFontSize.Value);

                Assert.True(model.ShowCladeLabels.Value);
                Assert.Equal(20, model.CladeLabelsFontSize.Value);
                Assert.Equal(5, model.CladeLabelsLineThickness.Value);

                Assert.False(model.ShowNodeValues.Value);
                Assert.Equal(15, model.NodeValueFontSize.Value);
                Assert.Equal(CladeValueType.Supports, model.NodeValueType.Value);

                Assert.True(model.ShowBranchValues.Value);
                Assert.Equal(15, model.BranchValueFontSize.Value);
                Assert.Equal(CladeValueType.Supports, model.BranchValueType.Value);
                Assert.Null(model.BranchValueHideRegexPattern.Value);

                Assert.True(model.ShowBranchDecorations.Value);
                Assert.Empty(model.BranchDecorations);

                Assert.True(model.ShowScaleBar.Value);
                Assert.Equal(0.1, model.ScaleBarValue.Value);
                Assert.Equal(25, model.ScaleBarFontSize.Value);
                Assert.Equal(5, model.ScaleBarThickness.Value);

                #endregion TreeEditSidebar
            });

            Assert.False(await model.Undo());
            Assert.False(await model.Redo());
        }

        #endregion Ctors
    }
}
