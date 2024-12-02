namespace TreeViewer.Core.Trees
{
    public class TreeTest
    {
        #region Ctors

        [Fact]
        public void Ctor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Tree(null!));
        }

        [Fact(Skip = "Not implemented")]
        public void Ctor_WithNonRootClade()
        {
        }

        [Fact(Skip = "Not implemented")]
        public void Ctor_AsPositive()
        {
        }

        #endregion Ctors
    }
}
