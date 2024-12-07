using System.Runtime.CompilerServices;
using TreeViewer.Core.Trees;

namespace TreeViewer
{
    /// <summary>
    /// <see cref="Clade"/>とDOMのidの変換を行います。
    /// </summary>
    internal static class CladeIdManager
    {
        // ID format:
        // 1. address
        // 2. address-suffix

        /// <summary>
        /// IDから<see cref="Clade"/>を取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="id"/>が空文字</exception>
        /// <exception cref="FormatException">IDのフォーマットが無効</exception>
        public static Clade FromId(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            if (IntPtr.TryParse(id, out IntPtr ptr)) return PtrToClade(ptr);
            int hyphenIndex = id.IndexOf('-');
            if (hyphenIndex < 0) throw new FormatException();

            ptr = IntPtr.Parse(id.AsSpan()[..hyphenIndex]);
            return PtrToClade(ptr);
        }

        /// <summary>
        /// <see cref="IntPtr"/>から<see cref="Clade"/>への変換を行います。
        /// </summary>
        /// <param name="ptr">変換するポインタ</param>
        /// <returns><paramref name="ptr"/>の参照する<see cref="Clade"/>インスタンス</returns>
        private static Clade PtrToClade(IntPtr ptr) => Unsafe.As<IntPtr, Clade>(ref ptr);

        /// <summary>
        /// IDを取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <param name="suffix">サフィックス</param>
        /// <returns>ID</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public static string GetId(this Clade clade, string? suffix = null)
        {
            ArgumentNullException.ThrowIfNull(clade);

            string result = CladeToPtr(clade).ToString();
            if (!string.IsNullOrEmpty(suffix)) result += '-' + suffix;
            return result;
        }

        /// <summary>
        /// <see cref="Clade"/>から<see cref="IntPtr"/>への変換を行います。
        /// </summary>
        /// <param name="clade">変換するオブジェクト</param>
        /// <returns><paramref name="clade"/>を表すポインタ</returns>
        private static IntPtr CladeToPtr(Clade clade) => Unsafe.As<Clade, IntPtr>(ref clade);
    }
}
