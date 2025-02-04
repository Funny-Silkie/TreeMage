namespace TreeMage.Core.Drawing
{
    public partial class TMPointTest
    {
        private readonly TMPoint point;

        public TMPointTest()
        {
            point = new TMPoint(10, 20);
        }
    }

    public partial class TMSizeTest
    {
        private readonly TMSize size;

        public TMSizeTest()
        {
            size = new TMSize(10, 20);
        }
    }

    public partial class TMRectTest
    {
        private readonly TMRect rect;

        public TMRectTest()
        {
            rect = new TMRect(10, 20, 30, 40);
        }

        #region Ctors

        [Fact]
        public void Ctor_WithPointAndSize()
        {
            Assert.Equal(rect, new TMRect(new TMPoint(10, 20), new TMSize(30, 40)));
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void Point_Get()
        {
            Assert.Equal(new TMPoint(10, 20), rect.Point);
        }

        [Fact]
        public void Size_Get()
        {
            Assert.Equal(new TMSize(30, 40), rect.Size);
        }

        #endregion Properties
    }

    public partial class TMColorTest
    {
        private readonly TMColor known;
        private readonly TMColor rgb;
        private readonly TMColor rgba;

        public TMColorTest()
        {
            known = new TMColor("black");
            rgb = new TMColor("rgb(10, 20, 30)");
            rgba = new TMColor("rgba(10, 20, 30, 40)");
        }
    }
}
