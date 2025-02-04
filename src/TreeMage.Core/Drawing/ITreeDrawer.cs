using TreeMage.Core.Drawing.Styles;
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
        void Draw(Tree tree, DrawingOptions options)
        {
            ArgumentNullException.ThrowIfNull(tree);
            ArgumentNullException.ThrowIfNull(options);

            PositionManager.Reset(tree);
            InitDocument();

            TMSize documentSize = PositionManager.CalcDocumentSize();

            BeginTree(documentSize, tree);

            #region 系統樹部分

            foreach (Clade current in tree.GetAllClades())
            {
                if (current.GetIsHidden()) continue;

                // クレードのシェード
                if (!string.IsNullOrEmpty(current.Style.ShadeColor))
                {
                    TMRect area = PositionManager.CalcCladeShadePosition(current);
                    DrawCladeShade(area, new TMColor(current.Style.ShadeColor));
                }

                // 折りたたみ
                if (current.GetIsExternal() && !current.IsLeaf)
                {
                    (TMPoint left, TMPoint rightTop, TMPoint rightBottom) = PositionManager.CalcCollapseTrianglePositions(current);
                    DrawCollapsedTriangle(left, rightTop, rightBottom, new TMColor(current.Style.BranchColor), tree.Style.BranchThickness);
                }

                if (current.IsLeaf)
                {
                    // 系統名
                    if (tree.Style.ShowLeafLabels && !string.IsNullOrEmpty(current.Taxon))
                    {
                        TMRect area = PositionManager.CalcLeafPosition(current);
                        DrawLeafLabel(current.Taxon, area.Point, new TMColor(current.Style.LeafColor), tree.Style.LeafLabelsFontSize);
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
                            TMPoint point = PositionManager.CalcNodeValuePosition(current, nodeValue);
                            DrawNodeValue(nodeValue, point, new TMColor(current.Style.BranchColor), tree.Style.NodeValueFontSize);
                        }
                    }
                }

                // クレード名
                if (tree.Style.ShowCladeLabels && !string.IsNullOrEmpty(current.Style.CladeLabel))
                {
                    (TMPoint lineBegin, TMPoint lineEnd, TMPoint text) = PositionManager.CalcCladeLabelPosition(current);
                    DrawCladeLabel(current.Style.CladeLabel, lineBegin, lineEnd, text, tree.Style.CladeLabelsLineThickness, tree.Style.CladeLabelsFontSize);
                }

                if (current.GetDrawnBranchLength() > 0)
                {
                    // 横棒
                    {
                        (TMPoint parentPoint, TMPoint childPoint) = PositionManager.CalcHorizontalBranchPositions(current);
                        DrawHorizontalBranch(parentPoint, childPoint, options.BranchColoring is BranchColoringType.Both or BranchColoringType.Horizontal ? new TMColor(current.Style.BranchColor) : new TMColor("black"), tree.Style.BranchThickness);
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
                            TMPoint point = PositionManager.CalcBranchValuePosition(current, branchValue);
                            DrawBranchValue(branchValue, point, new TMColor(current.Style.BranchColor), tree.Style.BranchValueFontSize);
                        }
                    }
                }

                Clade? parent = current.Parent;
                if (parent is not null)
                {
                    if (parent.Children.Count > 1)
                    {
                        (TMPoint parentPoint, TMPoint childPoint) = PositionManager.CalcVerticalBranchPositions(current);

                        // 縦棒
                        if (parentPoint != childPoint) DrawVerticalBranch(parentPoint, childPoint, options.BranchColoring is BranchColoringType.Both or BranchColoringType.Vertical ? new TMColor(current.Style.BranchColor) : new TMColor("black"), tree.Style.BranchThickness);
                    }
                }
            }

            #endregion 系統樹部分

            #region スケールバー

            if (tree.Style.ShowScaleBar && tree.Style.ScaleBarValue > 0)
            {
                TMPoint offset = PositionManager.CalcScaleBarOffset();
                (TMPoint lineBegin, TMPoint lineEnd, TMPoint text) = PositionManager.CalcScaleBarPositions();

                DrawScalebar(tree.Style.ScaleBarValue, offset, lineBegin, lineEnd, text, tree.Style.ScaleBarFontSize, tree.Style.ScaleBarThickness);
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
        /// <param name="size">描画サイズ</param>
        /// <param name="tree">ツリー</param>
        void BeginTree(TMSize size, Tree tree);

        /// <summary>
        /// クレードのシェードを描画します。
        /// </summary>
        /// <param name="area">描画範囲</param>
        /// <param name="fill">色</param>
        void DrawCladeShade(TMRect area, TMColor fill);

        /// <summary>
        /// 折りたたみの三角形を描画します。
        /// </summary>
        /// <param name="left">左の頂点座標</param>
        /// <param name="rightTop">右上の頂点座標</param>
        /// <param name="rightBottom">右下の頂点座標</param>
        /// <param name="stroke">線の色</param>
        /// <param name="lineThickness">線の太さ</param>
        void DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness);

        /// <summary>
        /// 葉を描画します。
        /// </summary>
        /// <param name="taxon">系統名</param>
        /// <param name="point">座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawLeafLabel(string taxon, TMPoint point, TMColor fill, int fontSize);

        /// <summary>
        /// 結節点の値を描画します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="point">座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize);

        /// <summary>
        /// 枝の値を描画します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="point">座標</param>
        /// <param name="fill">文字色</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize);

        /// <summary>
        /// クレード名を描画します。
        /// </summary>
        /// <param name="cladeName">クレード名</param>
        /// <param name="lineBegin">線の開始点</param>
        /// <param name="lineEnd">線の終了点</param>
        /// <param name="testPoint">文字の座標</param>
        /// <param name="lineThickness">線の太さ</param>
        /// <param name="fontSize">フォントサイズ</param>
        void DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint testPoint, int lineThickness, int fontSize);

        /// <summary>
        /// 枝の横線を描画します。
        /// </summary>
        /// <param name="parentPoint">親側の座標</param>
        /// <param name="childPoint">子側の座標</param>
        /// <param name="stroke">色</param>
        /// <param name="thickness">太さ</param>
        void DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness);

        /// <summary>
        /// 枝の横線を描画します。
        /// </summary>
        /// <param name="parentPoint">親側の座標</param>
        /// <param name="childPoint">子側の座標</param>
        /// <param name="stroke">色</param>
        /// <param name="thickness">太さ</param>
        void DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness);

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
        /// <param name="offset">オフセット</param>
        /// <param name="lineBegin">線の開始点</param>
        /// <param name="lineEnd">線の終了点</param>
        /// <param name="textPoint">文字の座標</param>
        /// <param name="fontSize">フォントサイズ</param>
        /// <param name="lineThickness">線の太さ</param>
        void DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness);

        /// <summary>
        /// ツリーの描画を終了します。
        /// </summary>
        void FinishTree();
    }
}
