using System.Buffers;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Settings;
using Xunit.Sdk;

namespace TreeViewer.TestUtilities.Assertions
{
    /// <summary>
    /// 自作のアサーションを記述します。
    /// </summary>
    public static class CustomizedAssertions
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
                Assert.Equal(expected.ShowCladeLabels, actual.ShowCladeLabels);
                Assert.Equal(expected.CladeLabelsFontSize, actual.CladeLabelsFontSize);
                Assert.Equal(expected.CladeLabelLineThickness, actual.CladeLabelLineThickness);
                Assert.Equal(expected.ShowNodeValues, actual.ShowNodeValues);
                Assert.Equal(expected.NodeValueType, actual.NodeValueType);
                Assert.Equal(expected.NodeValueFontSize, actual.NodeValueFontSize);
                Assert.Equal(expected.ShowBranchValues, actual.ShowBranchValues);
                Assert.Equal(expected.BranchValueType, actual.BranchValueType);
                Assert.Equal(expected.BranchValueFontSize, actual.BranchValueFontSize);
                Assert.Equal(expected.BranchValueHideRegex?.ToString(), actual.BranchValueHideRegex?.ToString());
                Assert.Equal(expected.BranchValueHideRegexPattern, actual.BranchValueHideRegexPattern);
                Assert.Equal(expected.ShowBranchDecorations, actual.ShowBranchDecorations);
                Assert.Equal(expected.DecorationStyles.Length, actual.DecorationStyles.Length);
                Assert.Equal(expected.ShowScaleBar, actual.ShowScaleBar);
                Assert.Equal(expected.ScaleBarValue, actual.ScaleBarValue);
                Assert.Equal(expected.ScaleBarFontSize, actual.ScaleBarFontSize);
                Assert.Equal(expected.ScaleBarThickness, actual.ScaleBarThickness);
                Assert.Equal(expected.CollapseType, actual.CollapseType);
                Assert.Equal(expected.CollapsedConstantWidth, actual.CollapsedConstantWidth);
            });
            for (int i = 0; i < expected.DecorationStyles.Length; i++) Equal(expected.DecorationStyles[i], actual.DecorationStyles[i]);
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
                Assert.Equal(expected.Collapsed, actual.Collapsed);
                Assert.Equal(expected.CladeLabel, actual.CladeLabel);
            });
        }

        /// <summary>
        /// <see cref="BranchDecorationStyle"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(BranchDecorationStyle expected, BranchDecorationStyle actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.Enabled, actual.Enabled);
                Assert.Equal(expected.Regex?.ToString(), actual.Regex?.ToString());
                Assert.Equal(expected.RegexPattern, actual.RegexPattern);
                Assert.Equal(expected.ShapeSize, actual.ShapeSize);
                Assert.Equal(expected.DecorationType, actual.DecorationType);
                Assert.Equal(expected.ShapeColor, actual.ShapeColor);
            });
        }

        /// <summary>
        /// <see cref="Clade"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <remarks>親要素の比較は行いません</remarks>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(Clade expected, Clade actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.Taxon, actual.Taxon);
                Assert.Equal(expected.Supports, actual.Supports);
                Assert.Equal(expected.BranchLength, actual.BranchLength);
                Assert.Equal(expected.ChildrenInternal.Count, actual.ChildrenInternal.Count);
                Equal(expected.Style, actual.Style);
            });

            for (int i = 0; i < expected.ChildrenInternal.Count; i++) Equal(expected.ChildrenInternal[i], actual.ChildrenInternal[i]);
        }

        /// <summary>
        /// <see cref="Tree"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <remarks>親要素の比較は行いません</remarks>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(Tree expected, Tree actual)
        {
            Equal(expected.Root, actual.Root);
            try
            {
                Equal(expected.Style, actual.Style);
            }
            catch (EqualException)
            {
                throw EqualException.ForMismatchedValuesWithError(expected.Style, actual.Style, banner: "Tree style differs");
            }
        }

        /// <summary>
        /// <see cref="Configurations"/>同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <remarks>親要素の比較は行いません</remarks>
        /// <exception cref="EqualException"><paramref name="expected"/>と<paramref name="actual"/>間に等価性が認められない</exception>
        public static void Equal(Configurations expected, Configurations actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.BranchColoring, actual.BranchColoring);
                Assert.Equal(expected.AutoOrderingMode, actual.AutoOrderingMode);
            });
        }

        /// <summary>
        /// 変換メソッドの失敗を検証します。
        /// </summary>
        /// <typeparam name="TArg">引数の型</typeparam>
        /// <typeparam name="TResult">変換結果</typeparam>
        /// <param name="operation">処理関数</param>
        /// <param name="arg">引数</param>
        public static void FailToTry<TArg, TResult>(TryOperation<TArg, TResult> operation, TArg arg)
            where TArg : allows ref struct
        {
            Assert.False(operation.Invoke(arg, out TResult actual));
            if (typeof(TResult).IsClass) Assert.Null(actual);
            else Assert.Equal(default, actual);
        }

        /// <summary>
        /// 変換メソッドの失敗を検証します。
        /// </summary>
        /// <typeparam name="TArg1">引数の型1</typeparam>
        /// <typeparam name="TArg2">引数の型2</typeparam>
        /// <typeparam name="TResult">変換結果</typeparam>
        /// <param name="operation">処理関数</param>
        /// <param name="arg1">引数1</param>
        /// <param name="arg2">引数2</param>
        public static void FailToTry<TArg1, TArg2, TResult>(TryOperation<TArg1, TArg2, TResult> operation, TArg1 arg1, TArg2 arg2)
            where TArg1 : allows ref struct
            where TArg2 : allows ref struct
        {
            Assert.False(operation.Invoke(arg1, arg2, out TResult result));
            Assert.Equal(default, result);
        }

        /// <summary>
        /// 変換メソッドの成功を検証します。
        /// </summary>
        /// <typeparam name="TArg">引数の型</typeparam>
        /// <typeparam name="TResult">変換結果の型</typeparam>
        /// <param name="operation">処理関数</param>
        /// <param name="arg">引数</param>
        /// <param name="expected">予期される変換結果の値</param>
        public static void SucceedToTry<TArg, TResult>(TryOperation<TArg, TResult> operation, TArg arg, TResult expected)
            where TArg : allows ref struct
        {
            Assert.True(operation.Invoke(arg, out TResult actual));
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// 変換メソッドの成功を検証します。
        /// </summary>
        /// <typeparam name="TArg1">引数の型1</typeparam>
        /// <typeparam name="TArg2">引数の型2</typeparam>
        /// <typeparam name="TResult">変換結果の型</typeparam>
        /// <param name="operation">処理関数</param>
        /// <param name="arg1">引数1</param>
        /// <param name="arg2">引数2</param>
        /// <param name="expected">予期される変換結果の値</param>
        public static void SucceedToTry<TArg1, TArg2, TResult>(TryOperation<TArg1, TArg2, TResult> operation, TArg1 arg1, TArg2 arg2, TResult expected)
            where TArg1 : allows ref struct
            where TArg2 : allows ref struct
        {
            Assert.True(operation.Invoke(arg1, arg2, out TResult actual));
            Assert.Equal(expected, actual);
        }
    }
}
