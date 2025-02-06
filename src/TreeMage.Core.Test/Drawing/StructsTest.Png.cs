using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace TreeMage.Core.Drawing
{
    public partial class TMPointTest
    {
        #region Instance Methods

        [Fact]
        public void ToPng()
        {
            Assert.Equal(new PointF(10, 20), point.ToPng());
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Implicit_Operator_ToPointF()
        {
            Assert.Equal(new PointF(10, 20), (PointF)point);
        }

        #endregion Operators
    }

    public partial class TMColorTest
    {
        #region Instance Methods

        [Fact]
        public void ToPngColor()
        {
            Assert.Multiple(() => Assert.Equal(Color.Black, known.ToPngColor()),
                            () => Assert.Equal(new Color(new Rgba32(10, 20, 30)), rgb.ToPngColor()),
                            () => Assert.Equal(new Color(new Rgba32(10, 20, 30, 40)), rgba.ToPngColor()));
        }

        [Fact]
        public void ToPngBrush()
        {
            Assert.Equal(new Color(new Rgba32(10, 20, 30)), rgb.ToPngBrush().Color);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        public void ToPngPen(double width)
        {
            var pen = rgb.ToPngPen(width);
            Assert.Multiple(() => Assert.Equal(new SolidBrush(new Color(new Rgba32(10, 20, 30))), pen.StrokeFill),
                            () => Assert.Equal(width, pen.StrokeWidth));
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Explicit_Operator_ToPngColor()
        {
            Assert.Multiple(() => Assert.Equal(Color.Black, (Color)known),
                            () => Assert.Equal(new Color(new Rgba32(10, 20, 30)), (Color)rgb),
                            () => Assert.Equal(new Color(new Rgba32(10, 20, 30, 40)), (Color)rgba));
        }

        #endregion Operators
    }
}
