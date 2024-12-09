namespace TreeViewer.Core.Styles
{
    public class BranchDecorationStyleTest
    {
        private readonly BranchDecorationStyle style;

        public BranchDecorationStyleTest()
        {
            style = new BranchDecorationStyle();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new BranchDecorationStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal("100", style.Regex.ToString());
                Assert.Equal(5, style.ShapeSize);
                Assert.Equal(BranchDecorationType.ClosedCircle, style.DecorationType);
                Assert.Equal("black", style.ShapeColor);
            });
        }

        #endregion Ctors
    }
}
