using PdfSharpCore.Pdf;
using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Exporting
{
    /// <summary>
    /// PDFへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class PdfExporter : IExporter
    {
        /// <inheritdoc/>
        public ExportType Type => ExportType.Pdf;

        /// <summary>
        /// <see cref="PdfExporter"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PdfExporter()
        {
        }

        /// <inheritdoc/>
        public void Export(Tree tree, Stream destination, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(options);

            var drawer = new PdfDrawer();
            ((ITreeDrawer)drawer).Draw(tree, options);
            using PdfDocument document = drawer.Document;
            document.Save(destination);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            Export(tree, destination, options);

            await Task.CompletedTask;
        }
    }
}
