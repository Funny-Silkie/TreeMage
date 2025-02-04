using PdfSharpCore.Drawing;

namespace TreeMage.Core.Drawing
{
    public partial record struct TMPoint
    {
        /// <summary>
        /// <see cref="XPoint"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public XPoint ToPdf() => new XPoint(X, Y);

        /// <summary>
        /// <see cref="XPoint"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator XPoint(TMPoint value) => value.ToPdf();

        /// <summary>
        /// <see cref="ValueTuple{T1, T2}"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator TMPoint((double, double) value) => new TMPoint(value.Item1, value.Item2);

        /// <summary>
        /// <see cref="ValueTuple{T1, T2}"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator (double x, double y)(TMPoint value) => (value.X, value.Y);
    }

    public partial record struct TMSize
    {
        /// <summary>
        /// <see cref="XSize"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public XSize ToPdf() => new XSize(Width, Height);

        /// <summary>
        /// <see cref="XSize"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator XSize(TMSize value) => value.ToPdf();

        /// <summary>
        /// <see cref="ValueTuple{T1, T2}"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator TMSize((double, double) value) => new TMSize(value.Item1, value.Item2);

        /// <summary>
        /// <see cref="ValueTuple{T1, T2}"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator (double width, double height)(TMSize value) => (value.Width, value.Height);
    }

    public partial record struct TMRect
    {
        /// <summary>
        /// <see cref="XRect"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public XRect ToPdf() => new XRect(X, Y, Width, Height);

        /// <summary>
        /// <see cref="XRect"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator XRect(TMRect value) => value.ToPdf();

        /// <summary>
        /// <see cref="ValueTuple{T1, T2, T3, T4}"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator TMRect((double, double, double, double) value) => new TMRect(value.Item1, value.Item2, value.Item3, value.Item4);

        /// <summary>
        /// <see cref="ValueTuple{T1, T2, T3, T4}"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator (double x, double y, double width, double height)(TMRect value) => (value.X, value.Y, value.Width, value.Height);
    }

    public partial record struct TMColor
    {
        /// <summary>
        /// <see cref="XColor"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public XColor ToPdfColor()
        {
            if (TryGetRgb(Value, out byte r, out byte g, out byte b)) return XColor.FromArgb(r, g, b);
            if (TryGetRgba(Value, out r, out g, out b, out byte a)) return XColor.FromArgb(a, r, g, b);

            if (Enum.TryParse(Value, true, out XKnownColor knownColor)) return XColor.FromKnownColor(knownColor);
            return XColor.FromArgb(0, 0, 0);
        }

        /// <summary>
        /// <see cref="XSolidBrush"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public XSolidBrush ToPdfBrush() => new XSolidBrush(ToPdfColor());

        /// <summary>
        /// <see cref="XPen"/>へ変換します。
        /// </summary>
        /// <param name="width">太さ</param>
        /// <returns>変換後の値</returns>
        public XPen ToPdfPen(double width) => new XPen(ToPdfColor(), width);

        /// <summary>
        /// <see cref="XColor"/>へ明示的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static explicit operator XColor(TMColor value) => value.ToPdfColor();
    }
}
