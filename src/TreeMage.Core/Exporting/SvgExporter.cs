using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Exporting
{
    /// <summary>
    /// SVGへの出力を行う<see cref="IExporter"/>の実装です。
    /// </summary>
    public class SvgExporter : IExporter
    {
        /// <inheritdoc/>
        public ExportType Type => ExportType.Svg;

        /// <summary>
        /// <see cref="SvgExporter"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SvgExporter()
        {
        }

        /// <inheritdoc/>
        public void Export(Tree tree, Stream destination, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(options);

            var drawer = new SvgDrawer();
            ((ITreeDrawer)drawer).Draw(tree, options.DrawingOptions);
            drawer.Document.Write(destination);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            Export(tree, destination, options);

            await Task.CompletedTask;
        }
    }
}
