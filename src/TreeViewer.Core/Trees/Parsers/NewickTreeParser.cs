namespace TreeViewer.Core.Trees.Parsers
{
    /// <summary>
    /// NEWICKフォーマットのツリーのパーサーを表します。
    /// </summary>
    public class NewickTreeParser : ITreeParser
    {
        private const char BeginOfCladeToken = '(';
        private const char EndOfCladeToken = ')';
        private const char BeginOfBranchLengthToken = ':';
        private const char CladeSeparatorToken = ',';
        private const char EndOfTreeToken = ';';

        /// <inheritdoc/>
        public TreeFormat TargetFormat => TreeFormat.Newick;

        /// <summary>
        /// <see cref="NewickTreeParser"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NewickTreeParser()
        {
        }

        /// <inheritdoc/>
        public async Task<Tree[]> ReadAsync(TextReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);

            var result = new List<Tree>();

            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan().Trim();
                if (lineSpan.Length == 0) continue;

                foreach (Range currentTreeRange in lineSpan.Split(EndOfTreeToken))
                {
                    ReadOnlySpan<char> treeBlock = lineSpan[currentTreeRange].Trim();
                    if (treeBlock.Length == 0) continue;

                    Clade root = ReadClade(treeBlock);
                    result.Add(new Tree(root));
                }
            }

            return [.. result];
        }

        /// <summary>
        /// クレードを読み込みます。
        /// </summary>
        /// <param name="text">読み込むテキスト</param>
        /// <returns></returns>
        /// <exception cref="TreeFormatException">ツリーのフォーマットが無効</exception>
        private static Clade ReadClade(ReadOnlySpan<char> text)
        {
            if (text.Length == 0) throw new TreeFormatException("空のクレードがあります");

            var result = new Clade();

            int indexOfBegin = text.IndexOf(BeginOfCladeToken);
            int indexOfEnd = text.LastIndexOf(EndOfCladeToken);

            if (indexOfBegin < 0)
            {
                // leaf
                // A:l
                if (indexOfEnd < 0)
                {
                    int indexOfBranchLengthBeginningToken = text.LastIndexOf(BeginOfBranchLengthToken);

                    if (indexOfBranchLengthBeginningToken < 0) result.Taxon = text.ToString();
                    else
                    {
                        result.Taxon = text[..indexOfBranchLengthBeginningToken].TrimEnd().ToString();
                        if (indexOfBranchLengthBeginningToken < text.Length - 1)
                        {
                            ReadOnlySpan<char> branchLengthSpan = text[(indexOfBranchLengthBeginningToken + 1)..].TrimStart();
                            if (branchLengthSpan.Length > 0)
                            {
                                if (!double.TryParse(branchLengthSpan, out double branchLength)) throw new TreeFormatException($"枝長のフォーマット'{branchLengthSpan}'が無効です");
                                result.BranchLength = branchLength;
                            }
                        }
                    }
                    return result;
                }

                throw new TreeFormatException("括弧の数に違いがあります");
            }
            if (indexOfEnd < 0) throw new TreeFormatException("括弧の数に違いがあります");

            // bipartition
            // (A,(B,C),D)s:l

            if (text[0] != BeginOfCladeToken) throw new TreeFormatException($"クレードのフォーマット'{text}'が無効です");
            if (text[^1] != EndOfCladeToken)
            {
                ReadOnlySpan<char> suffix = text[(indexOfEnd + 1)..].TrimStart();

                int indexOfBranchLengthBeginningToken = suffix.LastIndexOf(BeginOfBranchLengthToken);

                if (indexOfBranchLengthBeginningToken < 0) result.Taxon = suffix.ToString();
                else
                {
                    result.Supports = suffix[..indexOfBranchLengthBeginningToken].TrimEnd().ToString();
                    if (indexOfBranchLengthBeginningToken < suffix.Length - 1)
                    {
                        ReadOnlySpan<char> branchLengthSpan = suffix[(indexOfBranchLengthBeginningToken + 1)..].TrimStart();
                        if (branchLengthSpan.Length > 0)
                        {
                            if (!double.TryParse(branchLengthSpan, out double branchLength)) throw new TreeFormatException($"枝長のフォーマット'{branchLengthSpan}'が無効です");
                            result.BranchLength = branchLength;
                        }
                    }
                }
            }

            text = text[(indexOfBegin + 1)..indexOfEnd].Trim();

            // A,(B,C),D

            int indexOfChildBegin = 0;
            int nestLevel = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char currentCharacter = text[i];
                switch (currentCharacter)
                {
                    case BeginOfCladeToken:
                        nestLevel++;
                        continue;

                    case EndOfCladeToken:
                        nestLevel--;
                        continue;

                    case CladeSeparatorToken:
                        if (nestLevel > 0) continue;

                        result.ChildrenInternal.Add(ReadClade(text[indexOfChildBegin..i].Trim()));
                        indexOfChildBegin = i + 1;
                        continue;
                }
            }

            if (indexOfChildBegin < text.Length - 1) result.ChildrenInternal.Add(ReadClade(text[indexOfChildBegin..].TrimStart()));
            return result;
        }

        /// <inheritdoc/>
        public async Task WriteAsync(TextWriter writer, params Tree[] trees)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(trees);

            foreach (Tree currentTree in trees)
            {
                if (currentTree is null) throw new ArgumentException("要素がnullです", nameof(trees));

                await WriteCladeAsync(writer, currentTree.Root);
                await writer.WriteAsync(EndOfTreeToken);
            }
        }

        /// <summary>
        /// クレードを書き込みます。
        /// </summary>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="clade">対象の<see cref="Clade"/>のインスタンス</param>
        private static async Task WriteCladeAsync(TextWriter writer, Clade clade)
        {
            if (clade.IsLeaf) await writer.WriteAsync(clade.Taxon);
            else
            {
                await writer.WriteAsync(BeginOfCladeToken);

                await WriteCladeAsync(writer, clade.ChildrenInternal[0]);
                for (int i = 1; i < clade.ChildrenInternal.Count; i++)
                {
                    await writer.WriteAsync(CladeSeparatorToken);
                    await WriteCladeAsync(writer, clade.ChildrenInternal[i]);
                }

                await writer.WriteAsync(EndOfCladeToken);
            }

            await writer.WriteAsync(clade.Supports);
            if (!double.IsNaN(clade.BranchLength))
            {
                await writer.WriteAsync(BeginOfBranchLengthToken);
                await writer.WriteAsync(clade.BranchLength.ToString());
            }
        }
    }
}
