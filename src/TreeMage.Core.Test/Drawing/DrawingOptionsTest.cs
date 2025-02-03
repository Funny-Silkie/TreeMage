namespace TreeMage.Core.Drawing
{
    public class DrawingOptionsTest
    {
        #region Ctors

        [Fact]
        public void Ctor()
        {
            var options = new DrawingOptions();

            Assert.Equal(BranchColoringType.Both, options.BranchColoring);
        }

        #endregion Ctors
    }
}
