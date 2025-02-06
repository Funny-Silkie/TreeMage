using SixLabors.ImageSharp;
using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Exporting
{
    /// <summary>
    /// PNGへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class PngExporter : IExporter
    {
        /// <inheritdoc/>
        public ExportType Type => ExportType.Png;

        /// <summary>
        /// <see cref="PngExporter"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PngExporter()
        {
        }

        /// <inheritdoc/>
        public void Export(Tree tree, Stream destination, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(options);

            using var drawer = new PngDrawer();
            ((ITreeDrawer)drawer).Draw(tree, options.DrawingOptions);
            drawer.Image.SaveAsPng(destination);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(options);

            using var drawer = new PngDrawer();
            ((ITreeDrawer)drawer).Draw(tree, options.DrawingOptions);
            await drawer.Image.SaveAsPngAsync(destination);
        }
    }
}
