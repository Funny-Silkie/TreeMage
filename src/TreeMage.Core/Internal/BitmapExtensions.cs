using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TreeMage.Core.Drawing;

namespace TreeMage.Core.Internal
{
    /// <summary>
    /// ビットマップイメージの拡張を表します。
    /// </summary>
    internal static class BitmapImageExtensions
    {
        /// <summary>
        /// 矩形を描画します。
        /// </summary>
        /// <param name="image">対象の画像</param>
        /// <param name="brush">ブラシ</param>
        /// <param name="area">描画範囲</param>
        public static void DrawClosedRectangle(this Image image, Brush brush, TMRect area)
        {
            float xStart = (float)area.X;
            float yStart = (float)area.Y;
            float xEnd = (float)(area.X + area.Width);
            float yEnd = (float)(area.Y + area.Height);

            image.Mutate(context => context.FillPolygon(brush,
                                                        new PointF(xStart, yStart),
                                                        new PointF(xStart, yEnd),
                                                        new PointF(xEnd, yEnd),
                                                        new PointF(xEnd, yStart)));
        }

        /// <summary>
        /// 矩形を描画します。
        /// </summary>
        /// <param name="image">対象の画像</param>
        /// <param name="pen">線のペン</param>
        /// <param name="area">描画範囲</param>
        public static void DrawOpenRectangle(this Image image, Pen pen, TMRect area)
        {
            float xStart = (float)area.X;
            float yStart = (float)area.Y;
            float xEnd = (float)(area.X + area.Width);
            float yEnd = (float)(area.Y + area.Height);

            image.Mutate(context => context.DrawPolygon(pen,
                                                        new PointF(xStart, yStart),
                                                        new PointF(xStart, yEnd),
                                                        new PointF(xEnd, yEnd),
                                                        new PointF(xEnd, yStart)));
        }

        public static void DrawLine(this Image image, Pen pen, TMPoint point1, TMPoint point2)
        {
            image.Mutate(context => context.DrawLine(pen, point1, point2));
        }

        public static void DrawPath(this Image image, Pen pen, Brush brush, params IEnumerable<TMPoint> lines)
        {
            PointF[] positions = lines.Select(x => x.ToPng())
                                      .ToArray();
            image.Mutate(context => context.FillPolygon(brush, positions).DrawPolygon(pen, positions));
        }

        public static void DrawClosedCircle(this Image image, Brush brush, TMPoint center, double radius)
        {
            image.Mutate(context => context.Fill(brush, new EllipsePolygon(center, (float)radius)));
        }

        public static void DrawOpenCircle(this Image image, Pen pen, TMPoint center, double radius)
        {
            image.Mutate(context => context.Draw(pen, new EllipsePolygon(center, (float)radius)));
        }

        public static void DrawString(this Image image, string text, Font font, Brush brush, TMPoint point)
        {
            DrawString(image, text, brush, point, new RichTextOptions(font)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
            });
        }

        public static void DrawString(this Image image, string text, Brush brush, TMPoint point, RichTextOptions options)
        {
            options.Origin = point.ToPng();
            image.Mutate(context => context.DrawText(options, text, brush));
        }
    }
}
