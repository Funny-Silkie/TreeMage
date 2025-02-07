using PdfSharpCore.Drawing;
using SixLabors.Fonts;

namespace TreeMage.Core.Drawing
{
    public class FontManagerTest
    {
        #region Static Methods

        [Theory]
        [InlineData(12)]
        [InlineData(30)]
        [InlineData(72)]
        public void GetImageSharpFont(int size)
        {
            Font font = FontManager.GetImageSharpFont(size);
            Assert.Equal(size, font.Size);

            Assert.Same(font, FontManager.GetImageSharpFont(size));
        }

        [Theory]
        [InlineData(12)]
        [InlineData(30)]
        [InlineData(72)]
        public void GetPdfFont(int size)
        {
            XFont font = FontManager.GetPdfFont(size);
            Assert.Equal(size, font.Size);

            Assert.Same(font, FontManager.GetPdfFont(size));
        }

        #endregion Static Methods
    }
}
