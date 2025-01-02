using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Trees;
using TreeViewer.Data;

namespace TreeViewer.Models
{
    public partial class MainModel
    {
        /// <summary>
        /// <see cref="MainModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainModel()
        {
            Trees = new ReactiveCollection<Tree>().AddTo(Disposables);
            TargetTree = new ReactiveProperty<Tree?>().AddTo(Disposables);
            FocusedSvgElementIdList = [];
            ProjectPath = new ReactiveProperty<string?>().AddTo(Disposables);

            #region Header

            TreeIndex = new ReactiveProperty<int>(1).WithSubscribe(OnTreeIndexChanged)
                                                        .AddTo(Disposables);
            MaxTreeIndex = new ReactiveProperty<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            EditMode = new ReactiveProperty<TreeEditMode>().AddTo(Disposables);
            EditMode.Zip(EditMode.Skip(1), (x, y) => (before: x, after: y)).Subscribe(v => OperateAsUndoable((arg, tree) =>
            {
                EditMode.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                EditMode.Value = arg.before;

                NotifyTreeUpdated();
            }, v));
            SelectionTarget = new ReactiveProperty<SelectionMode>(SelectionMode.Node).WithSubscribe(OnSelectionTargetChanged)
                                                                                     .AddTo(Disposables);

            #endregion Header

            #region TreeEditSidebar

            #region Layout

            CollapseType = new ReactiveProperty<CladeCollapseType>(CladeCollapseType.TopMax).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapseType = arg.after;
                CollapseType!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.CollapseType = arg.before;
                CollapseType!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.CollapseType ?? CladeCollapseType.TopMax, after: v))).AddTo(Disposables);
            CollapsedConstantWidth = new ReactiveProperty<double>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.after;
                CollapsedConstantWidth!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.before;
                CollapsedConstantWidth!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.CollapsedConstantWidth ?? 1, after: v))).AddTo(Disposables);

            #endregion Layout

            #region Tree

            XScale = new ReactiveProperty<int>(300).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.XScale = arg.after;
                XScale!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.XScale = arg.before;
                XScale!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.XScale ?? 0, after: v))).AddTo(Disposables);
            YScale = new ReactiveProperty<int>(30).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.YScale = arg.after;
                YScale!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.YScale = arg.before;
                YScale!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.YScale ?? 0, after: v))).AddTo(Disposables);
            BranchThickness = new ReactiveProperty<int>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchThickness = arg.after;
                BranchThickness!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.BranchThickness = arg.before;
                BranchThickness!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.BranchThickness ?? 0, after: v))).AddTo(Disposables);

            #endregion Tree

            #region Search

            SearchQuery = new ReactiveProperty<string>(string.Empty).AddTo(Disposables);
            SearchTarget = new ReactiveProperty<TreeSearchTarget>().AddTo(Disposables);
            SearchOnIgnoreCase = new ReactiveProperty<bool>(false).AddTo(Disposables);
            SearchWithRegex = new ReactiveProperty<bool>().AddTo(Disposables);

            #endregion Search

            #region LeafLabels

            ShowLeafLabels = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.after;
                ShowLeafLabels!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.before;
                ShowLeafLabels!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowLeafLabels ?? false, after: v))).AddTo(Disposables);
            LeafLabelsFontSize = new ReactiveProperty<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.after;
                LeafLabelsFontSize!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.before;
                LeafLabelsFontSize!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.LeafLabelsFontSize ?? 0, after: v))).AddTo(Disposables);

            #endregion LeafLabels

            #region CladeLabels

            ShowCladeLabels = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.after;
                ShowCladeLabels!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.before;
                ShowCladeLabels!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowCladeLabels ?? false, after: v))).AddTo(Disposables);
            CladeLabelsFontSize = new ReactiveProperty<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.after;
                CladeLabelsFontSize!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.before;
                CladeLabelsFontSize!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.CladeLabelsFontSize ?? 0, after: v))).AddTo(Disposables);
            CladeLabelsLineThickness = new ReactiveProperty<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.after;
                CladeLabelsLineThickness!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.before;
                CladeLabelsLineThickness!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.CladeLabelLineThickness ?? 0, after: v)));

            #endregion CladeLabels

            #region NodeValues

            ShowNodeValues = new ReactiveProperty<bool>(false).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.after;
                ShowNodeValues!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.before;
                ShowNodeValues!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowNodeValues ?? false, after: v))).AddTo(Disposables);
            NodeValueType = new ReactiveProperty<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueType = arg.after;
                NodeValueType!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueType = arg.before;
                NodeValueType!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.NodeValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            NodeValueFontSize = new ReactiveProperty<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.after;
                NodeValueFontSize!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.before;
                NodeValueFontSize!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.NodeValueFontSize ?? 0, after: v))).AddTo(Disposables);

            #endregion NodeValues

            #region BranchValues

            ShowBranchValues = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.after;
                ShowBranchValues!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.before;
                ShowBranchValues!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowBranchValues ?? false, after: v))).AddTo(Disposables);
            BranchValueType = new ReactiveProperty<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueType = arg.after;
                BranchValueType!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueType = arg.before;
                BranchValueType!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.BranchValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            BranchValueFontSize = new ReactiveProperty<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.after;
                BranchValueFontSize!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.before;
                BranchValueFontSize!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.BranchValueFontSize ?? 0, after: v))).AddTo(Disposables);
            BranchValueHideRegexPattern = new ReactiveProperty<string?>().WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.after;
                BranchValueHideRegexPattern!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.before;
                BranchValueHideRegexPattern!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.BranchValueHideRegexPattern, after: v))).AddTo(Disposables);

            #endregion BranchValues

            #region BranchDecorations

            ShowBranchDecorations = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.after;
                ShowBranchDecorations!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.before;
                ShowBranchDecorations!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowBranchDecorations ?? false, after: v))).AddTo(Disposables);
            BranchDecorations = new ReactiveCollection<BranchDecorationModel>().AddTo(Disposables);

            #endregion BranchDecorations

            #region ScaleBar

            ShowScaleBar = new ReactiveProperty<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.after;
                ShowScaleBar!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.before;
                ShowScaleBar!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ShowScaleBar ?? false, after: v))).AddTo(Disposables);
            ScaleBarValue = new ReactiveProperty<double>(0.1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.after;
                ScaleBarValue!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.before;
                ScaleBarValue!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ScaleBarValue ?? 0.1, after: v))).AddTo(Disposables);
            ScaleBarFontSize = new ReactiveProperty<int>(25).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.after;
                ScaleBarFontSize!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.before;
                ScaleBarFontSize!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ScaleBarFontSize ?? 0, after: v))).AddTo(Disposables);
            ScaleBarThickness = new ReactiveProperty<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.after;
                ScaleBarThickness!.Value = arg.after;

                NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.before;
                ScaleBarThickness!.Value = arg.before;

                NotifyTreeUpdated();
            }, (before: TargetTree.Value?.Style?.ScaleBarThickness ?? 0, after: v))).AddTo(Disposables);

            #endregion ScaleBar

            #endregion TreeEditSidebar

            undoService.Clear();
        }
    }
}
