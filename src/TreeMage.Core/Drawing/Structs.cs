using System.Text.RegularExpressions;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// 座標を表します。
    /// </summary>
    /// <param name="X">X座標を取得します。</param>
    /// <param name="Y">Y座標を取得します。</param>
    public readonly partial record struct TMPoint(double X, double Y);

    /// <summary>
    /// サイズを表します。
    /// </summary>
    /// <param name="Width">横幅を取得します。</param>
    /// <param name="Height">高さを取得します。</param>
    public readonly partial record struct TMSize(double Width, double Height);

    /// <summary>
    /// 矩形を表します。
    /// </summary>
    /// <param name="X">X座標を取得します。</param>
    /// <param name="Y">Y座標を取得します。</param>
    /// <param name="Width">横幅を取得します。</param>
    /// <param name="Height">高さを取得します。</param>
    public readonly partial record struct TMRect(double X, double Y, double Width, double Height)
    {
        /// <summary>
        /// 基準点の座標を取得します。
        /// </summary>
        public TMPoint Point => new TMPoint(X, Y);

        /// <summary>
        /// 矩形のサイズを取得します。
        /// </summary>
        public TMSize Size => new TMSize(Width, Height);

        /// <summary>
        /// <see cref="TMRect"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="point">基準点の座標</param>
        /// <param name="size">サイズ</param>
        public TMRect(TMPoint point, TMSize size)
            : this(point.X, point.Y, size.Width, size.Height)
        {
        }
    }

    /// <summary>
    /// 色を表します。
    /// </summary>
    /// <param name="Value">色を表す文字列を取得します。</param>
    public readonly partial record struct TMColor(string Value)
    {
        /// <summary>
        /// RGBカラーを取得します。
        /// </summary>
        /// <param name="color">色を表す文字列</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        /// <returns><paramref name="color"/>からRGBカラーを取得できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        private static bool TryGetRgb(string color, out byte r, out byte g, out byte b)
        {
            Match rgbMatch = GetRgbRegex().Match(color);
            if (rgbMatch.Success)
            {
                if (byte.TryParse(rgbMatch.Groups[1].ValueSpan, out r) &&
                    byte.TryParse(rgbMatch.Groups[2].ValueSpan, out g) &&
                    byte.TryParse(rgbMatch.Groups[3].ValueSpan, out b)) return true;
            }

            r = g = b = 0;
            return false;
        }

        /// <summary>
        /// RGBAカラーを取得します。
        /// </summary>
        /// <param name="color">色を表す文字列</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        /// <param name="a">A</param>
        /// <returns><paramref name="color"/>からRGBAカラーを取得できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        private static bool TryGetRgba(string color, out byte r, out byte g, out byte b, out byte a)
        {
            Match rgbMatch = GetRgbaRegex().Match(color);
            if (rgbMatch.Success)
            {
                if (byte.TryParse(rgbMatch.Groups[1].ValueSpan, out r) &&
                    byte.TryParse(rgbMatch.Groups[2].ValueSpan, out g) &&
                    byte.TryParse(rgbMatch.Groups[3].ValueSpan, out b) &&
                    byte.TryParse(rgbMatch.Groups[4].ValueSpan, out a)) return true;
            }

            r = g = b = a = 0;
            return false;
        }

        [GeneratedRegex(@"rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)")]
        private static partial Regex GetRgbRegex();

        [GeneratedRegex(@"rgba\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)")]
        private static partial Regex GetRgbaRegex();
    }
}
