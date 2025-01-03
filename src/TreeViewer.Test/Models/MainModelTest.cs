﻿using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Data;
using TreeViewer.TestUtilities;
using TreeViewer.TestUtilities.Assertions;

namespace TreeViewer.Models
{
    public partial class MainModelTest
    {
        private readonly Tree tree;
        private readonly MainModel model;
        private readonly List<string?> updatedProperties = [];

        public MainModelTest()
        {
            model = new MainModel();
            tree = DummyData.CreateTree();
            model.BranchDecorations.Add(new BranchDecorationModel(model, new BranchDecorationStyle()
            {
                RegexPattern = "hoge",
            }));
            model.Trees.Add(tree);
            model.TargetTree.Value = tree;
            model.ApplyTreeStyle(tree);

            model.ClearUndoQueue();
            model.PropertyChanged += (_, e) => updatedProperties.Add(e.PropertyName);
        }

        #region Instance Methods

        [Fact]
        public void NotifyTreeUpdated()
        {
            model.NotifyTreeUpdated();

            Assert.Single(updatedProperties, "TargetTree");
        }

        [Fact]
        public void ApplyTreeStyle()
        {
            Tree tree = DummyData.CreateTree();
            tree.Style.ApplyValues(DummyData.CreateTreeStyle());

            model.ApplyTreeStyle(tree);

            Assert.Multiple(() =>
            {
                Assert.Equal(300, tree.Style.XScale);
                Assert.Equal(30, tree.Style.YScale);
                Assert.Equal(1, tree.Style.BranchThickness);
                Assert.True(tree.Style.ShowLeafLabels);
                Assert.Equal(20, tree.Style.LeafLabelsFontSize);
                Assert.True(tree.Style.ShowCladeLabels);
                Assert.Equal(20, tree.Style.CladeLabelsFontSize);
                Assert.Equal(5, tree.Style.CladeLabelLineThickness);
                Assert.False(tree.Style.ShowNodeValues);
                Assert.Equal(CladeValueType.Supports, tree.Style.NodeValueType);
                Assert.Equal(15, tree.Style.NodeValueFontSize);
                Assert.True(tree.Style.ShowBranchValues);
                Assert.Equal(CladeValueType.Supports, tree.Style.BranchValueType);
                Assert.Equal(15, tree.Style.BranchValueFontSize);
                Assert.Null(tree.Style.BranchValueHideRegexPattern);
                Assert.True(tree.Style.ShowBranchDecorations);
                BranchDecorationStyle decorationStyle = Assert.Single(tree.Style.DecorationStyles);
                CustomizedAssertions.Equal(new BranchDecorationStyle() { RegexPattern = "hoge" }, decorationStyle);
                Assert.True(tree.Style.ShowScaleBar);
                Assert.Equal(0.1, tree.Style.ScaleBarValue);
                Assert.Equal(25, tree.Style.ScaleBarFontSize);
                Assert.Equal(5, tree.Style.ScaleBarThickness);
                Assert.Equal(CladeCollapseType.TopMax, tree.Style.CollapseType);
                Assert.Equal(1, tree.Style.CollapsedConstantWidth);
            });
        }

        [Fact]
        public void LoadTreeStyle()
        {
            Tree tree = DummyData.CreateTree();
            tree.Style.ApplyValues(DummyData.CreateTreeStyle());

            model.LoadTreeStyle(tree);

            Assert.Multiple(() =>
            {
                Assert.Equal(10, model.XScale.Value);
                Assert.Equal(10, model.YScale.Value);
                Assert.Equal(10, model.BranchThickness.Value);
                Assert.False(model.ShowLeafLabels.Value);
                Assert.Equal(1, model.LeafLabelsFontSize.Value);
                Assert.False(model.ShowCladeLabels.Value);
                Assert.Equal(1, model.CladeLabelsFontSize.Value);
                Assert.Equal(1, model.CladeLabelsLineThickness.Value);
                Assert.False(model.ShowNodeValues.Value);
                Assert.Equal(CladeValueType.BranchLength, model.NodeValueType.Value);
                Assert.Equal(1, model.NodeValueFontSize.Value);
                Assert.False(model.ShowBranchValues.Value);
                Assert.Equal(CladeValueType.Supports, model.BranchValueType.Value);
                Assert.Equal(1, model.BranchValueFontSize.Value);
                Assert.Equal("100/100", model.BranchValueHideRegexPattern.Value);
                Assert.False(model.ShowBranchDecorations.Value);
                BranchDecorationModel decoration = Assert.Single(model.BranchDecorations);
                Assert.Equal("100/100", decoration.TargetRegexPattern.Value);
                Assert.False(model.ShowScaleBar.Value);
                Assert.Equal(3, model.ScaleBarValue.Value);
                Assert.Equal(50, model.ScaleBarFontSize.Value);
                Assert.Equal(10, model.ScaleBarThickness.Value);
                Assert.Equal(CladeCollapseType.Constant, model.CollapseType.Value);
                Assert.Equal(2, model.CollapsedConstantWidth.Value);
            });
        }

        [Fact]
        public async Task ClearUndoQueue()
        {
            model.XScale.Value = 1;
            model.YScale.Value = 1;
            await model.Undo();

            model.ClearUndoQueue();

            Assert.False(await model.Undo());
            Assert.False(await model.Redo());
        }

        [Theory]
        [InlineData(SelectionMode.Node)]
        [InlineData(SelectionMode.Clade)]
        public void Focus_OnSelectNodeOrClade(SelectionMode mode)
        {
            model.SelectionTarget.Value = mode;
            model.FocusAll();
            updatedProperties.Clear();

            Clade target = tree.Root.Children[0];
            model.Focus(target);

            Assert.Multiple(() =>
            {
                Assert.Single(model.FocusedSvgElementIdList, new CladeId(target, CladeIdSuffix.Branch));
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });
        }

        [Fact]
        public void Focus_OnSelectTaxa()
        {
            model.SelectionTarget.Value = SelectionMode.Taxa;
            model.FocusAll();
            updatedProperties.Clear();

            Clade target = tree.Root.Children[0];
            model.Focus(target);

            Assert.Multiple(() =>
            {
                Assert.Single(model.FocusedSvgElementIdList, new CladeId(target, CladeIdSuffix.Leaf));
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });
        }

        [Fact]
        public void FocusAll_OnTreeMissing()
        {
            model.TargetTree.Value = null;
            model.FocusAll();

            Assert.Multiple(() =>
            {
                Assert.Empty(model.FocusedSvgElementIdList);
                Assert.Empty(updatedProperties);
            });
        }

        [Theory]
        [InlineData(SelectionMode.Node)]
        [InlineData(SelectionMode.Clade)]
        public void FocusAll_OnTreeExistsAndSelectNodeOrClade(SelectionMode mode)
        {
            model.SelectionTarget.Value = mode;
            updatedProperties.Clear();
            model.FocusAll();

            Assert.Multiple(() =>
            {
                Assert.Equal(12, model.FocusedSvgElementIdList.Count);
                Assert.True(model.FocusedSvgElementIdList.All(x => x.Suffix == CladeIdSuffix.Branch));
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });
        }

        [Fact]
        public void FocusAll_OnTreeExistsAndSelectTaxa()
        {
            model.SelectionTarget.Value = SelectionMode.Taxa;
            updatedProperties.Clear();
            model.FocusAll();

            Assert.Multiple(() =>
            {
                Assert.Equal(12, model.FocusedSvgElementIdList.Count);
                Assert.True(model.FocusedSvgElementIdList.All(x => x.Suffix == CladeIdSuffix.Leaf));
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });
        }

        [Fact]
        public void UnfocusAll()
        {
            model.FocusAll();
            updatedProperties.Clear();
            model.UnfocusAll();

            Assert.Multiple(() =>
            {
                Assert.Empty(model.FocusedSvgElementIdList);
                Assert.Single(updatedProperties, "FocusedSvgElementIdList");
            });
        }

        [Fact]
        public void Reroot_OnTreeMissing()
        {
            model.TargetTree.Value = null;

            model.Reroot(tree.Root, false);

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Null(model.TargetTree.Value);
            });
        }

        [Fact]
        public void Reroot_WithLeaf()
        {
            model.Reroot(tree.Root.Children[0], false);

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Same(tree, model.TargetTree.Value);
            });
        }

        [Fact]
        public void Reroot_WithOutsiderClade()
        {
            model.Reroot(new Clade(), false);

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Same(tree, model.TargetTree.Value);
            });
        }

        [Fact]
        public async Task Reroot_WithCompatibleClade()
        {
            model.Reroot(tree.Root.Children[1], true);

            Tree? rerooted = model.TargetTree.Value;
            Assert.Multiple(() =>
            {
                Assert.Single(updatedProperties, "TargetTree");
                Assert.NotSame(tree, rerooted);
                Assert.NotNull(rerooted);
                Assert.Same(rerooted, model.Trees[0]);
                CustomizedAssertions.Equal(tree.Style, rerooted.Style);
                Assert.Equal(2, rerooted.Root.Children.Count);
                Assert.Equal("30/45", rerooted.Root.Children[0].Supports);
                Assert.Equal("30/45", rerooted.Root.Children[1].Supports);
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Same(tree, model.Trees[0]);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Same(rerooted, model.TargetTree.Value);
                Assert.Same(rerooted, model.Trees[0]);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        [Fact]
        public void SwapSisters_OnTreeMissing()
        {
            model.TargetTree.Value = null;

            model.SwapSisters(tree.Root.Children[0], tree.Root.Children[1]);

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.Null(model.TargetTree.Value);
            });
        }

        [Fact]
        public void SwapSisters_WithRoot()
        {
            model.SwapSisters(tree.Root, tree.Root.Children[0]);
            model.SwapSisters(tree.Root.Children[0], tree.Root);

            Assert.Empty(updatedProperties);
        }

        [Fact]
        public void SwapSisters_WithSameClade()
        {
            model.SwapSisters(tree.Root.Children[0], tree.Root.Children[0]);

            Assert.Empty(updatedProperties);
        }

        [Fact]
        public async Task SwapSisters_WithCompatibleClade()
        {
            model.SwapSisters(tree.Root.Children[0], tree.Root.Children[1]);

            Assert.Multiple(() =>
            {
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Same(tree, model.Trees[0]);
                Assert.Equal("30/45", tree.Root.Children[0].Supports);
                Assert.Equal("A", tree.Root.Children[1].Taxon);
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Same(tree, model.Trees[0]);
                Assert.Equal("A", tree.Root.Children[0].Taxon);
                Assert.Equal("30/45", tree.Root.Children[1].Supports);
                Assert.Single(updatedProperties, "TargetTree");
            });

            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Equal("30/45", tree.Root.Children[0].Supports);
                Assert.Equal("A", tree.Root.Children[1].Taxon);
            });
        }

        [Fact]
        public void ExtractSubtree_OnTreeMissing()
        {
            model.TargetTree.Value = null;

            model.ExtractSubtree(tree.Root.Children[1]);

            Assert.Multiple(() =>
            {
                Assert.Null(model.TargetTree.Value);
                Assert.Single(model.Trees, tree);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Assert.Empty(updatedProperties);
            });
        }

        [Fact]
        public void ExtractSubtree_WithRoot()
        {
            model.ExtractSubtree(tree.Root);

            Assert.Multiple(() =>
            {
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Single(model.Trees, tree);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Assert.Empty(updatedProperties);
            });
        }

        [Fact]
        public void ExtractSubtree_WithLeaf()
        {
            model.ExtractSubtree(tree.Root.Children[0]);

            Assert.Multiple(() =>
            {
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Single(model.Trees, tree);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Assert.Empty(updatedProperties);
            });
        }

        [Fact]
        public async Task ExtractSubtree_WithCompatibleClade()
        {
            model.ExtractSubtree(tree.Root.Children[1].Children[1]);
            Tree? extracted = model.TargetTree.Value;

            Assert.Multiple(() =>
            {
                Assert.NotSame(tree, extracted);
                Assert.NotNull(extracted);
                Assert.Equal("((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2;", extracted.ToString());
                Assert.Same(extracted, model.Trees[1]);
                CustomizedAssertions.Equal(tree.Style, extracted.Style);
                Assert.Equal(2, model.TreeIndex.Value);
                Assert.Equal(2, model.MaxTreeIndex.Value);
                Assert.Equal(2, model.Trees.Count);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Assert.Single(model.Trees, tree);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.NotSame(tree, extracted);
                Assert.Same(extracted, model.TargetTree.Value);
                Assert.Same(extracted, model.Trees[1]);
                Assert.Equal(2, model.TreeIndex.Value);
                Assert.Equal(2, model.MaxTreeIndex.Value);
                Assert.Equal(2, model.Trees.Count);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        [Fact]
        public void CollapseClade_OnTreeMissing()
        {
            model.SelectionTarget.Value = SelectionMode.Node;
            model.TargetTree.Value = null;
            model.Focus(tree.Root.Children[0]);
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
            });
        }

        [Fact]
        public void CollapseClade_OnNoOrMultipleSelection()
        {
            model.UnfocusAll();
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
            });
        }

        [Fact]
        public void CollapseClade_OnRootSelected()
        {
            model.SelectionTarget.Value = SelectionMode.Node;
            model.Focus(tree.Root);
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
            });
        }

        [Fact]
        public void CollapseClade_OnLeafSelected()
        {
            model.SelectionTarget.Value = SelectionMode.Node;
            model.Focus(tree.Root.Children[0]);
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
            });
        }

        [Fact]
        public void CollapseClade_OnTaxaSelection()
        {
            model.SelectionTarget.Value = SelectionMode.Taxa;
            model.Focus(tree.Root.Children[1].Children[0]);
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Empty(updatedProperties);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
            });
        }

        [Fact]
        public async Task CollapseClade_OnCompatibleCladeSelected()
        {
            model.SelectionTarget.Value = SelectionMode.Node;
            model.Focus(tree.Root.Children[1].Children[0]);
            updatedProperties.Clear();

            model.CollapseClade();

            Assert.Multiple(() =>
            {
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Single(model.Trees, tree);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Clade collapsed = Assert.Single(tree.GetAllClades(), x => x.Style.Collapsed);
                Assert.DoesNotContain(tree.GetAllClades().Where(x => x != collapsed), x => x.Style.Collapsed);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.DoesNotContain(tree.GetAllClades(), x => x.Style.Collapsed);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Clade collapsed = Assert.Single(tree.GetAllClades(), x => x.Style.Collapsed);
                Assert.DoesNotContain(tree.GetAllClades().Where(x => x != collapsed), x => x.Style.Collapsed);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OrderByBranchLength_OnTreeMissing(bool descending)
        {
            model.TargetTree.Value = null;
            model.OrderByBranchLength(descending);

            Assert.Multiple(() =>
            {
                Assert.Null(model.TargetTree.Value);
                Assert.Empty(updatedProperties);
            });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task OrderByBranchLength_OnTreeExisting(bool descending)
        {
            Tree beforeOperation = tree.Clone();
            Tree expectedOrdered = tree.Clone();
            expectedOrdered.OrderByLength(descending);

            model.OrderByBranchLength(descending);
            Tree? actualOrdered = model.TargetTree.Value;

            Assert.Multiple(() =>
            {
                Assert.NotNull(actualOrdered);
                Assert.NotSame(tree, actualOrdered);
                CustomizedAssertions.Equal(expectedOrdered, actualOrdered);
                Assert.Single(model.Trees, actualOrdered);
                Assert.Equal(1, model.TreeIndex.Value);
                Assert.Equal(1, model.MaxTreeIndex.Value);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool undoSuccess = await model.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Same(tree, model.TargetTree.Value);
                Assert.Single(model.Trees, tree);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await model.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Same(actualOrdered, model.TargetTree.Value);
                Assert.Single(model.Trees, actualOrdered);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        #endregion Instance Methods
    }
}
