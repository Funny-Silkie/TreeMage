using System.Text.RegularExpressions;

namespace TreeViewer.Core.Drawing.Styles
{
    /// <summary>
    /// 枝の装飾のスタイルを表します。
    /// </summary>
    public partial class BranchDecorationStyle : ICloneable
    {
        /// <summary>
        /// 装飾する枝の正規表現パターンを取得または設定します。
        /// </summary>
        public Regex Regex { get; set; } = GetDefaultRegex();

        /// <summary>
        /// 図形のサイズを取得または設定します。
        /// </summary>
        public int ShapeSize { get; set; } = 5;

        /// <summary>
        /// 装飾のタイプを取得または設定します。
        /// </summary>
        public BranchDecorationType DecorationType { get; set; } = BranchDecorationType.ClosedCircle;

        /// <summary>
        /// 装飾の色を取得または設定します。
        /// </summary>
        public string ShapeColor { get; set; } = "black";

        /// <summary>
        /// <see cref="BranchDecorationStyle"/>の新しいインスタンスを初期化します。
        /// </summary>
        public BranchDecorationStyle()
        {
        }

        /// <summary>
        /// デフォルトの<see cref="Regex"/>を取得します。
        /// </summary>
        /// <returns>デフォルトの<see cref="Regex"/>に用いる値</returns>
        [GeneratedRegex("100")]
        private static partial Regex GetDefaultRegex();

        /// <summary>
        /// 他のスタイルの内容を適用します。
        /// </summary>
        /// <param name="style">適用するインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="style"/>が<see langword="null"/></exception>
        public void ApplyValues(BranchDecorationStyle style)
        {
            ArgumentNullException.ThrowIfNull(style);

            Regex = style.Regex;
            ShapeSize = style.ShapeSize;
            DecorationType = style.DecorationType;
            ShapeColor = style.ShapeColor;
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public BranchDecorationStyle Clone() => new BranchDecorationStyle()
        {
            Regex = Regex,
            ShapeSize = ShapeSize,
            DecorationType = DecorationType,
            ShapeColor = ShapeColor,
        };

        object ICloneable.Clone() => Clone();
    }
}
