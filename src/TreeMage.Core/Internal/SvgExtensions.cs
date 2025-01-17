using Svg;
using Svg.Pathing;
using Svg.Transforms;
using System.Drawing;

namespace TreeMage.Core.Internal
{
    /// <summary>
    /// SVG用の拡張を記述します。
    /// </summary>
    internal static class SvgExtensions
    {
        /// <summary>
        /// 指定した要素の子要素に追加します。
        /// </summary>
        /// <typeparam name="TElement">子として追加する要素の型</typeparam>
        /// <param name="element">子として追加する要素</param>
        /// <param name="parent">親要素</param>
        /// <returns><paramref name="element"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>または<paramref name="parent"/>が<see langword="null"/></exception>
        public static TElement AddTo<TElement>(this TElement element, SvgElement parent)
            where TElement : SvgElement
        {
            ArgumentNullException.ThrowIfNull(element);
            ArgumentNullException.ThrowIfNull(parent);

            parent.Children.Add(element);
            return element;
        }

        #region PathSegment

        /// <summary>
        /// 指定した座標に移動します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>M <paramref name="x"/> <paramref name="y"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList MoveToAbsolutely(this SvgPathSegmentList list, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgMoveToSegment(false, new PointF(x, y)));
            return list;
        }

        /// <summary>
        /// 指定した座標に平行移動します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>m <paramref name="x"/> <paramref name="y"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList MoveToRelatively(this SvgPathSegmentList list, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgMoveToSegment(true, new PointF(x, y)));
            return list;
        }

        /// <summary>
        /// X軸方向に平行移動し，描画します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="width">X軸方面の移動量</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>h <paramref name="width"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList DrawHorizontalLineRelatively(this SvgPathSegmentList list, float width)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgLineSegment(true, new PointF(width, float.NaN)));
            return list;
        }

        /// <summary>
        /// Y軸方向に平行移動し，描画します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="height">Y軸方面の移動量</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>v <paramref name="height"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList DrawVerticalLineRelatively(this SvgPathSegmentList list, float height)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgLineSegment(true, new PointF(float.NaN, height)));
            return list;
        }

        /// <summary>
        /// 平行移動して描画します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="x">X軸座標</param>
        /// <param name="y">Y軸座標</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>l <paramref name="x"/> <paramref name="y"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList DrawLineAbsolutely(this SvgPathSegmentList list, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgLineSegment(false, new PointF(x, y)));
            return list;
        }

        /// <summary>
        /// 平行移動して描画します。
        /// </summary>
        /// <param name="list">使用する<see cref="SvgPathSegmentList"/>のインスタンス</param>
        /// <param name="x">X軸方面の移動量</param>
        /// <param name="y">Y軸方面の移動量</param>
        /// <returns><paramref name="list"/></returns>
        /// <remarks>"<c>l <paramref name="x"/> <paramref name="y"/></c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>が<see langword="null"/></exception>
        public static SvgPathSegmentList DrawLineRelatively(this SvgPathSegmentList list, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new SvgLineSegment(true, new PointF(x, y)));
            return list;
        }

        #endregion PathSegment

        #region Transform

        /// <summary>
        /// 指定した座標への平行移動を追加します。
        /// </summary>
        /// <param name="collection">使用する<see cref="SvgTransformCollection"/>のインスタンス</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns><paramref name="collection"/></returns>
        /// <remarks>"<c>translate(<paramref name="x"/>, <paramref name="y"/>)</c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>が<see langword="null"/></exception>
        public static SvgTransformCollection Translate(this SvgTransformCollection collection, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(collection);
            collection.Add(new SvgTranslate(x, y));
            return collection;
        }

        /// <summary>
        /// 回転を追加します。
        /// </summary>
        /// <param name="collection">使用する<see cref="SvgTransformCollection"/>のインスタンス</param>
        /// <param name="angle">回転角度</param>
        /// <returns><paramref name="collection"/></returns>
        /// <remarks>"<c>rotate(<paramref name="angle"/>)</c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>が<see langword="null"/></exception>
        public static SvgTransformCollection Rotate(this SvgTransformCollection collection, float angle)
        {
            ArgumentNullException.ThrowIfNull(collection);
            collection.Add(new SvgRotate(angle));
            return collection;
        }

        /// <summary>
        /// 拡大を追加します。
        /// </summary>
        /// <param name="collection">使用する<see cref="SvgTransformCollection"/>のインスタンス</param>
        /// <param name="x">X方向の拡大率</param>
        /// <param name="y">Y方向の拡大率</param>
        /// <returns><paramref name="collection"/></returns>
        /// <remarks>"<c>scale(<paramref name="x"/>, <paramref name="y"/>)</c>"と等価</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>が<see langword="null"/></exception>
        public static SvgTransformCollection Scale(this SvgTransformCollection collection, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(collection);
            collection.Add(new SvgScale(x, y));
            return collection;
        }

        #endregion Transform
    }
}
