using Svg;
using Svg.Pathing;
using Svg.Transforms;

namespace TreeMage.Core.Internal
{
    public class SvgExtensionsTest
    {
        #region Static Methods

        [Fact]
        public void AddTo_WithNullElement()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.AddTo<SvgElement>(null!, new SvgGroup()));
        }

        [Fact]
        public void AddTo_WithNullParent()
        {
            Assert.Throws<ArgumentNullException>(() => new SvgGroup().AddTo(null!));
        }

        [Fact]
        public void AddTo_AsPositive()
        {
            var parent = new SvgGroup();
            var child = new SvgGroup();

            Assert.Same(child, child.AddTo(parent));
            Assert.Single(parent.Children, parent.Children[0]);
        }

        [Fact]
        public void MoveToAbsolutely_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.MoveToAbsolutely(null!, 0, 0));
        }

        [Fact]
        public void MoveToAbsolutely_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.MoveToAbsolutely(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgMoveToSegment>(added);
                Assert.Equal(10, ((SvgMoveToSegment)added).End.X);
                Assert.Equal(20, ((SvgMoveToSegment)added).End.Y);
                Assert.False(((SvgMoveToSegment)added).IsRelative);
            });
        }

        [Fact]
        public void MoveToRelatively_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.MoveToRelatively(null!, 0, 0));
        }

        [Fact]
        public void MoveToRelatively_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.MoveToRelatively(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgMoveToSegment>(added);
                Assert.Equal(10, ((SvgMoveToSegment)added).End.X);
                Assert.Equal(20, ((SvgMoveToSegment)added).End.Y);
                Assert.True(((SvgMoveToSegment)added).IsRelative);
            });
        }

        [Fact]
        public void DrawHorizontalLineRelatively_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.DrawHorizontalLineRelatively(null!, 0));
        }

        [Fact]
        public void DrawHorizontalLineRelatively_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.DrawHorizontalLineRelatively(10));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgLineSegment>(added);
                Assert.Equal(10, ((SvgLineSegment)added).End.X);
                Assert.Equal(float.NaN, ((SvgLineSegment)added).End.Y);
                Assert.True(((SvgLineSegment)added).IsRelative);
            });
        }

        [Fact]
        public void DrawVerticalLineRelatively_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.DrawVerticalLineRelatively(null!, 0));
        }

        [Fact]
        public void DrawVerticalLineRelatively_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.DrawVerticalLineRelatively(10));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgLineSegment>(added);
                Assert.Equal(float.NaN, ((SvgLineSegment)added).End.X);
                Assert.Equal(10, ((SvgLineSegment)added).End.Y);
                Assert.True(((SvgLineSegment)added).IsRelative);
            });
        }

        [Fact]
        public void DrawLineAbsolutely_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.DrawLineAbsolutely(null!, 0, 0));
        }

        [Fact]
        public void DrawLineAbsolutely_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.DrawLineAbsolutely(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgLineSegment>(added);
                Assert.Equal(10, ((SvgLineSegment)added).End.X);
                Assert.Equal(20, ((SvgLineSegment)added).End.Y);
                Assert.False(((SvgLineSegment)added).IsRelative);
            });
        }

        [Fact]
        public void DrawLineRelatively_WithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.DrawLineRelatively(null!, 0, 0));
        }

        [Fact]
        public void DrawLineRelatively_AsPositive()
        {
            var list = new SvgPathSegmentList();

            Assert.Same(list, list.DrawLineRelatively(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(list);

                SvgPathSegment added = list.First();
                Assert.IsType<SvgLineSegment>(added);
                Assert.Equal(10, ((SvgLineSegment)added).End.X);
                Assert.Equal(20, ((SvgLineSegment)added).End.Y);
                Assert.True(((SvgLineSegment)added).IsRelative);
            });
        }

        [Fact]
        public void Translate_WithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.Translate(null!, 0, 0));
        }

        [Fact]
        public void Translate_AsPositive()
        {
            var collection = new SvgTransformCollection();

            Assert.Same(collection, collection.Translate(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(collection);

                SvgTransform added = collection.First();

                Assert.IsType<SvgTranslate>(added);
                Assert.Equal(10, ((SvgTranslate)added).X);
                Assert.Equal(20, ((SvgTranslate)added).Y);
            });
        }

        [Fact]
        public void Rotate_WithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.Rotate(null!, 0));
        }

        [Fact]
        public void Rotate_AsPositive()
        {
            var collection = new SvgTransformCollection();

            Assert.Same(collection, collection.Rotate(10));

            Assert.Multiple(() =>
            {
                Assert.Single(collection);

                SvgTransform added = collection.First();

                Assert.IsType<SvgRotate>(added);
                Assert.Equal(10, ((SvgRotate)added).Angle);
            });
        }

        [Fact]
        public void Scale_WithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => SvgExtensions.Scale(null!, 0, 0));
        }

        [Fact]
        public void Scale_AsPositive()
        {
            var collection = new SvgTransformCollection();

            Assert.Same(collection, collection.Scale(10, 20));

            Assert.Multiple(() =>
            {
                Assert.Single(collection);

                SvgTransform added = collection.First();

                Assert.IsType<SvgScale>(added);
                Assert.Equal(10, ((SvgScale)added).X);
                Assert.Equal(20, ((SvgScale)added).Y);
            });
        }

        #endregion Static Methods
    }
}
