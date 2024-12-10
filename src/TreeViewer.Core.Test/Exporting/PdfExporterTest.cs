using PdfSharpCore.Drawing;
using System.Text.RegularExpressions;
using TreeViewer.Core.Assertions;
using TreeViewer.Core.Styles;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    public partial class PdfExporterTest
    {
        private const string outputPath = "test.pdf";

        private readonly PdfExporter exporter;
        private readonly ExportOptions exportOptions;

        public PdfExporterTest()
        {
            exporter = new PdfExporter();
            exportOptions = new ExportOptions()
            {
                XScale = 30,
                ScaleBarValue = 1,
            };
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new PdfExporter());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Type_Get()
        {
            Assert.Equal(ExportType.Pdf, exporter.Type);
        }

        #endregion Properties

        #region Methods

        [Fact]
        public void Export_WithNullTree()
        {
            using var stream = new MemoryStream();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(null!, stream, new ExportOptions()));
        }

        [Fact]
        public void Export_WithNullStream()
        {
            Assert.Throws<ArgumentNullException>(() => exporter.Export(TreeTest.CreateDummyTree(), null!, new ExportOptions()));
        }

        [Fact]
        public void Export_WithNullOptions()
        {
            using var stream = new MemoryStream();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(TreeTest.CreateDummyTree(), stream, null!));
        }

        [Fact]
        public void Export_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                exporter.Export(TreeTest.CreateDummyTree(), stream, exportOptions);
            }

            var fileInfo = new FileInfo(outputPath);

            Assert.True(fileInfo.Length > 0);
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => exporter.ExportAsync(TreeTest.CreateDummyTree(), null!, new ExportOptions()));
        }

        [Fact]
        public async Task ExportAsync_WithNullOptions()
        {
            using var stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentNullException>(() => exporter.ExportAsync(TreeTest.CreateDummyTree(), stream, null!));
        }

        [Fact]
        public async Task ExportAsync_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                await exporter.ExportAsync(TreeTest.CreateDummyTree(), stream, exportOptions);
            }

            var fileInfo = new FileInfo(outputPath);

            Assert.True(fileInfo.Length > 0);
        }

        #endregion Methods
    }
}
