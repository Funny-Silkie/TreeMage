using System.Buffers;
using TreeViewer.Core.Styles;
using Xunit.Sdk;

namespace TreeViewer.Core.Assertions
{
    /// <summary>
    /// 自作のアサーションを記述します。
    /// </summary>
    internal static class CustomizedAssertions
    {
        /// <summary>
        /// 二つのテキストファイルの等価性を検証します。
        /// </summary>
        /// <param name="expected">予期されるテキストファイルのパス</param>
        /// <param name="actual">実際のテキストファイルのパス</param>
        /// <exception cref="EqualException">同じ番号の行間で違いがみられる</exception>
        public static void EqualTextFiles(string expected, string actual)
        {
            if (string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase)) return;

            using var expectedReader = new StreamReader(expected);
            using var actualReader = new StreamReader(actual);
            int lineNo = 0;

            while (true)
            {
                lineNo++;
                string? expectedLine = expectedReader.ReadLine();
                string? actualLine = actualReader.ReadLine();

                if (expectedLine is null)
                {
                    try
                    {
                        Assert.Null(actualLine);
                    }
                    catch (NullException)
                    {
                        throw EqualException.ForMismatchedValuesWithError(null, actualLine, banner: $"Redundant line occurred at Line {lineNo}");
                    }
                    break;
                }

                try
                {
                    Assert.NotNull(actualLine);
                }
                catch (NotNullException)
                {
                    throw EqualException.ForMismatchedValuesWithError(expectedLine, null, banner: $"Missing line occurred at Line {lineNo}");
                }

                try
                {
                    Assert.Equal(expectedLine, actualLine);
                }
                catch (EqualException)
                {
                    throw EqualException.ForMismatchedValuesWithError(expectedLine, actualLine, banner: $"Value differ at Line {lineNo}");
                }
            }
        }

        /// <summary>
        /// 二つのバイナリファイルの等価性を検証します。
        /// </summary>
        /// <param name="expected">予期されるバイナリファイルのパス</param>
        /// <param name="actual">実際のバイナリファイルのパス</param>
        /// <exception cref="EqualException">ファイルに違いがみられる</exception>
        public static void EqualBinaryFiles(string expected, string actual)
        {
            if (string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase)) return;

            var expectedFileInfo = new FileInfo(expected);
            var actualFileInfo = new FileInfo(actual);

            try
            {
                Assert.Equal(expectedFileInfo.Length, actualFileInfo.Length);
            }
            catch (EqualException)
            {
                throw EqualException.ForMismatchedValuesWithError(expectedFileInfo.Length, actualFileInfo.Length, banner: "File sizes differ");
            }

            using var expectedStream = new FileStream(expected, FileMode.Open, FileAccess.Read);
            using var actualStream = new FileStream(actual, FileMode.Open, FileAccess.Read);

            const int bufferSize = 4096;

            byte[] expectedBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                Span<byte> expectedBufferSpan = expectedBuffer.AsSpan(0, bufferSize);

                byte[] actualBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    Span<byte> actualBufferSpan = actualBuffer.AsSpan(0, bufferSize);

                    while (true)
                    {
                        int expectedBytesToRead = expectedStream.Read(expectedBufferSpan);
                        int actualBytesToRead = actualStream.Read(actualBufferSpan);

                        Assert.Equal(expectedBytesToRead, actualBytesToRead);

                        if (expectedBytesToRead == 0) break;
                        Assert.Equal(expectedBufferSpan, actualBufferSpan);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(actualBuffer);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(expectedBuffer);
            }
        }

        /// <summary>
        /// <see cref="TreeStyle"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(TreeStyle expected, TreeStyle actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.XScale, actual.XScale);
                Assert.Equal(expected.YScale, actual.YScale);
                Assert.Equal(expected.BranchThickness, actual.BranchThickness);
                Assert.Equal(expected.ShowLeafLabels, actual.ShowLeafLabels);
                Assert.Equal(expected.LeafLabelsFontSize, actual.LeafLabelsFontSize);
                Assert.Equal(expected.ShowNodeValues, actual.ShowNodeValues);
                Assert.Equal(expected.NodeValueType, actual.NodeValueType);
                Assert.Equal(expected.NodeValueFontSize, actual.NodeValueFontSize);
                Assert.Equal(expected.ShowBranchValues, actual.ShowBranchValues);
                Assert.Equal(expected.BranchValueType, actual.BranchValueType);
                Assert.Equal(expected.BranchValueFontSize, actual.BranchValueFontSize);
                Assert.Equal(expected.ShowBranchDecorations, actual.ShowBranchDecorations);
                Assert.Equal(expected.DecorationStyles, actual.DecorationStyles);
                Assert.Equal(expected.ShowScaleBar, actual.ShowScaleBar);
                Assert.Equal(expected.ScaleBarValue, actual.ScaleBarValue);
                Assert.Equal(expected.ScaleBarFontSize, actual.ScaleBarFontSize);
                Assert.Equal(expected.ScaleBarThickness, actual.ScaleBarThickness);
            });
        }

        /// <summary>
        /// <see cref="CladeStyle"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(CladeStyle expected, CladeStyle actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.BranchColor, actual.BranchColor);
                Assert.Equal(expected.LeafColor, actual.LeafColor);
            });
        }
    }
}
