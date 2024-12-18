namespace TreeViewer
{
    /// <summary>
    /// 配列のヘルパークラスです。
    /// </summary>
    internal static class ArrayHelpers
    {
        /// <summary>
        /// 指定した要素が挿入された配列を生成します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="array">対象の配列</param>
        /// <param name="index">挿入位置</param>
        /// <param name="item">挿入する要素</param>
        /// <returns><paramref name="item"/>が挿入された新しい配列</returns>
        public static T[] Inserted<T>(T[] array, int index, T item)
        {
            if (array.Length == 0) return [item];
            if (index == 0) return array.Prepend(item).ToArray();
            if (index == array.Length) return [.. array, item];

            var result = new T[array.Length + 1];
            Span<T> resultSpan = result.AsSpan();
            array.AsSpan(0, index).CopyTo(resultSpan);
            resultSpan[index] = item;
            array.AsSpan(index).CopyTo(resultSpan[(index + 1)..]);

            return result;
        }
    }
}
