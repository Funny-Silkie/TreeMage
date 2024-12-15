using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TreeViewer.Core.Drawing.Styles
{
    /// <summary>
    /// 枝の装飾のスタイルを表します。
    /// </summary>
    public partial class BranchDecorationStyle : ICloneable
    {
        /// <summary>
        /// 有効かどうかを表す値を取得または設定します。
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 装飾する枝の正規表現パターンを取得します。
        /// </summary>
        /// <remarks><see cref="RegexPattern"/>が無効な場合は<see langword="null"/></remarks>
        public Regex? Regex { get; private set; }

        /// <summary>
        /// 正規表現パターンを取得または設定します。
        /// </summary>
        [StringSyntax(StringSyntaxAttribute.Regex)]
        public string? RegexPattern
        {
            get => _regexPattern;
            set
            {
                if (value == _regexPattern) return;
                if (string.IsNullOrEmpty(value)) Regex = null;
                else
                    try
                    {
                        Regex = new Regex(value);
                    }
                    catch
                    {
                        Regex = null;
                    }
                _regexPattern = value;
            }
        }

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private string? _regexPattern;

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
        /// 他のスタイルの内容を適用します。
        /// </summary>
        /// <param name="style">適用するインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="style"/>が<see langword="null"/></exception>
        public void ApplyValues(BranchDecorationStyle style)
        {
            ArgumentNullException.ThrowIfNull(style);

            Enabled = style.Enabled;
            Regex = style.Regex;
            _regexPattern = style._regexPattern;
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
            Enabled = Enabled,
            Regex = Regex,
            _regexPattern = _regexPattern,
            ShapeSize = ShapeSize,
            DecorationType = DecorationType,
            ShapeColor = ShapeColor,
        };

        object ICloneable.Clone() => Clone();
    }
}
