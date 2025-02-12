namespace TreeMage.Core.Drawing
{
    public partial class TMPointTest
    {
        private readonly TMPoint point;

        public TMPointTest()
        {
            point = new TMPoint(10, 20);
        }

        #region Operators

        [Fact]
        public void Operator_UnaryPlus()
        {
            Assert.Equal(point, +point);
        }

        [Fact]
        public void Operator_UnaryNegation()
        {
            Assert.Equal(new TMPoint(-10, -20), -point);
        }

        [Fact]
        public void Operator_Addition()
        {
            Assert.Equal(new TMPoint(40, 50), point + new TMPoint(30, 30));
        }

        [Fact]
        public void Operator_Subtraction()
        {
            Assert.Equal(new TMPoint(5, 15), point - new TMPoint(5, 5));
        }

        [Fact]
        public void Operator_Multiply()
        {
            Assert.Multiple(() => Assert.Equal(new TMPoint(20, 40), point * 2),
                            () => Assert.Equal(new TMPoint(20, 40), 2 * point));
        }

        [Fact]
        public void Operator_Division()
        {
            Assert.Equal(new TMPoint(5, 10), point / 2);
        }

        [Fact]
        public void Implicit_Operator_FromValueTuple()
        {
            Assert.Equal(point, (TMPoint)(10, 20));
        }

        [Fact]
        public void Implicit_Operator_ValueTuple()
        {
            Assert.Equal((10d, 20d), ((double, double))point);
        }

        #endregion Operators
    }

    public partial class TMSizeTest
    {
        private readonly TMSize size;

        public TMSizeTest()
        {
            size = new TMSize(10, 20);
        }

        #region Operators

        [Fact]
        public void Operator_UnaryPlus()
        {
            Assert.Equal(size, +size);
        }

        [Fact]
        public void Operator_UnaryNegation()
        {
            Assert.Equal(new TMSize(-10, -20), -size);
        }

        [Fact]
        public void Operator_Addition()
        {
            Assert.Equal(new TMSize(40, 50), size + new TMSize(30, 30));
        }

        [Fact]
        public void Operator_Subtraction()
        {
            Assert.Equal(new TMSize(5, 15), size - new TMSize(5, 5));
        }

        [Fact]
        public void Operator_Multiply()
        {
            Assert.Multiple(() => Assert.Equal(new TMSize(20, 40), size * 2),
                            () => Assert.Equal(new TMSize(20, 40), 2 * size));
        }

        [Fact]
        public void Operator_Division()
        {
            Assert.Equal(new TMSize(5, 10), size / 2);
        }

        [Fact]
        public void Implicit_Operator_FromValueTuple()
        {
            Assert.Equal(size, (TMSize)(10, 20));
        }

        [Fact]
        public void Implicit_Operator_ValueTuple()
        {
            Assert.Equal((10d, 20d), ((double, double))size);
        }

        #endregion Operators
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

        #region Operators

        [Fact]
        public void Implicit_Operator_FromValueTuple()
        {
            Assert.Equal(rect, (TMRect)(10, 20, 30, 40));
        }

        [Fact]
        public void Implicit_Operator_ValueTuple()
        {
            Assert.Equal((10d, 20d, 30d, 40d), ((double, double, double, double))rect);
        }

        #endregion Operators
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

        #region Ctors

        [Fact]
        public void Ctor_WithRgb()
        {
            Assert.Equal(new TMColor("rgb(10, 20, 30)"), new TMColor(10, 20, 30));
        }

        [Fact]
        public void Ctor_WithRgba()
        {
            Assert.Equal(new TMColor("rgb(10, 20, 30, 40)"), new TMColor(10, 20, 30, 40));
        }

        #endregion Ctors

        #region Static Properties

        [Fact]
        public void Black_Get()
        {
            Assert.Equal("black", TMColor.Black.Value);
        }

        [Fact]
        public void White_Get()
        {
            Assert.Equal("white", TMColor.White.Value);
        }

        #endregion Static Properties
    }
}
