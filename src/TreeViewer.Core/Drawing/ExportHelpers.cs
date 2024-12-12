using PdfSharpCore.Drawing;
using Svg;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing
{
    /// <summary>
    /// 描画のヘルパークラスを表します。
    /// </summary>
    internal static partial class DrawHelpers
    {
        /// <summary>
        /// RGBカラーを取得します。
        /// </summary>
        /// <param name="color">色を表す文字列</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        /// <returns><paramref name="color"/>からRGBカラーを取得できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryGetRgb(string color, out byte r, out byte g, out byte b)
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
        public static bool TryGetRgba(string color, out byte r, out byte g, out byte b, out byte a)
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

        /// <summary>
        /// 色を表す文字列から<see cref="SvgColourServer"/>を取得します。
        /// </summary>
        /// <param name="color">色</param>
        /// <returns><paramref name="color"/>に対応した<see cref="SvgColourServer"/>の新しいインスタンス</returns>
        public static SvgColourServer CreateSvgColor(string color)
        {
            if (TryGetRgb(color, out byte r, out byte g, out byte b)) return new SvgColourServer(Color.FromArgb(r, g, b));
            if (TryGetRgba(color, out r, out g, out b, out byte a)) return new SvgColourServer(Color.FromArgb(a, r, g, b));

            return new SvgColourServer(ColorTranslator.FromHtml(color));
        }

        /// <summary>
        /// 色を表す文字列から<see cref="XColor"/>を取得します。
        /// </summary>
        /// <param name="value">色を表す文字列</param>
        /// <returns><paramref name="color"/>に対応した<see cref="XColor"/></returns>
        public static XColor CreatePdfColor(string value)
        {
            if (TryGetRgb(value, out byte r, out byte g, out byte b)) return XColor.FromArgb(r, g, b);
            if (TryGetRgba(value, out r, out g, out b, out byte a)) return XColor.FromArgb(a, r, g, b);

            if (Enum.TryParse(value, true, out XKnownColor knownColor)) return XColor.FromKnownColor(knownColor);
            return XColor.FromArgb(0, 0, 0);
        }

        /// <summary>
        /// 色から<see cref="XBrush"/>に変換します。
        /// </summary>
        /// <param name="color">色</param>
        /// <returns><paramref name="color"/>に対応した<see cref="XBrush"/>のインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static XSolidBrush ToBrush(this XColor color) => new XSolidBrush(color);

        /// <summary>
        /// 色から<see cref="XPen"/>に変換します。
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="width">線の太さ</param>
        /// <returns><paramref name="color"/>に対応した<see cref="XPen"/>のインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static XPen ToPen(this XColor color, double width) => new XPen(color, width);

        /// <summary>
        /// 表示するクレードの値を選択します。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        /// <param name="valueType">取得する値のタイプ</param>
        /// <returns><paramref name="valueType"/>に応じた<paramref name="clade"/>における値</returns>
        public static string SelectShowValue(Clade clade, CladeValueType valueType)
        {
            switch (valueType)
            {
                case CladeValueType.Supports:
                    string supports = clade.Supports ?? string.Empty;
                    return supports.Trim();

                case CladeValueType.BranchLength:
                    double branchLength = clade.BranchLength;
                    if (double.IsNaN(branchLength)) return string.Empty;
                    return branchLength.ToString();

                default: return string.Empty;
            }
        }
    }
}
