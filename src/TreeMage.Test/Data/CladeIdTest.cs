using System.Runtime.CompilerServices;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities.Assertions;

namespace TreeMage.Data
{
    public class CladeIdTest
    {
        private readonly Clade clade;
        private readonly CladeId id;

        public CladeIdTest()
        {
            clade = new Clade();
            id = new CladeId(clade, CladeIdSuffix.Branch);
        }

        #region Static Methods

        [Fact]
        public void Parse_WithString_WithNullText()
        {
            Assert.Throws<ArgumentNullException>(() => CladeId.Parse(null!));
        }

        [Fact]
        public void Parse_WithString_WithEmptyText()
        {
            Assert.Throws<ArgumentException>(() => CladeId.Parse(string.Empty));
        }

        [Fact]
        public void Parse_WithString_WithInvalidText()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => CladeId.Parse("0"));
                Assert.Throws<FormatException>(() => CladeId.Parse("abc"));
                Assert.Throws<FormatException>(() => CladeId.Parse("-1-branch"));
                Assert.Throws<FormatException>(() => CladeId.Parse("abc-branch"));
                Assert.Throws<ArgumentException>(() => CladeId.Parse("0-branch"));
                Assert.Throws<FormatException>(() => CladeId.Parse("123-none"));
            });
        }

        [Fact]
        public void Parse_WithString_AsPositive()
        {
            Clade clade = this.clade;
            IntPtr ptr = Unsafe.As<Clade, IntPtr>(ref clade);

            Assert.Multiple(() =>
            {
                Assert.Equal(new CladeId(clade, CladeIdSuffix.None), CladeId.Parse(ptr.ToString()));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Branch), CladeId.Parse($"{ptr}-branch"));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Node), CladeId.Parse($"{ptr}-node"));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Leaf), CladeId.Parse($"{ptr}-leaf"));
            });
        }

        [Fact]
        public void Parse_WithReadOnlySpan_WithEmptyText()
        {
            Assert.Throws<ArgumentException>(() => CladeId.Parse([]));
        }

        [Fact]
        public void Parse_WithReadOnlySpan_WithInvalidText()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => CladeId.Parse("0".AsSpan()));
                Assert.Throws<FormatException>(() => CladeId.Parse("abc".AsSpan()));
                Assert.Throws<FormatException>(() => CladeId.Parse("-1-branch".AsSpan()));
                Assert.Throws<FormatException>(() => CladeId.Parse("abc-branch".AsSpan()));
                Assert.Throws<ArgumentException>(() => CladeId.Parse("0-branch".AsSpan()));
                Assert.Throws<FormatException>(() => CladeId.Parse("123-none".AsSpan()));
            });
        }

        [Fact]
        public void Parse_WithReadOnlySpan_AsPositive()
        {
            Clade clade = this.clade;
            IntPtr ptr = Unsafe.As<Clade, IntPtr>(ref clade);

            Assert.Multiple(() =>
            {
                Assert.Equal(new CladeId(clade, CladeIdSuffix.None), CladeId.Parse(ptr.ToString().AsSpan()));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Branch), CladeId.Parse($"{ptr}-branch".AsSpan()));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Node), CladeId.Parse($"{ptr}-node".AsSpan()));
                Assert.Equal(new CladeId(clade, CladeIdSuffix.Leaf), CladeId.Parse($"{ptr}-leaf".AsSpan()));
            });
        }

        [Fact]
        public void TryParse_WithString_WithNullText()
        {
            CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, null);
        }

        [Fact]
        public void TryParse_WithString_WithEmptyText()
        {
            CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, string.Empty);
        }

        [Fact]
        public void TryParse_WithString_WithInvalidText()
        {
            Assert.Multiple(() =>
            {
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "0");
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "abc");
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "-1-branch");
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "abc-branch");
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "0-branch");
                CustomizedAssertions.FailToTry<string?, CladeId>(CladeId.TryParse, "123-none");
            });
        }

        [Fact]
        public void TryParse_WithString_AsPositive()
        {
            Clade clade = this.clade;
            IntPtr ptr = Unsafe.As<Clade, IntPtr>(ref clade);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.SucceedToTry<string?, CladeId>(CladeId.TryParse, ptr.ToString(), new CladeId(clade, CladeIdSuffix.None));
                CustomizedAssertions.SucceedToTry<string?, CladeId>(CladeId.TryParse, $"{ptr}-branch", new CladeId(clade, CladeIdSuffix.Branch));
                CustomizedAssertions.SucceedToTry<string?, CladeId>(CladeId.TryParse, $"{ptr}-node", new CladeId(clade, CladeIdSuffix.Node));
                CustomizedAssertions.SucceedToTry<string?, CladeId>(CladeId.TryParse, $"{ptr}-leaf", new CladeId(clade, CladeIdSuffix.Leaf));
            });
        }

        [Fact]
        public void TryParse_WithReadOnlySpan_WithEmptyText()
        {
            CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, []);
        }

        [Fact]
        public void TryParse_WithReadOnlySpan_WithInvalidText()
        {
            Assert.Multiple(() =>
            {
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "0");
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "abc");
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "-1-branch");
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "abc-branch");
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "0-branch");
                CustomizedAssertions.FailToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, "123-none");
            });
        }

        [Fact]
        public void TryParse_WithReadOnlySpan_AsPositive()
        {
            Clade clade = this.clade;
            IntPtr ptr = Unsafe.As<Clade, IntPtr>(ref clade);

            Assert.Multiple(() =>
            {
                CustomizedAssertions.SucceedToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, ptr.ToString(), new CladeId(clade, CladeIdSuffix.None));
                CustomizedAssertions.SucceedToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, $"{ptr}-branch", new CladeId(clade, CladeIdSuffix.Branch));
                CustomizedAssertions.SucceedToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, $"{ptr}-node", new CladeId(clade, CladeIdSuffix.Node));
                CustomizedAssertions.SucceedToTry<ReadOnlySpan<char>, CladeId>(CladeId.TryParse, $"{ptr}-leaf", new CladeId(clade, CladeIdSuffix.Leaf));
            });
        }

        #endregion Static Methods

        #region Ctors

        [Fact]
        public void Ctor_WithClade_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CladeId(null!));
        }

        [Fact]
        public void Ctor_WithClade_AsPositive()
        {
            var id = new CladeId(clade);

            Assert.Multiple(() =>
            {
                Assert.Same(clade, id.Clade);
                Assert.Equal(CladeIdSuffix.None, id.Suffix);
            });
        }

        [Fact]
        public void Ctor_WithCladeAndSuffix_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CladeId(null!, CladeIdSuffix.None));
        }

        [Theory]
        [InlineData(CladeIdSuffix.None)]
        [InlineData(CladeIdSuffix.Branch)]
        [InlineData(CladeIdSuffix.Node)]
        [InlineData(CladeIdSuffix.Leaf)]
        public void Ctor_WithCladeAndSuffix_AsPositive(CladeIdSuffix suffix)
        {
            var id = new CladeId(clade, suffix);

            Assert.Multiple(() =>
            {
                Assert.Same(clade, id.Clade);
                Assert.Equal(suffix, id.Suffix);
            });
        }

        #endregion Ctors

        #region Instance Methods

        [Fact]
        public void Equal_WithCladeId()
        {
            Assert.Multiple(() =>
            {
                Assert.True(id.Equals(id));
                Assert.True(id.Equals(new CladeId(clade, CladeIdSuffix.Branch)));
                Assert.False(id.Equals(new CladeId(new Clade(), CladeIdSuffix.Branch)));
                Assert.False(id.Equals(new CladeId(clade, CladeIdSuffix.None)));
                Assert.False(id.Equals(new CladeId(clade, CladeIdSuffix.Leaf)));
                Assert.False(id.Equals(new CladeId(clade, CladeIdSuffix.Node)));
                Assert.False(id.Equals(new CladeId()));
            });
        }

        [Fact]
        public void Equal_WithCladeObject()
        {
            Assert.Multiple(() =>
            {
                Assert.True(id.Equals(obj: id));
                Assert.True(id.Equals(obj: new CladeId(clade, CladeIdSuffix.Branch)));
                Assert.False(id.Equals(obj: new CladeId(new Clade(), CladeIdSuffix.Branch)));
                Assert.False(id.Equals(obj: new CladeId(clade, CladeIdSuffix.None)));
                Assert.False(id.Equals(obj: new CladeId(clade, CladeIdSuffix.Leaf)));
                Assert.False(id.Equals(obj: new CladeId(clade, CladeIdSuffix.Node)));
                Assert.False(id.Equals(obj: new CladeId()));
                Assert.False(id.Equals(obj: null));
                Assert.False(id.Equals(obj: clade));
            });
        }

        [Fact]
        public void GetHashCode_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(id.GetHashCode(), new CladeId(clade, CladeIdSuffix.Branch).GetHashCode());
                Assert.NotEqual(id.GetHashCode(), new CladeId(new Clade(), CladeIdSuffix.Branch).GetHashCode());
                Assert.NotEqual(id.GetHashCode(), new CladeId(clade, CladeIdSuffix.None).GetHashCode());
                Assert.NotEqual(id.GetHashCode(), new CladeId(clade, CladeIdSuffix.Node).GetHashCode());
                Assert.NotEqual(id.GetHashCode(), new CladeId(clade, CladeIdSuffix.Leaf).GetHashCode());
                Assert.NotEqual(id.GetHashCode(), new CladeId().GetHashCode());
            });
        }

        [Fact]
        public void ToString_OnWithoutSuffix()
        {
            Clade clade = this.clade;

            Assert.Equal(Unsafe.As<Clade, IntPtr>(ref clade).ToString(), new CladeId(clade).ToString());
        }

        [Theory]
        [InlineData(CladeIdSuffix.Branch)]
        [InlineData(CladeIdSuffix.Node)]
        [InlineData(CladeIdSuffix.Leaf)]
        public void ToString_OnWithSuffix(CladeIdSuffix suffix)
        {
            Clade clade = this.clade;
            string prefix = Unsafe.As<Clade, IntPtr>(ref clade).ToString();

            Assert.Equal($"{prefix}-{suffix.ToString().ToLower()}", new CladeId(clade, suffix).ToString());
        }

        #endregion Instance Methods

        #region Operators

#pragma warning disable CS1718 // 同じ変数と比較されました

        [Fact]
        public void Operator_Equality()
        {
            Assert.Multiple(() =>
            {
                Assert.True(id == id);
                Assert.True(id == new CladeId(clade, CladeIdSuffix.Branch));
                Assert.False(id == new CladeId(new Clade(), CladeIdSuffix.Branch));
                Assert.False(id == new CladeId(clade, CladeIdSuffix.None));
                Assert.False(id == new CladeId(clade, CladeIdSuffix.Leaf));
                Assert.False(id == new CladeId(clade, CladeIdSuffix.Node));
                Assert.False(id == new CladeId());
            });
        }

        [Fact]
        public void Operator_Inequality()
        {
            Assert.Multiple(() =>
            {
                Assert.False(id != id);
                Assert.False(id != new CladeId(clade, CladeIdSuffix.Branch));
                Assert.True(id != new CladeId(new Clade(), CladeIdSuffix.Branch));
                Assert.True(id != new CladeId(clade, CladeIdSuffix.None));
                Assert.True(id != new CladeId(clade, CladeIdSuffix.Leaf));
                Assert.True(id != new CladeId(clade, CladeIdSuffix.Node));
                Assert.True(id != new CladeId());
            });
        }

#pragma warning restore CS1718 // 同じ変数と比較されました

        #endregion Operators
    }
}
