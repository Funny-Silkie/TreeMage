using System.Runtime.CompilerServices;
using TreeViewer.Core.Trees;

namespace TreeViewer
{
    public class CladeIdManagerTest
    {
        private Clade clade;

        public CladeIdManagerTest()
        {
            clade = new Clade();
        }

        #region Static Methods

        [Fact]
        public void FromId_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => CladeIdManager.FromId(null!));
        }

        [Fact]
        public void FromId_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => CladeIdManager.FromId(string.Empty));
        }

        [Fact]
        public void FromId_AsPositive_WithInteger()
        {
            IntPtr id = Unsafe.As<Clade, IntPtr>(ref clade);
            Clade recovered = CladeIdManager.FromId(id.ToString());

            Assert.Same(clade, recovered);
        }

        [Fact]
        public void FromId_AsPositive_WithSuffix()
        {
            IntPtr id = Unsafe.As<Clade, IntPtr>(ref clade);
            Clade recovered = CladeIdManager.FromId($"{id}-hoge-fuga");

            Assert.Same(clade, recovered);
        }

        [Fact]
        public void GetId_WithNullClade()
        {
            Assert.Throws<ArgumentNullException>(() => CladeIdManager.GetId(null!));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetId_AsPositive_WithNullOrEmptySuffix(string? suffix)
        {
            string id = clade.GetId(suffix);

            Assert.Equal(Unsafe.As<Clade, IntPtr>(ref clade).ToString(), id);
        }

        [Fact]
        public void GetId_AsPositive_WithNotEmptySuffix()
        {
            string id = clade.GetId("branch");

            Assert.Equal($"{Unsafe.As<Clade, IntPtr>(ref clade)}-branch", id);
        }

        #endregion Static Methods
    }
}
