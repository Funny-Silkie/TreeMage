namespace TreeViewer.Core.Exporting
{
    public class ExportOptionsTest
    {
        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new ExportOptions());

            Assert.Null(exception);
        }

        #endregion Ctors
    }
}
