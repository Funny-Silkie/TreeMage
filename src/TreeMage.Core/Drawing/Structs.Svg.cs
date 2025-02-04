using Svg;
using System.Drawing;

namespace TreeMage.Core.Drawing
{
    public partial record struct TMColor
    {
        /// <summary>
        /// <see cref="Color"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public Color ToSvgColor()
        {
            if (TryGetRgb(Value, out byte r, out byte g, out byte b)) return Color.FromArgb(r, g, b);
            if (TryGetRgba(Value, out r, out g, out b, out byte a)) return Color.FromArgb(a, r, g, b);

            return ColorTranslator.FromHtml(Value);
        }

        /// <summary>
        /// <see cref="SvgColourServer"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public SvgColourServer ToSvgColorServer() => new SvgColourServer(ToSvgColor());

        /// <summary>
        /// <see cref="Color"/>へ明示的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static explicit operator Color(TMColor value) => value.ToSvgColor();
    }
}
