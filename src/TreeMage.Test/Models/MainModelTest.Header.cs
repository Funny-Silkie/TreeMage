using TreeMage.Core.Trees;
using TreeMage.Data;
using TreeMage.TestUtilities;
using TreeMage.TestUtilities.Assertions;

namespace TreeMage.Models
{
    public partial class MainModelTest
    {
        [Fact]
        public async Task TreeIndex_Set()
        {
            Tree secondTree = DummyData.CreateTree();
            secondTree.Style.ApplyValues(DummyData.CreateTreeStyle());
            model.Trees.Add(secondTree);
            updatedProperties.Clear();

            await PropertySetTest(model.TreeIndex, 2, (p, v) => Assert.Multiple(() =>
            {
                Assert.Equal(["FocusedSvgElementIdList", "TargetTree"], updatedProperties);
                Assert.NotNull(model.TargetTree.Value);
                Assert.NotSame(tree, model.TargetTree.Value);
                Assert.Same(secondTree, model.TargetTree.Value);
                Assert.Empty(model.FocusedSvgElementIdList);

                var dummyTree = new Tree(new Clade());
                model.ApplyTreeStyle(dummyTree);
                CustomizedAssertions.Equal(dummyTree.Style, model.TargetTree.Value.Style);
            }), (p, v) => Assert.Multiple(() =>
            {
                Assert.Equal(["FocusedSvgElementIdList", "TargetTree"], updatedProperties);
                Assert.NotNull(model.TargetTree.Value);
                Assert.Same(tree, model.TargetTree.Value);
                Assert.NotSame(secondTree, model.TargetTree.Value);
                Assert.Empty(model.FocusedSvgElementIdList);

                var dummyTree = new Tree(new Clade());
                model.ApplyTreeStyle(dummyTree);
                CustomizedAssertions.Equal(dummyTree.Style, model.TargetTree.Value.Style);
            }));
        }

        [Theory]
        [InlineData(TreeEditMode.Reroot)]
        [InlineData(TreeEditMode.Swap)]
        [InlineData(TreeEditMode.Subtree)]
        public async Task EditMode_Set(TreeEditMode mode)
        {
            await PropertySetTestAsOnlyTargetTreeUpdated(model.EditMode, mode);
        }

        [Fact]
        public async Task SelectionTarget_Set_OnNothingFocused()
        {
            model.UnfocusAll();
            updatedProperties.Clear();

            model.SelectionTarget.Value = SelectionMode.Node;
            Assert.Multiple(() =>
            {
                Assert.Empty(model.FocusedSvgElementIdList);
                Assert.Empty(updatedProperties);
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }

        [Fact]
        public async Task SelectionTarget_Set_OnFocused()
        {
            Clade focused = tree.Root.Children[0];
            model.Focus(focused);
            updatedProperties.Clear();

            model.SelectionTarget.Value = SelectionMode.Taxa;
            Assert.Multiple(() =>
            {
                CladeId focusedId = Assert.Single(model.FocusedSvgElementIdList);
                Assert.Same(focused, focusedId.Clade);
                Assert.Equal(CladeIdSuffix.Leaf, focusedId.Suffix);
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });

            bool undoSuccess = await model.Undo();
            Assert.False(undoSuccess);
        }
    }
}
