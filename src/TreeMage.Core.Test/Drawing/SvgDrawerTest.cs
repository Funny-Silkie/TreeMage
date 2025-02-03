using Svg;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Exporting;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities;

namespace TreeMage.Core.Drawing
{
    public class SvgDrawerTest
    {
        // lang=regex
        private const string DecorationRegex = @"^([8-9]\d|100)/([8-9]\d|100)$";

        private readonly Tree tree;
        private readonly SvgDrawer drawer;
        private readonly ExportOptions exportOptions;

        public SvgDrawerTest()
        {
            tree = DummyData.CreateTree();
            tree.Style.XScale = 30;
            tree.Style.ScaleBarValue = 1;
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.CladeLabel = "hoge";
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.ShadeColor = "lightblue";
            drawer = new SvgDrawer();
            exportOptions = new ExportOptions();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new SvgDrawer());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Document_Get_OnDefault()
        {
            Assert.Throws<InvalidOperationException>(() => _ = drawer.Document);
        }

        #endregion Properties

        #region Methods

        [Fact]
        public void Draw_WithNullTree()
        {
            Assert.Throws<ArgumentNullException>(() => ((ITreeDrawer)drawer).Draw(null!, exportOptions));
        }

        [Fact]
        public void Draw_WithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => ((ITreeDrawer)drawer).Draw(tree, null!));
        }

        [Fact]
        public void Draw_AsPositive_Minumum()
        {
            tree.Style.ShowLeafLabels = false;
            tree.Style.ShowCladeLabels = false;
            tree.Style.ShowNodeValues = false;
            tree.Style.ShowBranchValues = false;
            tree.Style.ShowBranchDecorations = false;
            tree.Style.ShowScaleBar = false;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement? shadesGroup = svg.GetElementById("shades");
                Assert.NotNull(shadesGroup);
                Assert.IsType<SvgGroup>(shadesGroup);
                Assert.Single(shadesGroup.Children);
                Assert.True(shadesGroup.Children.All(x => x is SvgRectangle));

                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                Assert.Null(svg.GetElementById("leaf-labels"));
                Assert.Null(svg.GetElementById("clade-labels"));
                Assert.Null(svg.GetElementById("node-values"));
                Assert.Null(svg.GetElementById("branch-values"));
                Assert.Null(svg.GetElementById("branch-decorations"));
                Assert.Null(svg.GetElementById("scale-bar"));
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputLeafLabels()
        {
            tree.Style.ShowLeafLabels = true;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? leavesGroup = svg.GetElementById("leaf-labels");
                Assert.NotNull(leavesGroup);
                Assert.IsType<SvgGroup>(leavesGroup);
                Assert.Equal(7, leavesGroup.Children.Count);
                Assert.True(leavesGroup.Children.All(x => x is SvgText));
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputNodeValues()
        {
            tree.Style.ShowNodeValues = true;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? nodeValuesGroup = svg.GetElementById("node-values");
                Assert.NotNull(nodeValuesGroup);
                Assert.IsType<SvgGroup>(nodeValuesGroup);
                Assert.Equal(4, nodeValuesGroup.Children.Count);
                Assert.True(nodeValuesGroup.Children.All(x => x is SvgText));
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputBranchValues()
        {
            tree.Style.ShowBranchValues = true;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? branchValuesGroup = svg.GetElementById("branch-values");
                Assert.NotNull(branchValuesGroup);
                Assert.IsType<SvgGroup>(branchValuesGroup);
                Assert.Equal(11, branchValuesGroup.Children.Count);
                Assert.True(branchValuesGroup.Children.All(x => x is SvgText));
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputBranchDecorationsAsEmpty()
        {
            tree.Style.ShowBranchDecorations = true;
            tree.Style.DecorationStyles = [];

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? branchDecorationsGroup = svg.GetElementById("branch-decorations");
                Assert.NotNull(branchDecorationsGroup);
                Assert.IsType<SvgGroup>(branchDecorationsGroup);
                Assert.Empty(branchDecorationsGroup.Children);
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputBranchDecorationsAsNotEmpty()
        {
            tree.Style.ShowBranchDecorations = true;
            tree.Style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    RegexPattern = DecorationRegex,
                    DecorationType = BranchDecorationType.ClosedCircle,
                },
            ];

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? branchDecorationsGroup = svg.GetElementById("branch-decorations");
                Assert.NotNull(branchDecorationsGroup);
                Assert.IsType<SvgGroup>(branchDecorationsGroup);
                Assert.Equal(2, branchDecorationsGroup.Children.Count);
                Assert.True(branchDecorationsGroup.Children.All(x => x is SvgCircle));
            });
        }

        [Fact]
        public void Draw_AsPositive_OnOutputScaleBar()
        {
            tree.Style.ShowScaleBar = true;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? scaleBarGroup = svg.GetElementById("scale-bar");
                Assert.NotNull(scaleBarGroup);
                Assert.IsType<SvgGroup>(scaleBarGroup);
                Assert.Equal(2, scaleBarGroup.Children.Count);
                Assert.IsType<SvgText>(scaleBarGroup.Children[0]);
                Assert.Equal(tree.Style.ScaleBarValue.ToString(), ((SvgText)scaleBarGroup.Children[0]).Text);
                Assert.IsType<SvgLine>(scaleBarGroup.Children[1]);
            });
        }

        [Fact]
        public void Draw_AsPositive_Maximum()
        {
            tree.Style.ShowLeafLabels = true;
            tree.Style.ShowCladeLabels = true;
            tree.Style.ShowNodeValues = true;
            tree.Style.ShowBranchValues = true;
            tree.Style.ShowBranchDecorations = true;
            tree.Style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    RegexPattern = DecorationRegex,
                    DecorationType = BranchDecorationType.ClosedCircle,
                },
            ];
            tree.Style.ShowScaleBar = true;

            ((ITreeDrawer)drawer).Draw(tree, exportOptions);
            SvgDocument svg = drawer.Document;

            Assert.Multiple(() =>
            {
                Assert.Equal(459.2, svg.Width, 0.1);
                Assert.Equal(348, svg.Height, 0.1);

                SvgElement? shadesGroup = svg.GetElementById("shades");
                Assert.NotNull(shadesGroup);
                Assert.IsType<SvgGroup>(shadesGroup);
                Assert.Single(shadesGroup.Children);
                Assert.True(shadesGroup.Children.All(x => x is SvgRectangle));

                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? leafLabelsGroup = svg.GetElementById("leaf-labels");
                Assert.NotNull(leafLabelsGroup);
                Assert.IsType<SvgGroup>(leafLabelsGroup);
                Assert.Equal(7, leafLabelsGroup.Children.Count);
                Assert.True(leafLabelsGroup.Children.All(x => x is SvgText));

                SvgElement? cladeLabelsGroup = svg.GetElementById("clade-labels");
                Assert.NotNull(cladeLabelsGroup);
                Assert.IsType<SvgGroup>(cladeLabelsGroup);
                Assert.Single(cladeLabelsGroup.Children);
                Assert.True(cladeLabelsGroup.Children.All(x => x is SvgGroup));

                SvgElement? nodeValuesGroup = svg.GetElementById("node-values");
                Assert.NotNull(nodeValuesGroup);
                Assert.IsType<SvgGroup>(nodeValuesGroup);
                Assert.Equal(4, nodeValuesGroup.Children.Count);
                Assert.True(nodeValuesGroup.Children.All(x => x is SvgText));

                SvgElement? branchValuesGroup = svg.GetElementById("branch-values");
                Assert.NotNull(branchValuesGroup);
                Assert.IsType<SvgGroup>(branchValuesGroup);
                Assert.Equal(11, branchValuesGroup.Children.Count);
                Assert.True(branchValuesGroup.Children.All(x => x is SvgText));

                SvgElement? branchDecorationsGroup = svg.GetElementById("branch-decorations");
                Assert.NotNull(branchDecorationsGroup);
                Assert.IsType<SvgGroup>(branchDecorationsGroup);
                Assert.Equal(2, branchDecorationsGroup.Children.Count);
                Assert.True(branchDecorationsGroup.Children.All(x => x is SvgCircle));

                SvgElement? scaleBarGroup = svg.GetElementById("scale-bar");
                Assert.NotNull(scaleBarGroup);
                Assert.IsType<SvgGroup>(scaleBarGroup);
                Assert.Equal(2, scaleBarGroup.Children.Count);
                Assert.IsType<SvgText>(scaleBarGroup.Children[0]);
                Assert.Equal(tree.Style.ScaleBarValue.ToString(), ((SvgText)scaleBarGroup.Children[0]).Text);
                Assert.IsType<SvgLine>(scaleBarGroup.Children[1]);
            });
        }

        #endregion Methods
    }
}
