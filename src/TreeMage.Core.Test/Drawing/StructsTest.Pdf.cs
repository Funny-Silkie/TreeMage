using PdfSharpCore.Drawing;
using TreeMage.TestUtilities.Assertions;

namespace TreeMage.Core.Drawing
{
    public partial class TMPointTest
    {
        #region Instance Methods

        [Fact]
        public void ToPdf()
        {
            Assert.Equal(new XPoint(10, 20), point.ToPdf());
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Implicit_Operator_ToXPoint()
        {
            Assert.Equal(new XPoint(10, 20), (XPoint)point);
        }

        #endregion Operators
    }

    public partial class TMSizeTest
    {
        #region Instance Methods

        [Fact]
        public void ToPdf()
        {
            Assert.Equal(new XSize(10, 20), size.ToPdf());
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Implicit_Operator_ToXSize()
        {
            Assert.Equal(new XSize(10, 20), (XSize)size);
        }

        #endregion Operators
    }

    public partial class TMRectTest
    {
        #region Instance Methods

        [Fact]
        public void ToPdf()
        {
            Assert.Equal(new XRect(10, 20, 30, 40), rect.ToPdf());
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Implicit_Operator_ToXRect()
        {
            Assert.Equal(new XRect(10, 20, 30, 40), (XRect)rect);
        }

        #endregion Operators
    }

    public partial class TMColorTest
    {
        #region Instance Methods

        [Fact]
        public void ToPdfColor()
        {
            Assert.Multiple(() => CustomizedAssertions.Equal(XColor.FromKnownColor(XKnownColor.Black), known.ToPdfColor()),
                            () => CustomizedAssertions.Equal(XColor.FromArgb(10, 20, 30), rgb.ToPdfColor()),
                            () => CustomizedAssertions.Equal(XColor.FromArgb(40, 10, 20, 30), rgba.ToPdfColor()));
        }

        [Fact]
        public void ToPdfBrush()
        {
            CustomizedAssertions.Equal(XColor.FromArgb(10, 20, 30), rgb.ToPdfBrush().Color);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        public void ToPdfPen(double width)
        {
            var pen = rgb.ToPdfPen(width);
            Assert.Multiple(() => CustomizedAssertions.Equal(XColor.FromArgb(10, 20, 30), pen.Color),
                            () => Assert.Equal(width, pen.Width));
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Explicit_Operator_ToXColor()
        {
            Assert.Multiple(() => CustomizedAssertions.Equal(XColor.FromKnownColor(XKnownColor.Black), (XColor)known),
                            () => CustomizedAssertions.Equal(XColor.FromArgb(10, 20, 30), (XColor)rgb),
                            () => CustomizedAssertions.Equal(XColor.FromArgb(40, 10, 20, 30), (XColor)rgba));
        }

        #endregion Operators
    }
}
