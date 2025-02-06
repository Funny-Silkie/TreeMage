using SkiaSharp;

namespace TreeMage.Core.Drawing
{
    public partial record struct TMRect
    {
        /// <summary>
        /// <see cref="SKRect"/>へ変換します。
        /// </summary>
        /// <returns>変換後の値</returns>
        public SKRect ToSkia() => SKRect.Create((float)X, (float)Y, (float)Width, (float)Height);

        /// <summary>
        /// <see cref="SKRect"/>へ暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator SKRect(TMRect value) => value.ToSkia();
    }
}
