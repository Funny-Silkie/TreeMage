using TreeMage.Core.Drawing;
using TreeMage.TestUtilities.Assertions;

namespace TreeMage.Core.Exporting
{
    public class ExportOptionsTest
    {
        #region Ctors

        [Fact]
        public void Ctor()
        {
            var options = new ExportOptions();

            CustomizedAssertions.Equal(new DrawingOptions(), options.DrawingOptions);
        }

        #endregion Ctors
    }
}
