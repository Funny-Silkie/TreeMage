using System.Text.RegularExpressions;

namespace TreeViewer.Core.Drawing.Styles
{
    /// <summary>
    /// 枝の装飾のスタイルを表します。
    /// </summary>
    public partial class BranchDecorationStyle
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
    }
}
