using TreeViewer.Core.Drawing;

namespace TreeViewer.Core.Exporting
{
    public class ExportOptionsTest
    {
        #region Ctors

        [Fact]
        public void Ctor()
        {
            var options = new ExportOptions();

            Assert.Equal(BranchColoringType.Both, options.BranchColoring);
        }

        #endregion Ctors
    }
}
