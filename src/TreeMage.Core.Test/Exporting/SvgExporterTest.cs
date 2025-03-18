using TreeMage.TestUtilities.Assertions;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities;
using System.Runtime.InteropServices;

namespace TreeMage.Core.Exporting
{
    public partial class SvgExporterTest
    {
        private const string outputPath = "test.svg";

        private readonly Tree tree;
        private readonly SvgExporter exporter;
        private readonly ExportOptions exportOptions;

        public SvgExporterTest()
        {
            tree = DummyData.CreateTree();
            tree.Style.XScale = 30;
            tree.Style.ScaleBarValue = 1;
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.CladeLabel = "hoge";
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.ShadeColor = "lightblue";
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
#if WINDOWS
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("Core", "Export", "test.svg"), outputPath);
#endif
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
#if WINDOWS
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("Core", "Export", "test.svg"), outputPath);
#endif
            });
        }

        #endregion Instance Methods
    }
}
