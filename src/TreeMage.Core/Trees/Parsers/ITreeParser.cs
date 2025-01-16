namespace TreeMage.Core.Trees.Parsers
{
    /// <summary>
    /// 系統樹のパーサーを表します。
    /// </summary>
    public interface ITreeParser
    {
        /// <summary>
        /// 対象のフォーマットを取得します。
        /// </summary>
        TreeFormat TargetFormat { get; }

        /// <summary>
        /// フォーマットに対応したインスタンスを取得します。
        /// </summary>
        /// <param name="targetFormat">ツリーのフォーマット</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetFormat"/>が無効</exception>
        static ITreeParser CreateFromTargetFormat(TreeFormat targetFormat)
        {
            return targetFormat switch
            {
                TreeFormat.Newick => new NewickTreeParser(),
                _ => throw new ArgumentOutOfRangeException(nameof(targetFormat)),
            };
        }

        /// <inheritdoc cref="ReadAsync(TextReader)"/>
        Tree[] Read(TextReader reader) => ReadAsync(reader).Result;

        /// <summary>
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="reader">使用する<see cref="TextReader"/>のインスタンス</param>
        /// <returns>読み込まれた系統樹一覧</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>が<see langword="null"/></exception>
        /// <exception cref="TreeFormatException">系統樹のフォーマットが無効</exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="reader"/>が既に破棄されている</exception>
        Task<Tree[]> ReadAsync(TextReader reader);

        /// <inheritdoc cref="WriteAsync(TextWriter, Tree[])"/>
        void Write(TextWriter writer, params Tree[] trees) => WriteAsync(writer, trees).Wait();

        /// <summary>
        /// 系統樹を書き込みます。
        /// </summary>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="trees"></param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="trees"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="trees"/>の要素が<see langword="null"/></exception>
        /// <exception cref="IOException">I/Oエラーが発生</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        Task WriteAsync(TextWriter writer, params Tree[] trees);
    }
}
