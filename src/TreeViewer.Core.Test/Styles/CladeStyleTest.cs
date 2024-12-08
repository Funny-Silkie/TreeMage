namespace TreeViewer.Core.Styles
{
    public class CladeStyleTest
    {
        private readonly CladeStyle style;

        public CladeStyleTest()
        {
            style = new CladeStyle();
        }

        /// <summary>
        /// スタイル同士の等価性の比較を行います。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        internal static void CompareStyles(CladeStyle expected, CladeStyle actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.BranchColor, actual.BranchColor);
                Assert.Equal(expected.LeafColor, actual.LeafColor);
            });
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new CladeStyle();

            Assert.Multiple(() =>
            {
                Assert.Equal("black", style.BranchColor);
                Assert.Equal("black", style.LeafColor);
            });
        }

        #endregion Ctors

        #region Methods

        [Fact]
        public void ApplyValues_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => style.ApplyValues(null!));
        }

        [Fact]
        public void ApplyValues_AsPositive()
        {
            style.ApplyValues(new CladeStyle()
            {
                BranchColor = "white",
                LeafColor = "none",
            });

            Assert.Multiple(() =>
            {
                Assert.Equal("white", style.BranchColor);
                Assert.Equal("none", style.LeafColor);
            });
        }

        #endregion Methods
    }
}
