namespace TreeMage
{
    public class ArrayHelpersTest
    {
        #region Static Methods

        [Fact]
        public void Inserted_WithEmpty()
        {
            int[] array = [];

            Assert.Equal([0], ArrayHelpers.Inserted(array, 0, 0));
        }

        [Fact]
        public void Inserted_AsFirst()
        {
            int[] array = [1, 2, 3];

            Assert.Equal([0, 1, 2, 3], ArrayHelpers.Inserted(array, 0, 0));
        }

        [Fact]
        public void Inserted_AsLast()
        {
            int[] array = [1, 2, 3];

            Assert.Equal([1, 2, 3, 4], ArrayHelpers.Inserted(array, 3, 4));
        }

        [Fact]
        public void Inserted_AsMiddle()
        {
            int[] array = [1, 2, 3];

            Assert.Multiple(() =>
            {
                Assert.Equal([1, 10, 2, 3], ArrayHelpers.Inserted(array, 1, 10));
                Assert.Equal([1, 2, 10, 3], ArrayHelpers.Inserted(array, 2, 10));
            });
        }

        #endregion Static Methods
    }
}
