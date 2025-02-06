using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace TreeMage.Core.Drawing
{
    public partial record struct TMPoint
    {
        /// <summary>
        /// <see cref="PointF"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public PointF ToPng() => new PointF((float)X, (float)Y);

        /// <summary>
        /// <see cref="PointF"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator PointF(TMPoint value) => value.ToPng();
    }

    public partial record struct TMColor
    {
        /// <summary>
        /// <see cref="Color"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public Color ToPngColor()
        {
            if (TryGetRgb(Value, out byte r, out byte g, out byte b)) return new Rgba32(r, g, b, 255);
            if (TryGetRgba(Value, out r, out g, out b, out byte a)) return new Rgba32(r, g, b, a);
            if (Color.TryParse(Value, out Color result)) return result;
            return Color.Black;
        }

        /// <summary>
        /// <see cref="SolidBrush"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public SolidBrush ToPngBrush() => new SolidBrush(ToPngColor());

        /// <summary>
        /// <see cref="SolidPen"/>へ変換します。
        /// </summary>
        /// <param name="width">太さ</param>
        /// <returns>変換後の値</returns>
        public SolidPen ToPngPen(double width) => new SolidPen(ToPngColor(), (float)width);

        /// <summary>
        /// <see cref="Color"/>へ明示的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static explicit operator Color(TMColor value) => value.ToPngColor();
    }
}
