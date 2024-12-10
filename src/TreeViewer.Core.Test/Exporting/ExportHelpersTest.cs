using PdfSharpCore.Drawing;
using System.Drawing;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Exporting
{
    public class ExportHelpersTest
    {
        #region Static Methods

        [Fact]
        public void TryGetRgb_AsSuccess()
        {
            bool success = ExportHelpers.TryGetRgb("rgb(1, 11, 111)", out byte r, out byte g, out byte b);

            Assert.Multiple(() =>
            {
                Assert.True(success);
                Assert.Equal(1, r);
                Assert.Equal(11, g);
                Assert.Equal(111, b);
            });
        }

        [Fact]
        public void TryGetRgb_AsFailure()
        {
            bool success = ExportHelpers.TryGetRgb("white", out byte r, out byte g, out byte b);

            Assert.Multiple(() =>
            {
                Assert.False(success);
                Assert.Equal(0, r);
                Assert.Equal(0, g);
                Assert.Equal(0, b);
            });
        }

        [Fact]
        public void TryGetRgba_AsSuccess()
        {
            bool success = ExportHelpers.TryGetRgba("rgba(1, 11, 111, 01)", out byte r, out byte g, out byte b, out byte a);

            Assert.Multiple(() =>
            {
                Assert.True(success);
                Assert.Equal(1, r);
                Assert.Equal(11, g);
                Assert.Equal(111, b);
                Assert.Equal(1, a);
            });
        }

        [Fact]
        public void TryGetRgba_AsFailure()
        {
            bool success = ExportHelpers.TryGetRgba("white", out byte r, out byte g, out byte b, out byte a);

            Assert.Multiple(() =>
            {
                Assert.False(success);
                Assert.Equal(0, r);
                Assert.Equal(0, g);
                Assert.Equal(0, b);
                Assert.Equal(0, a);
            });
        }

        [Fact]
        public void CreateSvgColor()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(Color.Black, ExportHelpers.CreateSvgColor("black").Colour);
                Assert.Equal(Color.FromArgb(10, 20, 30), ExportHelpers.CreateSvgColor("rgb(10, 20, 30)").Colour);
                Assert.Equal(Color.FromArgb(40, 10, 20, 30), ExportHelpers.CreateSvgColor("rgba(10, 20, 30, 40)").Colour);
            });
        }

        [Fact]
        public void CreatePdfColor()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(XColor.FromArgb(10, 20, 30), ExportHelpers.CreatePdfColor("rgb(10, 20, 30)"));
                Assert.Equal(XColor.FromArgb(40, 10, 20, 30), ExportHelpers.CreatePdfColor("rgba(10, 20, 30, 40)"));
                Assert.Equal(XColor.FromKnownColor(XKnownColor.Red), ExportHelpers.CreatePdfColor("red"));
                Assert.Equal(XColor.FromArgb(0, 0, 0), ExportHelpers.CreatePdfColor("!!!"));
            });
        }

        [Fact]
        public void ToBrush()
        {
            XColor color = XColor.FromArgb(100, 150, 255);

            Assert.Equal(color, color.ToBrush().Color);
        }

        [Fact]
        public void ToPen()
        {
            XColor color = XColor.FromArgb(100, 150, 255);
            XPen pen = color.ToPen(3);

            Assert.Multiple(() =>
            {
                Assert.Equal(color, pen.Color);
                Assert.Equal(3, pen.Width);
            });
        }

        [Fact]
        public void SelectShowValue()
        {
            var clade = new Clade()
            {
                Supports = "100",
                BranchLength = 0.1,
            };

            Assert.Multiple(() =>
            {
                Assert.Empty(ExportHelpers.SelectShowValue(clade, (CladeValueType)(-1)));
                Assert.Equal(clade.Supports, ExportHelpers.SelectShowValue(clade, CladeValueType.Supports));
                Assert.Equal(clade.BranchLength.ToString(), ExportHelpers.SelectShowValue(clade, CladeValueType.BranchLength));
            });
        }

        #endregion Static Methods
    }
}
