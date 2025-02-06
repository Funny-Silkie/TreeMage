using SkiaSharp;

namespace TreeMage.Core.Drawing
{
    public partial class TMRectTest
    {
        #region Instance Methods

        [Fact]
        public void ToSkia()
        {
            Assert.Equal(SKRect.Create(10, 20, 30, 40), rect.ToSkia());
        }

        #endregion Instance Methods

        #region Operators

        [Fact]
        public void Operator_Implicit_ToSKRect()
        {
            Assert.Equal(SKRect.Create(10, 20, 30, 40), (SKRect)rect);
        }

        #endregion Operators
    }
}
