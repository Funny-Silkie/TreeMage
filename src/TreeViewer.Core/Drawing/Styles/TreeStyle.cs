using TreeViewer.Core.Trees;

namespace TreeViewer.Core.Drawing.Styles
{
    /// <summary>
    /// ツリーのスタイルを表します。
    /// </summary>
    public class TreeStyle
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
        /// <see cref="TreeStyle"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TreeStyle()
        {
        }
    }
}
