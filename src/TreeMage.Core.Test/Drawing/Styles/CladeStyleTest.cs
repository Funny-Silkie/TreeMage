using TreeMage.TestUtilities.Assertions;

namespace TreeMage.Core.Drawing.Styles
{
    public class CladeStyleTest
    {
        private readonly CladeStyle style;

        public CladeStyleTest()
        {
            style = new CladeStyle();
        }

        /// <summary>
        /// ダミーの値を設定します。
        /// </summary>
        /// <param name="style">対象の<see cref="CladeStyle"/>のインスタンス</param>
        private static void ApplyDummyValues(CladeStyle style)
        {
            style.BranchColor = "red";
            style.LeafColor = "blue";
            style.Collapsed = true;
            style.CladeLabel = "clade";
            style.ShadeColor = "blue";
            style.YScale = 2;
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
                Assert.False(style.Collapsed);
                Assert.Null(style.CladeLabel);
                Assert.Null(style.ShadeColor);
                Assert.Equal(1, style.YScale);
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
            var applied = new CladeStyle();
            ApplyDummyValues(applied);

            style.ApplyValues(applied);

            CustomizedAssertions.Equal(applied, style);
        }

        [Fact]
        public void Clone()
        {
            ApplyDummyValues(style);

            CladeStyle cloned = style.Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            ApplyDummyValues(style);

            var cloned = (CladeStyle)((ICloneable)style).Clone();

            CustomizedAssertions.Equal(style, cloned);
        }

        #endregion Methods
    }
}
