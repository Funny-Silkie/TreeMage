using System.Drawing;

namespace TreeMage.Core.Drawing
{
    public partial class TMColorTest
    {
        #region Instance Methods

        [Fact]
        public void ToSvgColor()
        {
            Assert.Multiple(() => Assert.Equal(Color.Black, known.ToSvgColor()),
                            () => Assert.Equal(Color.FromArgb(10, 20, 30), rgb.ToSvgColor()),
                            () => Assert.Equal(Color.FromArgb(40, 10, 20, 30), rgba.ToSvgColor()));
        }

        [Fact]
        public void ToSvgColorServer()
        {
            Assert.Equal(Color.FromArgb(10, 20, 30), rgb.ToSvgColorServer().Colour);
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Explicit_Operator_ToSvgColor()
        {
            Assert.Multiple(() => Assert.Equal(Color.Black, (Color)known),
                            () => Assert.Equal(Color.FromArgb(10, 20, 30), (Color)rgb),
                            () => Assert.Equal(Color.FromArgb(40, 10, 20, 30), (Color)rgba));
        }

        #endregion Operators
    }
}
