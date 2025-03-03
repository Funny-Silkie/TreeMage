﻿using Svg;
using System.Drawing;
using System.Drawing.Imaging;
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

            var drawer = new SvgDrawer();
            ((ITreeDrawer)drawer).Draw(tree, options.DrawingOptions);
            SvgDocument svg = drawer.Document;

            using Bitmap bitmap = svg.Draw();
            bitmap.Save(destination, ImageFormat.Png);
        }

        /// <inheritdoc/>
        public async Task ExportAsync(Tree tree, Stream destination, ExportOptions options)
        {
            Export(tree, destination, options);

            await Task.CompletedTask;
        }
    }
}
