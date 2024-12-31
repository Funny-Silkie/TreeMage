using TreeViewer.Core.Trees;

namespace TreeViewer.Data
{
    public class CladeIdExtensionsTest
    {
        #region Static Methods

        [Fact]
        public void GetId_WithNullClade()
        {
            Assert.Throws<ArgumentNullException>(() => CladeIdExtensions.GetId(null!, CladeIdSuffix.None));
        }

        [Theory]
        [InlineData(CladeIdSuffix.None)]
        [InlineData(CladeIdSuffix.Branch)]
        [InlineData(CladeIdSuffix.Node)]
        [InlineData(CladeIdSuffix.Leaf)]
        public void GetId_AsPositive(CladeIdSuffix suffix)
        {
            var clade = new Clade();
            CladeId id = clade.GetId(suffix);

            Assert.Multiple(() =>
            {
                Assert.Equal(suffix, id.Suffix);
                Assert.Same(clade, id.Clade);
            });
        }

        #endregion Static Methods
    }
}
