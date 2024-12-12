using System.Buffers;
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
    }
}
