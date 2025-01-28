using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeMage.Core.Drawing.Styles;

namespace TreeMage.Models
{
    /// <summary>
    /// 枝の装飾のModelクラスです。
    /// </summary>
    public class BranchDecorationModel : ModelBase
    {
        private readonly MainModel mainModel;

        /// <summary>
        /// 対象のスタイルを取得します。
        /// </summary>
        public BranchDecorationStyle Style { get; }

        /// <summary>
        /// 対象の正規表現のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string?> TargetRegexPattern { get; }

        /// <summary>
        /// 装飾の種類のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<BranchDecorationType> DecorationType { get; }

        /// <summary>
        /// 図形のサイズを表すプロパティを取得します。
        /// </summary>
        public ReactiveProperty<int> ShapeSize { get; }

        /// <summary>
        /// 図形の色を表すプロパティを取得します。
        /// </summary>
        public ReactiveProperty<string> ShapeColor { get; }

        /// <summary>
        /// 表示するどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactiveProperty<bool> Visible { get; }

        /// <summary>
        /// <see cref="BranchDecorationModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">親となる<see cref="MainModel"/>のインスタンス</param>
        /// <param name="style">対象のスタイル</param>
        public BranchDecorationModel(MainModel mainModel, BranchDecorationStyle style)
        {
            this.mainModel = mainModel;
            Style = style;

            TargetRegexPattern = new ReactiveProperty<string?>(Style.RegexPattern, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.mainModel.OperateAsUndoable(arg =>
            {
                TargetRegexPattern!.Value = arg.after;
                Style.RegexPattern = arg.after;
                this.mainModel.NotifyTreeUpdated();
            }, arg =>
            {
                TargetRegexPattern!.Value = arg.before;
                Style.RegexPattern = arg.before;
                this.mainModel.NotifyTreeUpdated();
            }, (before: Style.RegexPattern, after: v))).AddTo(Disposables);
            DecorationType = new ReactiveProperty<BranchDecorationType>(Style.DecorationType, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.mainModel.OperateAsUndoable(arg =>
            {
                DecorationType!.Value = arg.after;
                Style.DecorationType = arg.after;
                this.mainModel.NotifyTreeUpdated();
            }, arg =>
            {
                DecorationType!.Value = arg.before;
                Style.DecorationType = arg.before;
                this.mainModel.NotifyTreeUpdated();
            }, (before: Style.DecorationType, after: v))).AddTo(Disposables);
            ShapeSize = new ReactiveProperty<int>(style.ShapeSize, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.mainModel.OperateAsUndoable(arg =>
            {
                ShapeSize!.Value = arg.after;
                Style.ShapeSize = arg.after;
                this.mainModel.NotifyTreeUpdated();
            }, arg =>
            {
                ShapeSize!.Value = arg.before;
                Style.ShapeSize = arg.before;
                this.mainModel.NotifyTreeUpdated();
            }, (before: Style.ShapeSize, after: v))).AddTo(Disposables);
            ShapeColor = new ReactiveProperty<string>(Style.ShapeColor, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.mainModel.OperateAsUndoable(arg =>
            {
                ShapeColor!.Value = arg.after;
                Style.ShapeColor = arg.after;
                this.mainModel.NotifyTreeUpdated();
            }, arg =>
            {
                ShapeColor!.Value = arg.before;
                Style.ShapeColor = arg.before;
                this.mainModel.NotifyTreeUpdated();
            }, (before: Style.ShapeColor, after: v))).AddTo(Disposables);
            Visible = new ReactiveProperty<bool>(Style.Enabled, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.mainModel.OperateAsUndoable(arg =>
            {
                Visible!.Value = arg.after;
                Style.Enabled = arg.after;
                this.mainModel.NotifyTreeUpdated();
            }, arg =>
            {
                Visible!.Value = arg.before;
                Style.Enabled = arg.before;
                this.mainModel.NotifyTreeUpdated();
            }, (before: Style.Enabled, after: v))).AddTo(Disposables);
        }

        /// <summary>
        /// 自身を削除します。
        /// </summary>
        public void DeleteSelf()
        {
            int index = mainModel.BranchDecorations.IndexOf(this);

            mainModel.OperateAsUndoable((arg, tree) =>
            {
                TreeStyle treeStyle = tree.Style;
                mainModel.BranchDecorations.RemoveAt(index);
                treeStyle.DecorationStyles = Array.FindAll(treeStyle.DecorationStyles, x => x != Style);

                mainModel.NotifyTreeUpdated();
            }, (arg, tree) =>
            {
                TreeStyle treeStyle = tree.Style;
                mainModel.BranchDecorations.Insert(arg, this);
                treeStyle.DecorationStyles = ArrayHelpers.Inserted(treeStyle.DecorationStyles, arg, Style);

                mainModel.NotifyTreeUpdated();
            }, index);
        }
    }
}
