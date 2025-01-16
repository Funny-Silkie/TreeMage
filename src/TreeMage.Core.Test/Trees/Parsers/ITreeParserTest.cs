namespace TreeMage.Core.Trees.Parsers
{
    public class ITreeParserTest()
    {
        #region Methods

        [Fact]
        public void CreateFromTargetFormat_WithInvalidValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ITreeParser.CreateFromTargetFormat((TreeFormat)(-1)));
        }

        [Fact]
        public void CreateFromTargetFormat_AsPositive()
        {
            Assert.IsType<NewickTreeParser>(ITreeParser.CreateFromTargetFormat(TreeFormat.Newick));
        }

        #endregion Methods
    }
}
