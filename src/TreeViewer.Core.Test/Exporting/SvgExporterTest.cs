using Svg;
using TreeViewer.TestUtilities.Assertions;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    public partial class SvgExporterTest
    {
        private const string outputPath = "test.svg";

        // lang=regex
        private const string DecorationRegex = @"^([8-9]\d|100)/([8-9]\d|100)$";

        private readonly Tree tree;
        private readonly SvgExporter exporter;
        private readonly ExportOptions exportOptions;

        public SvgExporterTest()
        {
            tree = TreeTest.CreateDummyTree();
            tree.Style.XScale = 30;
            tree.Style.ScaleBarValue = 1;
            exporter = new SvgExporter();
            exportOptions = new ExportOptions();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new SvgExporter());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Type_Get()
        {
            Assert.Equal(ExportType.Svg, exporter.Type);
        }

        #endregion Properties

        #region Static Methods

        [Fact]
        public void CreateSvg_Minumum()
        {
            tree.Style.ShowLeafLabels = false;
            tree.Style.ShowNodeValues = false;
            tree.Style.ShowBranchValues = false;
            tree.Style.ShowBranchDecorations = false;
            tree.Style.ShowScaleBar = false;

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                Assert.Null(svg.GetElementById("leaves"));
                Assert.Null(svg.GetElementById("node-values"));
                Assert.Null(svg.GetElementById("branch-values"));
                Assert.Null(svg.GetElementById("branch-decorations"));
                Assert.Null(svg.GetElementById("scale-bar"));
            });
        }

        [Fact]
        public void CreateSvg_OnOutputLeaves()
        {
            tree.Style.ShowLeafLabels = true;

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

            Assert.Multiple(() =>
            {
                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? leavesGroup = svg.GetElementById("leaves");
                Assert.NotNull(leavesGroup);
                Assert.IsType<SvgGroup>(leavesGroup);
                Assert.Equal(7, leavesGroup.Children.Count);
                Assert.True(leavesGroup.Children.All(x => x is SvgText));
            });
        }

        [Fact]
        public void CreateSvg_OnOutputNodeValues()
        {
            tree.Style.ShowNodeValues = true;

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

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
        public void CreateSvg_OnOutputBranchValues()
        {
            tree.Style.ShowBranchValues = true;

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

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
        public void CreateSvg_OnOutputBranchDecorationsAsEmpty()
        {
            tree.Style.ShowBranchDecorations = true;
            tree.Style.DecorationStyles = [];

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

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
        public void CreateSvg_OnOutputBranchDecorationsAsNotEmpty()
        {
            tree.Style.ShowBranchDecorations = true;
            tree.Style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    RegexPattern = DecorationRegex,
                    DecorationType = BranchDecorationType.ClosedCircle,
                },
            ];

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

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
        public void CreateSvg_OnOutputScaleBar()
        {
            tree.Style.ShowScaleBar = true;

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

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
        public void CreateSvg_Maximum()
        {
            tree.Style.ShowLeafLabels = true;
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

            SvgDocument svg = SvgExporter.CreateSvg(tree, exportOptions);

            Assert.Multiple(() =>
            {
                Assert.Equal(392, svg.Width, 0.1);
                Assert.Equal(348, svg.Height, 0.1);

                SvgElement branchesGroup = svg.GetElementById("branches");
                Assert.NotNull(branchesGroup);
                Assert.IsType<SvgGroup>(branchesGroup);
                Assert.Equal(21, branchesGroup.Children.Count);
                Assert.True(branchesGroup.Children.All(x => x is SvgLine));

                SvgElement? leavesGroup = svg.GetElementById("leaves");
                Assert.NotNull(leavesGroup);
                Assert.IsType<SvgGroup>(leavesGroup);
                Assert.Equal(7, leavesGroup.Children.Count);
                Assert.True(leavesGroup.Children.All(x => x is SvgText));

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

        #endregion Static Methods

        #region Instance Methods

        [Fact]
        public void Export_WithNullTree()
        {
            using var stream = new MemoryStream();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(null!, stream, new ExportOptions()));
        }

        [Fact]
        public void Export_WithNullStream()
        {
            Assert.Throws<ArgumentNullException>(() => exporter.Export(tree, null!, new ExportOptions()));
        }

        [Fact]
        public void Export_WithNullOptions()
        {
            using var stream = new MemoryStream();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(tree, stream, null!));
        }

        [Fact]
        public void Export_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                exporter.Export(tree, stream, exportOptions);
            }

            var fileInfo = new FileInfo(outputPath);

            Assert.Multiple(() =>
            {
                Assert.True(fileInfo.Length > 0);
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("Core", "Export", "test.svg"), outputPath);
            });
        }

        [Fact]
        public async Task ExportAsync_WithNullTree()
        {
            using var stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentNullException>(() => exporter.ExportAsync(null!, stream, new ExportOptions()));
        }

        [Fact]
        public async Task ExportAsync_WithNullStream()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => exporter.ExportAsync(tree, null!, new ExportOptions()));
        }

        [Fact]
        public async Task ExportAsync_WithNullOptions()
        {
            using var stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentNullException>(() => exporter.ExportAsync(tree, stream, null!));
        }

        [Fact]
        public async Task ExportAsync_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                await exporter.ExportAsync(tree, stream, exportOptions);
            }

            var fileInfo = new FileInfo(outputPath);

            Assert.Multiple(() =>
            {
                Assert.True(fileInfo.Length > 0);
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("Core", "Export", "test.svg"), outputPath);
            });
        }

        #endregion Instance Methods
    }
}
