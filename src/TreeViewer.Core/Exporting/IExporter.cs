using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    /// <summary>
    /// データのエクスポートを行うインターフェイスを表します。
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// エクスポート方式を表す値を取得します。
        /// </summary>
        ExportType Type { get; }

        /// <summary>
        /// 指定したエクスポート方式を基に<see cref="IExporter"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="exportType">エクスポート方式</param>
        /// <returns><paramref name="exportType"/>に対応した<see cref="IExporter"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="exportType"/>が無効</exception>
        static IExporter Create(ExportType exportType)
        {
            return exportType switch
            {
                ExportType.Svg => new SvgExporter(),
                ExportType.Png => new PngExporter(),
                ExportType.Pdf => new PdfExporter(),
                _ => throw new ArgumentOutOfRangeException(nameof(exportType)),
            };
        }

        /// <inheritdoc cref="ExportAsync(Tree, Stream, ExportOptions)"/>
        void Export(Tree tree, Stream stream, ExportOptions options) => ExportAsync(tree, stream, options).Wait();

        /// <summary>
        /// データのエクスポートを行います。
        /// </summary>
        /// <param name="tree">エクスポートするツリー</param>
        /// <param name="destination">エクスポート先のストリームオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>または<paramref name="destination"/>，<paramref name="options"/>の何れかが<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="destination"/>が既に破棄されている</exception>
        Task ExportAsync(Tree tree, Stream destination, ExportOptions options);
    }
}
