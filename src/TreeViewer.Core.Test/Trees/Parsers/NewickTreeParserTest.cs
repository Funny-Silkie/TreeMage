namespace TreeViewer.Core.Trees.Parsers
{
    public class NewickTreeParserTest
    {
        private readonly NewickTreeParser parser;

        public NewickTreeParserTest()
        {
            parser = new NewickTreeParser();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? excepiton = Record.Exception(() => new NewickTreeParser());

            Assert.Null(excepiton);
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void TargetFormat_Get()
        {
            Assert.Equal(TreeFormat.Newick, parser.TargetFormat);
        }

        #endregion Properties

        #region Methods

        [Fact]
        public async Task ReadAsync_WithNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => parser.ReadAsync(null!));
        }

        [Fact]
        public async Task ReadAsync_WithInvalidFormat()
        {
            using var reader = new StringReader("(A");
            await Assert.ThrowsAsync<TreeFormatException>(() => parser.ReadAsync(reader));
        }

        [Fact]
        public async Task ReadAsync_AsPositive()
        {
            using var reader = new StringReader("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);");
            Tree[] trees = await parser.ReadAsync(reader);

            Assert.Multiple(() =>
            {
                Assert.Single(trees);
                CladeTest.CompareClades(TreeTest.CreateDummyTree().Root, trees[0].Root);
            });
        }

        [Fact]
        public async Task WriteAsync_WithNullWriter()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => parser.WriteAsync(null!, []));
        }

        [Fact]
        public async Task WriteAsync_WithNullTrees()
        {
            using var writer = new StringWriter();
            await Assert.ThrowsAsync<ArgumentNullException>(() => parser.WriteAsync(writer, null!));
        }

        [Fact]
        public async Task WriteAsync_WithTreesContainingNullTree()
        {
            using var writer = new StringWriter();
            await Assert.ThrowsAsync<ArgumentException>(() => parser.WriteAsync(writer, [null!, TreeTest.CreateDummyTree()]));
        }

        [Fact]
        public async Task WriteAsync_AsPositive()
        {
            using var writer = new StringWriter();
            await parser.WriteAsync(writer, TreeTest.CreateDummyTree());

            Assert.Equal("(A:2,((BAA:5,BAB:3)20/30:1,((BBAA:2,BBAB:1)85/95:1,BBB:3)100/100:2)30/45:2,C:1);", writer.ToString());
        }

        #endregion Methods
    }
}
