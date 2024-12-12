namespace TreeViewer.Core.Exporting
{
    public class IExporterTest
    {
        #region Static Methods

        [Fact]
        public void Create_WithInvalidType()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => IExporter.Create((ExportType)(-1)));
        }

        [Fact]
        public void Create_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.IsType<SvgExporter>(IExporter.Create(ExportType.Svg));
                Assert.IsType<PngExporter>(IExporter.Create(ExportType.Png));
                Assert.IsType<PdfExporter>(IExporter.Create(ExportType.Pdf));
            });
        }

        #endregion Static Methods
    }
}
