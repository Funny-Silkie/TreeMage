using Reactive.Bindings;
using TreeViewer.Core.Trees;
using TreeViewer.Data;

namespace TreeViewer.Models
{
    public partial class MainModel
    {
        /// <summary>
        /// 対象の系統樹のインデックスのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> TreeIndex { get; }

        /// <summary>
        /// <see cref="TreeIndex"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnTreeIndexChanged(int value)
        {
            if (onUndoOperation) return;

            value--;
            if ((uint)value >= (uint)Trees.Count) return;

            Tree? prevTree = TargetTree.Value;
            Tree nextTree = Trees[value];
            if (prevTree is null)
            {
                TargetTree.Value = Trees[value];
                LoadTreeStyle(nextTree);
            }
            else
            {
                int prevIndex = Trees.IndexOf(prevTree);

                OperateAsUndoable(arg =>
                {
                    ApplyTreeStyle(arg.prevTree);

                    UnfocusAll();
                    TargetTree.Value = arg.nextTree;

                    LoadTreeStyle(arg.nextTree);

                    TreeIndex.Value = arg.nextIndex + 1;
                    NotifyTreeUpdated();
                }, arg =>
                {
                    ApplyTreeStyle(arg.nextTree);

                    UnfocusAll();
                    TargetTree.Value = arg.prevTree;

                    LoadTreeStyle(arg.prevTree);

                    TreeIndex.Value = arg.prevIndex + 1;
                    NotifyTreeUpdated();
                }, (prevTree, nextTree, prevIndex, nextIndex: value));
            }
        }

        /// <summary>
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> MaxTreeIndex { get; }

        /// <summary>
        /// 編集モードのプロパティを取得します。
        /// </summary>
        public ReactiveProperty<TreeEditMode> EditMode { get; }

        /// <summary>
        /// 選択モードを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<SelectionMode> SelectionTarget { get; }

        /// <summary>
        /// <see cref="SelectionTarget"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnSelectionTargetChanged(SelectionMode value)
        {
            if (FocusedSvgElementIdList.Count == 0) return;

            HashSet<Clade> selectedClades = FocusedSvgElementIdList.Select(x => x.Clade)
                                                                   .ToHashSet();

            switch (value)
            {
                case SelectionMode.Node:
                    Focus(selectedClades);
                    break;

                case SelectionMode.Clade:
                    Focus(selectedClades.SelectMany(x => x.GetDescendants().Prepend(x)));
                    break;

                case SelectionMode.Taxa:
                    Focus(selectedClades.SelectMany(x => x.GetDescendants().Prepend(x)));
                    break;
            }
        }
    }
}
