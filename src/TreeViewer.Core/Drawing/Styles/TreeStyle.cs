using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing.Styles
{
    /// <summary>
    /// ツリーのスタイルを表します。
    /// </summary>
    public class TreeStyle : ICloneable
    {
        /// <summary>
        /// X方向の拡大率を取得または設定します。
        /// </summary>
        public int XScale { get; set; } = 300;

        /// <summary>
        /// Y方向の拡大率を取得または設定します。
        /// </summary>
        public int YScale { get; set; } = 30;

        /// <summary>
        /// 枝の太さを取得または設定します。
        /// </summary>
        public int BranchThickness { get; set; } = 1;

        /// <summary>
        /// 系統名を出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowLeafLabels { get; set; } = true;

        /// <summary>
        /// 系統名のフォントサイズを取得または設定します。
        /// </summary>
        public int LeafLabelsFontSize { get; set; } = 20;

        /// <summary>
        /// クレード名を出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowCladeLabels { get; set; } = true;

        /// <summary>
        /// クレード名のフォントサイズを取得または設定します。
        /// </summary>
        public int CladeLabelsFontSize { get; set; } = 20;

        /// <summary>
        /// クレード名の脇の直線の太さを取得または設定します。
        /// </summary>
        public int CladeLabelsLineThickness { get; set; } = 5;

        /// <summary>
        /// 結節点の値を出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowNodeValues { get; set; } = true;

        /// <summary>
        /// 出力する結節点の値の種類を取得または設定します。
        /// </summary>
        public CladeValueType NodeValueType { get; set; } = CladeValueType.Supports;

        /// <summary>
        /// 結節点の値のフォントサイズを取得または設定します。
        /// </summary>
        public int NodeValueFontSize { get; set; } = 15;

        /// <summary>
        /// 枝の値を出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowBranchValues { get; set; } = true;

        /// <summary>
        /// 出力する枝の値の種類を取得または設定します。
        /// </summary>
        public CladeValueType BranchValueType { get; set; } = CladeValueType.BranchLength;

        /// <summary>
        /// 枝の値のフォントサイズを取得または設定します。
        /// </summary>
        public int BranchValueFontSize { get; set; } = 15;

        /// <summary>
        /// 枝の値を非表示にする正規表現を取得します。
        /// </summary>
        public Regex? BranchValueHideRegex { get; private set; }

        /// <summary>
        /// 枝の値を非表示にする正規表現パターンを取得または設定します。
        /// </summary>
        [StringSyntax(StringSyntaxAttribute.Regex)]
        public string? BranchValueHideRegexPattern
        {
            get => _branchValueHideRegexPattern;
            set
            {
                if (_branchValueHideRegexPattern == value) return;
                if (string.IsNullOrEmpty(value)) BranchValueHideRegex = null;
                else
                    try
                    {
                        BranchValueHideRegex = new Regex(value);
                    }
                    catch
                    {
                        BranchValueHideRegex = null;
                    }
                _branchValueHideRegexPattern = value;
            }
        }

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private string? _branchValueHideRegexPattern;

        /// <summary>
        /// 枝の装飾を出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowBranchDecorations { get; set; } = true;

        /// <summary>
        /// 枝の装飾スタイル一覧を取得または設定します。
        /// </summary>
        public BranchDecorationStyle[] DecorationStyles { get; set; } = [];

        /// <summary>
        /// スケールバーを出力するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool ShowScaleBar { get; set; } = true;

        /// <summary>
        /// スケールバーの縮尺を取得または設定します。
        /// </summary>
        public double ScaleBarValue { get; set; } = 0.1;

        /// <summary>
        /// スケールバーのフォントサイズを取得または設定します。
        /// </summary>
        public int ScaleBarFontSize { get; set; } = 25;

        /// <summary>
        /// スケールバーの太さを取得または設定します。
        /// </summary>
        public int ScaleBarThickness { get; set; } = 5;

        /// <summary>
        /// 折り畳みのタイプを取得または設定します。
        /// </summary>
        public CladeCollapseType CollapseType { get; set; } = CladeCollapseType.TopMax;

        /// <summary>
        /// 折りたたまれた三角形の幅を取得または設定します。
        /// </summary>
        /// <remarks><see cref="CollapseType"/>が<see cref="CladeCollapseType.Constant"/>の際に使用される</remarks>
        public double CollapsedConstantWidth { get; set; } = 1;

        /// <summary>
        /// <see cref="TreeStyle"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TreeStyle()
        {
        }

        /// <summary>
        /// 他のスタイルの内容を適用します。
        /// </summary>
        /// <param name="style">適用するインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="style"/>が<see langword="null"/></exception>
        public void ApplyValues(TreeStyle style)
        {
            ArgumentNullException.ThrowIfNull(style);

            XScale = style.XScale;
            YScale = style.YScale;
            BranchThickness = style.BranchThickness;
            ShowLeafLabels = style.ShowLeafLabels;
            LeafLabelsFontSize = style.LeafLabelsFontSize;
            ShowCladeLabels = style.ShowCladeLabels;
            CladeLabelsFontSize = style.CladeLabelsFontSize;
            CladeLabelsLineThickness = style.CladeLabelsLineThickness;
            ShowNodeValues = style.ShowNodeValues;
            NodeValueType = style.NodeValueType;
            NodeValueFontSize = style.NodeValueFontSize;
            ShowBranchValues = style.ShowBranchValues;
            BranchValueType = style.BranchValueType;
            BranchValueFontSize = style.BranchValueFontSize;
            BranchValueHideRegex = style.BranchValueHideRegex;
            _branchValueHideRegexPattern = style._branchValueHideRegexPattern;
            ShowBranchDecorations = style.ShowBranchDecorations;
            DecorationStyles = Array.ConvertAll(style.DecorationStyles, x => x.Clone());
            ShowScaleBar = style.ShowScaleBar;
            ScaleBarValue = style.ScaleBarValue;
            ScaleBarFontSize = style.ScaleBarFontSize;
            ScaleBarThickness = style.ScaleBarThickness;
            CollapseType = style.CollapseType;
            CollapsedConstantWidth = style.CollapsedConstantWidth;
        }

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public TreeStyle Clone() => new TreeStyle()
        {
            XScale = XScale,
            YScale = YScale,
            BranchThickness = BranchThickness,
            ShowLeafLabels = ShowLeafLabels,
            LeafLabelsFontSize = LeafLabelsFontSize,
            ShowCladeLabels = ShowCladeLabels,
            CladeLabelsFontSize = CladeLabelsFontSize,
            CladeLabelsLineThickness = CladeLabelsLineThickness,
            ShowNodeValues = ShowNodeValues,
            NodeValueType = NodeValueType,
            NodeValueFontSize = NodeValueFontSize,
            ShowBranchValues = ShowBranchValues,
            BranchValueType = BranchValueType,
            BranchValueFontSize = BranchValueFontSize,
            BranchValueHideRegex = BranchValueHideRegex,
            _branchValueHideRegexPattern = _branchValueHideRegexPattern,
            ShowBranchDecorations = ShowBranchDecorations,
            DecorationStyles = Array.ConvertAll(DecorationStyles, x => x.Clone()),
            ShowScaleBar = ShowScaleBar,
            ScaleBarValue = ScaleBarValue,
            ScaleBarFontSize = ScaleBarFontSize,
            ScaleBarThickness = ScaleBarThickness,
            CollapseType = CollapseType,
            CollapsedConstantWidth = CollapsedConstantWidth,
        };

        object ICloneable.Clone() => Clone();
    }
}
