using Reactive.Bindings;
using System.Text.RegularExpressions;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Data;

namespace TreeViewer.Models
{
    public partial class MainModel
    {
        #region Layout

        /// <summary>
        /// 折り畳みのタイプのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<CladeCollapseType> CollapseType { get; }

        /// <summary>
        /// 折りたたまれた三角形の幅のプロパティを取得します。
        /// </summary>
        /// <remarks><see cref="CollapseType"/>が<see cref="CladeCollapseType.Constant"/>の際に使用される</remarks>
        public ReactiveProperty<double> CollapsedConstantWidth { get; }

        #endregion Layout

        #region Tree

        /// <summary>
        /// X軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> XScale { get; }

        /// <summary>
        /// Y軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> YScale { get; }

        /// <summary>
        /// 枝の太さのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> BranchThickness { get; }

        /// <summary>
        /// 枝長が存在しない場合の値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<double> DefaultBranchLength { get; }

        #endregion Tree

        #region Search

        /// <summary>
        /// 検索ワードのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string> SearchQuery { get; }

        /// <summary>
        /// 検索対象を表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<TreeSearchTarget> SearchTarget { get; }

        /// <summary>
        /// 大文字・小文字を無視して検索を行うかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> SearchOnIgnoreCase { get; }

        /// <summary>
        /// 検索に正規表現を使うかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> SearchWithRegex { get; }

        /// <summary>
        /// 検索を実行します。
        /// </summary>
        public void Search()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            string query = SearchQuery.Value;
            if (string.IsNullOrEmpty(query)) return;

            Func<Clade, string?> cladeConverter;
            switch (SearchTarget.Value)
            {
                case TreeSearchTarget.Taxon:
                    SelectionTarget.Value = SelectionMode.Taxa;
                    cladeConverter = x => x.Taxon;
                    break;

                case TreeSearchTarget.Supports:
                    SelectionTarget.Value = SelectionMode.Node;
                    cladeConverter = x => x.Supports;
                    break;

                default: return;
            }

            Predicate<string> cladeSelection;
            if (SearchWithRegex.Value)
            {
                Regex regex;
                var option = RegexOptions.None;
                if (SearchOnIgnoreCase.Value) option |= RegexOptions.IgnoreCase;
                try
                {
                    regex = new Regex(query, option);
                }
                catch
                {
                    return;
                }
                cladeSelection = x => regex.IsMatch(x);
            }
            else
            {
                StringComparison stringComparison = SearchOnIgnoreCase.Value ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                cladeSelection = x => x.Contains(query, stringComparison);
            }

            IEnumerable<Clade> targetClades = tree.GetAllClades()
                                                  .Select(x => (clade: x, value: cladeConverter.Invoke(x)))
                                                  .Where(x => !string.IsNullOrEmpty(x.value) && cladeSelection.Invoke(x.value))
                                                  .Select(x => x.clade);
            Focus(targetClades);
        }

        #endregion Search

        #region LeafLabels

        /// <summary>
        /// 系統名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowLeafLabels { get; }

        /// <summary>
        /// 系統名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> LeafLabelsFontSize { get; }

        #endregion LeafLabels

        #region CladeLabels

        /// <summary>
        /// クレード名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowCladeLabels { get; }

        /// <summary>
        /// クレード名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> CladeLabelsFontSize { get; }

        /// <summary>
        /// クレード名脇の線幅のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> CladeLabelsLineThickness { get; }

        #endregion CladeLabels

        #region NodeValues

        /// <summary>
        /// 結節点の値を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowNodeValues { get; }

        /// <summary>
        /// 結節点の値の種類のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<CladeValueType> NodeValueType { get; }

        /// <summary>
        /// 結節点の値のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> NodeValueFontSize { get; }

        #endregion NodeValues

        #region BranchValues

        /// <summary>
        /// 二分岐の値を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowBranchValues { get; }

        /// <summary>
        /// 二分岐の値の種類のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<CladeValueType> BranchValueType { get; }

        /// <summary>
        /// 二分岐の値のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> BranchValueFontSize { get; }

        /// <summary>
        /// 非表示にする枝の値の正規表現パターンのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> BranchValueHideRegexPattern { get; }

        #endregion BranchValues

        #region BranchDecorations

        /// <summary>
        /// 二分岐の装飾を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowBranchDecorations { get; }

        /// <summary>
        /// 枝の装飾情報一覧を取得します。
        /// </summary>
        public ReactiveCollection<BranchDecorationModel> BranchDecorations { get; }

        /// <summary>
        /// 枝の装飾を追加します。
        /// </summary>
        public void AddNewBranchDecoration()
        {
            var style = new BranchDecorationStyle();
            var decorationViewModel = new BranchDecorationModel(this, style);

            OperateAsUndoable((arg, tree) =>
            {
                BranchDecorations.AddOnScheduler(arg.vm);
                tree.Style.DecorationStyles = [.. tree.Style.DecorationStyles, arg.style];

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                BranchDecorations.RemoveOnScheduler(arg.vm);
                tree.Style.DecorationStyles = tree.Style.DecorationStyles[..^1];

                NotifyTreeUpdated();
            }, (vm: decorationViewModel, style));
        }

        #endregion BranchDecorations

        #region Scalebar

        /// <summary>
        /// スケールバーを表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> ShowScaleBar { get; }

        /// <summary>
        /// スケールバーの数値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<double> ScaleBarValue { get; }

        /// <summary>
        /// スケールバーのフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> ScaleBarFontSize { get; }

        /// <summary>
        /// スケールバーの太さのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> ScaleBarThickness { get; }

        #endregion Scalebar
    }
}
