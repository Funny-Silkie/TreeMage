using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Exporting;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// ツリーを描画するインターフェイスを表します。
    /// </summary>
    public interface ITreeDrawer
    {
        /// <summary>
        /// 座標を扱うオブジェクトを取得します。
        /// </summary>
        PositionManager PositionManager { get; }

        /// <summary>
        /// ツリーを描画します。
        /// </summary>
        /// <param name="tree">対象のツリー</param>
        /// <param name="options">描画用オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="tree"/>または<paramref name="options"/>が<see langword="null"/></exception>
        void Draw(Tree tree, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(options);

            PositionManager.Reset(tree);
            InitDocument();

            (double documentWidth, double documentHeight) = PositionManager.CalcDocumentSize();

            BeginTree(documentWidth, documentHeight, tree);

            #region 系統樹部分

            foreach (Clade current in tree.GetAllClades())
            {
                if (current.GetIsHidden()) continue;

                // クレードのシェード
                if (!string.IsNullOrEmpty(current.Style.ShadeColor))
                {
                    (double x, double y, double width, double height) = PositionManager.CalcCladeShadePosition(current);
                    DrawCladeShade(x, y, width, height, current.Style.ShadeColor);
                }

                // 折りたたみ
                if (current.GetIsExternal() && !current.IsLeaf)
                {
                    var (left, rightTop, rightBottom) = PositionManager.CalcCollapseTrianglePositions(current);
                    DrawCollapsedTriangle(left, rightTop, rightBottom, current.Style.BranchColor, tree.Style.BranchThickness);
                }

                if (current.IsLeaf)
                {
                    // 系統名
                    if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                    {
                        (double x, double y, _, _) = PositionManager.CalcLeafPosition(current);
                        DrawLeafLabel(current.Taxon, x, y, current.Style.LeafColor, tree.Style.LeafLabelsFontSize);
                    }
                }
                else
                {
                    // 結節点の値
                    if (tree.Style.ShowNodeValues && !current.GetIsExternal())
                    {
                        string nodeValue = DrawHelpers.SelectShowValue(current, tree.Style.NodeValueType);
                        if (nodeValue.Length > 0)
                        {
                            (double x, double y) = PositionManager.CalcNodeValuePosition(current, nodeValue);
                            DrawNodeValue(nodeValue, x, y, current.Style.BranchColor, tree.Style.NodeValueFontSize);
                        }
                    }
                }

                // クレード名
                if (tree.Style.ShowCladeLabels && !string.IsNullOrEmpty(current.Style.CladeLabel))
                {
                    var (line, text) = PositionManager.CalcCladeLabelPosition(current);
                    DrawCladeLabel(current.Style.CladeLabel, line, text, tree.Style.CladeLabelsLineThickness, tree.Style.CladeLabelsFontSize);
                }

                if (current.GetDrawnBranchLength() > 0)
                {
                    // 横棒
                    {
                        (double xParent, double xChild, double y) = PositionManager.CalcHorizontalBranchPositions(current);
                        DrawHorizontalBranch(xParent, xChild, y, options.BranchColoring is BranchColoringType.Both or BranchColoringType.Horizontal ? current.Style.BranchColor : "black", tree.Style.BranchThickness);
                    }

                    // 枝の装飾
                    if (tree.Style.ShowBranchDecorations && !string.IsNullOrEmpty(current.Supports))
                        foreach (BranchDecorationStyle currentDecoration in tree.Style.DecorationStyles.Where(x => x.Enabled && (x.Regex?.IsMatch(current.Supports) ?? false)))
                            DrawBranchDecoration(current, currentDecoration);

                    // 枝の値
                    if (tree.Style.ShowBranchValues)
                    {
                        string branchValue = DrawHelpers.SelectShowValue(current, tree.Style.BranchValueType);
                        if (branchValue.Length > 0 && (!tree.Style.BranchValueHideRegex?.IsMatch(branchValue) ?? true))
                        {
                            (double x, double y) = PositionManager.CalcBranchValuePosition(current, branchValue);
                            DrawBranchValue(branchValue, x, y, current.Style.BranchColor, tree.Style.BranchValueFontSize);
                        }
                    }
                }

                Clade? parent = current.Parent;
                if (parent is not null)
                {
                    if (parent.Children.Count > 1)
                    {
                        (double x, double yParent, double yChild) = PositionManager.CalcVerticalBranchPositions(current);

                        // 縦棒
                        if (yParent != yChild) DrawVerticalBranch(x, yParent, yChild, options.BranchColoring is BranchColoringType.Both or BranchColoringType.Vertical ? current.Style.BranchColor : "black", tree.Style.BranchThickness);
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (tree.Style.ShowScaleBar && tree.Style.ScaleBarValue > 0)
            {
                (double offsetX, double offsetY) = PositionManager.CalcScaleBarOffset();
                ((double xLeft, double xRight, double y) line, (double x, double y) text) = PositionManager.CalcScaleBarPositions();

                DrawScalebar(tree.Style.ScaleBarValue, offsetX, offsetY, line, text, tree.Style.ScaleBarFontSize, tree.Style.ScaleBarThickness);
            }

            #endregion スケールバー

            FinishTree();
        }

        /// <summary>
        /// ドキュメントを初期化します。
        /// </summary>
        void InitDocument();

        /// <summary>
        /// ツリーの描画を開始します。
        /// </summary>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <param name="tree">ツリー</param>
        void BeginTree(double width, double height, Tree tree);

        /// <summary>
        /// クレードのシェードを描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <param name="fill">色</param>
        void DrawCladeShade(double x, double y, double width, double height, string fill);

        /// <summary>
        /// 折りたたみの三角形を描画します。
        /// </summary>
        /// <param name="left">左の頂点座標</param>
        /// <param name="rightTop">右上の頂点座標</param>
        /// <param name="rightBottom">右下の頂点座標</param>
        /// <param name="stroke">線の色</param>
        /// <param name="lineThickness">線の太さ</param>
        void DrawCollapsedTriangle((double x, double y) left, (double x, double y) rightTop, (double x, double y) rightBottom, string stroke, int lineThickness);

        /// <summary>
        /// 葉を描画します。
        /// </summary>
        /// <param name="taxon">系統名</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawLeafLabel(string taxon, double x, double y, string fill, int fontSize);

        /// <summary>
        /// 結節点の値を描画します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawNodeValue(string value, double x, double y, string fill, int fontSize);

        /// <summary>
        /// 枝の値を描画します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawBranchValue(string value, double x, double y, string fill, int fontSize);

        /// <summary>
        /// クレード名を描画します。
        /// </summary>
        /// <param name="cladeName">クレード名</param>
        /// <param name="linePosition">線の座標</param>
        /// <param name="textPosition">文字の座標</param>
        /// <param name="lineThickness">線の太さ</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawCladeLabel(string cladeName, (double x, double yTop, double yBottom) linePosition, (double x, double y) textPosition, int lineThickness, int fontSize);

        /// <summary>
        /// 枝の横線を描画します。
        /// </summary>
        /// <param name="x1">X座標1</param>
        /// <param name="x2">X座標2</param>
        /// <param name="y">Y座標</param>
        /// <param name="stroke">色</param>
        /// <param name="thickness">太さ</param>
        void DrawHorizontalBranch(double x1, double x2, double y, string stroke, int thickness);

        /// <summary>
        /// 枝の横線を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y1">Y座標1</param>
        /// <param name="y2">Y座標2</param>
        /// <param name="stroke">色</param>
        /// <param name="thickness">太さ</param>
        void DrawVerticalBranch(double x, double y1, double y2, string stroke, int thickness);

        /// <summary>
        /// 枝の装飾を描画します。
        /// </summary>
        /// <param name="target">対象のクレード</param>
        /// <param name="style">スタイル</param>
        void DrawBranchDecoration(Clade target, BranchDecorationStyle style);

        /// <summary>
        /// スケールバーを描画します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="offsetX">オフセットのX座標</param>
        /// <param name="offsetY">オフセットのY座標</param>
        /// <param name="linePosition">線の座標</param>
        /// <param name="textPosition">文字の座標</param>
        /// <param name="fontSize">フォントサイズ</param>
        /// <param name="lineThickness">線の太さ</param>
        void DrawScalebar(double value, double offsetX, double offsetY, (double, double, double) linePosition, (double, double) textPosition, int fontSize, int lineThickness);

        /// <summary>
        /// ツリーの描画を終了します。
        /// </summary>
        void FinishTree();
    }
}
