using TreeMage.TestUtilities.Assertions;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities;
using System.Runtime.InteropServices;

namespace TreeMage.Core.Exporting
{
    public class PngExporterTest
    {
        private const string outputPath = "test.png";

        private readonly Tree tree;
        private readonly PngExporter exporter;
        private readonly ExportOptions exportOptions;

        public PngExporterTest()
        {
            tree = DummyData.CreateTree();
            tree.Style.XScale = 30;
            tree.Style.ScaleBarValue = 1;
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.CladeLabel = "hoge";
            tree.Root.ChildrenInternal[1].ChildrenInternal[1].Style.ShadeColor = "lightblue";
            exporter = new PngExporter();
            exportOptions = new ExportOptions();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new PngExporter());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Type_Get()
        {
            Assert.Equal(ExportType.Png, exporter.Type);
        }

        #endregion Properties

        #region Methods

#if WINDOWS

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
                CustomizedAssertions.EqualBinaryFiles(CreateTestDataPath("Core", "Export", "test.png"), outputPath);
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
                CustomizedAssertions.EqualBinaryFiles(CreateTestDataPath("Core", "Export", "test.png"), outputPath);
            });
        }

#endif

        #endregion Methods
    }
}
