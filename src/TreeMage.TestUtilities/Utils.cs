using System.Buffers;

namespace TreeMage.TestUtilities
{
    /// <summary>
    /// テストにおける共通処理を記述します。
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// テスト用データの含まれているディレクトリ
        /// </summary>
        public const string TestDataDirectory = "./TestData/";

        /// <summary>
        /// テストデータのパスを取得します。
        /// </summary>
        /// <param name="subpathes">テストデータの<see cref="TestDataDirectory"/>からの相対パス</param>
        /// <returns><paramref name="subpathes"/>に対応したテストデータのパス</returns>
        public static string CreateTestDataPath(params ReadOnlySpan<string> subpathes)
        {
            if (subpathes.Length == 0) return TestDataDirectory;
            if (subpathes.Length == 1) return Path.Join(TestDataDirectory, subpathes[0]);

            string[] array = ArrayPool<string>.Shared.Rent(subpathes.Length + 1);

            try
            {
                Span<string> span = array.AsSpan(0, subpathes.Length + 1);
                span[0] = TestDataDirectory;
                subpathes.CopyTo(span[1..]);

                return Path.Combine(span);
            }
            finally
            {
                ArrayPool<string>.Shared.Return(array);
            }
        }
    }
}
