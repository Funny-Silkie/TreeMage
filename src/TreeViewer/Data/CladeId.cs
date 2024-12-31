using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TreeViewer.Core.Trees;

namespace TreeViewer.Data
{
    /// <summary>
    /// クレードのIDを表します。
    /// </summary>
    public readonly struct CladeId : IEquatable<CladeId>, ISpanParsable<CladeId>
    {
        /// <summary>
        /// 対象のクレードを取得します。
        /// </summary>
        public Clade Clade { get; }

        /// <summary>
        /// サフィックスを取得します。
        /// </summary>
        public CladeIdSuffix Suffix { get; }

        /// <summary>
        /// <see cref="CladeId"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public CladeId(Clade clade) : this(clade, CladeIdSuffix.None)
        {
        }

        /// <summary>
        /// <see cref="CladeId"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <param name="suffix">サフィックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        public CladeId(Clade clade, CladeIdSuffix suffix)
        {
            ArgumentNullException.ThrowIfNull(clade);

            Clade = clade;
            Suffix = suffix;
        }

        /// <summary>
        /// <see cref="IntPtr"/>から<see cref="Core.Trees.Clade"/>への変換を行います。
        /// </summary>
        /// <param name="ptr">変換するポインタ</param>
        /// <returns><paramref name="ptr"/>の参照する<see cref="Core.Trees.Clade"/>インスタンス</returns>
        private static Clade? PtrToClade(IntPtr ptr) => Unsafe.As<IntPtr, Clade?>(ref ptr);

        /// <summary>
        /// <see cref="Core.Trees.Clade"/>から<see cref="IntPtr"/>への変換を行います。
        /// </summary>
        /// <param name="clade">変換するオブジェクト</param>
        /// <returns><paramref name="clade"/>を表すポインタ</returns>
        private static IntPtr CladeToPtr(Clade? clade) => Unsafe.As<Clade?, IntPtr>(ref clade);

        #region Equality

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CladeId id && Equals(id);

        /// <inheritdoc/>
        public bool Equals(CladeId other) => ReferenceEquals(Clade, other.Clade) && Suffix == other.Suffix;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(CladeToPtr(Clade), Suffix);

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality"/>
        public static bool operator ==(CladeId left, CladeId right) => left.Equals(right);

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality"/>
        public static bool operator !=(CladeId left, CladeId right) => !(left == right);

        #endregion Equality

        /// <inheritdoc cref="Parse(ReadOnlySpan{char})"/>
        public static CladeId Parse(string s)
        {
            ArgumentNullException.ThrowIfNull(s);

            return Parse(s.AsSpan());
        }

        static CladeId IParsable<CladeId>.Parse(string s, IFormatProvider? provider) => Parse(s);

        /// <summary>
        /// 文字列から<see cref="CladeId"/>へ変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <returns>変換後の値</returns>
        /// <exception cref="ArgumentException"><paramref name="s"/>が空文字または参照するインスタンスが<see langword="null"/></exception>
        /// <exception cref="OverflowException">値がオーバーフローした</exception>
        /// <exception cref="FormatException"><paramref name="s"/>のフォーマットが無効</exception>
        public static CladeId Parse(ReadOnlySpan<char> s)
        {
            if (s.Length == 0) throw new ArgumentException("空文字です", nameof(s));

            Clade? clade;
            if (IntPtr.TryParse(s, out IntPtr ptr))
            {
                clade = PtrToClade(ptr);
                if (clade is null) throw new ArgumentException("参照するインスタンスがnullです");

                return new CladeId(clade);
            }

            int hyphenIndex = s.IndexOf('-');
            if (hyphenIndex <= 0) throw new FormatException();

            clade = PtrToClade(IntPtr.Parse(s[..hyphenIndex]));
            if (clade is null) throw new ArgumentException("参照するインスタンスがnullです");

            CladeIdSuffix suffix = Enum.Parse<CladeIdSuffix>(s[(hyphenIndex + 1)..], true);
            if (suffix == CladeIdSuffix.None) throw new FormatException();

            return new CladeId(clade, suffix);
        }

        static CladeId ISpanParsable<CladeId>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);

        /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, out CladeId)"/>
        public static bool TryParse([NotNullWhen(true)] string? s, out CladeId id) => TryParse(s.AsSpan(), out id);

        static bool IParsable<CladeId>.TryParse(string? s, IFormatProvider? provider, out CladeId result) => TryParse(s, out result);

        /// <summary>
        /// 文字列から<see cref="CladeId"/>へ変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="id">変換後の値，変換に失敗したら既定値</param>
        /// <returns><paramref name="id"/>へ変換できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> s, out CladeId id)
        {
            Clade? clade;
            if (IntPtr.TryParse(s, out IntPtr ptr))
            {
                clade = PtrToClade(ptr);
                if (clade is null)
                {
                    id = default;
                    return false;
                }

                id = new CladeId(clade);
                return true;
            }

            int hyphenIndex = s.IndexOf('-');
            if (hyphenIndex <= 0)
            {
                id = default;
                return false;
            }

            if (!IntPtr.TryParse(s[..hyphenIndex], out ptr))
            {
                id = default;
                return false;
            }
            clade = PtrToClade(ptr);

            if (clade is null)
            {
                id = default;
                return false;
            }

            if (!Enum.TryParse(s[(hyphenIndex + 1)..], true, out CladeIdSuffix suffix) || suffix == CladeIdSuffix.None)
            {
                id = default;
                return false;
            }

            id = new CladeId(clade, suffix);
            return true;
        }

        static bool ISpanParsable<CladeId>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CladeId result) => TryParse(s, out result);

        /// <summary>
        /// ID文字列を取得します。
        /// </summary>
        /// <returns>ID文字列</returns>
        public override string ToString()
        {
            IntPtr ptr = CladeToPtr(Clade);

            return Suffix switch
            {
                CladeIdSuffix.None => ptr.ToString(),
                CladeIdSuffix.Branch => $"{ptr}-branch",
                CladeIdSuffix.Node => $"{ptr}-node",
                CladeIdSuffix.Leaf => $"{ptr}-leaf",
                _ => throw new InvalidOperationException(),
            };
        }
    }

    /// <summary>
    /// クレードのIDの拡張を表します。
    /// </summary>
    internal static class CladeIdExtensions
    {
        /// <summary>
        /// IDを取得します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <param name="suffix">サフィックス</param>
        /// <returns>ID</returns>
        /// <exception cref="ArgumentNullException"><paramref name="clade"/>が<see langword="null"/></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CladeId GetId(this Clade clade, CladeIdSuffix suffix) => new CladeId(clade, suffix);
    }
}
