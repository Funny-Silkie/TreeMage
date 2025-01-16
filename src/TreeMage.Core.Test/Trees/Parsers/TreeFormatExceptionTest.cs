namespace TreeMage.Core.Trees.Parsers
{
    public class TreeFormatExceptionTest
    {
        #region Ctors

        [Fact]
        public void Ctor_WithoutArgs()
        {
            Exception? exception = Record.Exception(() => new TreeFormatException());

            Assert.Null(exception);
        }

        [Fact]
        public void Ctor_WithString()
        {
            var exception = new TreeFormatException("hoge");

            Assert.Equal("hoge", exception.Message);
        }

        [Fact]
        public void Ctor_WithStringAndException()
        {
            var inner = new Exception();
            var exception = new TreeFormatException("hoge", inner);

            Assert.Multiple(() =>
            {
                Assert.Equal("hoge", exception.Message);
                Assert.Equal(inner, exception.InnerException);
            });
        }

        #endregion Ctors
    }
}
