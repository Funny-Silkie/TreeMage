using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    public class DrawHelpersTest
    {
        #region Static Methods

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
                Assert.Empty(DrawHelpers.SelectShowValue(clade, (CladeValueType)(-1)));
                Assert.Equal(clade.Supports, DrawHelpers.SelectShowValue(clade, CladeValueType.Supports));
                Assert.Equal(clade.BranchLength.ToString(), DrawHelpers.SelectShowValue(clade, CladeValueType.BranchLength));
            });
        }

        #endregion Static Methods
    }
}
