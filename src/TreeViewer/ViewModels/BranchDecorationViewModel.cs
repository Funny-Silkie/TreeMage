using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeViewer.Core.Drawing.Styles;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// 枝の装飾UIのViewModelを表します。
    /// </summary>
    public class BranchDecorationViewModel : ViewModelBase
    {
        private readonly HomeViewModel homeViewModel;

        /// <summary>
        /// 対象のスタイルを取得します。
        /// </summary>
        public BranchDecorationStyle Style { get; }

        /// <summary>
        /// 対象の正規表現のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string?> TargetRegexPattern { get; }

        /// <summary>
        /// 装飾の種類のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<BranchDecorationType> DecorationType { get; }

        /// <summary>
        /// 図形のサイズを表すプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> ShapeSize { get; }

        /// <summary>
        /// 図形の色を表すプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string> ShapeColor { get; }

        /// <summary>
        /// 表示するどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> Visible { get; }

        /// <summary>
        /// 自身を削除するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand DeleteSelfCommand { get; }

        /// <summary>
        /// <see cref="BranchDecorationViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="homeViewModel">親となる<see cref="HomeViewModel"/>のインスタンス</param>
        /// <param name="style">対象のスタイル</param>
        public BranchDecorationViewModel(HomeViewModel homeViewModel, BranchDecorationStyle style)
        {
            this.homeViewModel = homeViewModel;
            Style = style;

            TargetRegexPattern = new ReactivePropertySlim<string?>(Style.RegexPattern, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.homeViewModel.OperateAsUndoable(arg =>
            {
                TargetRegexPattern!.Value = arg.after;
                Style.RegexPattern = arg.after;
                RequestRerenderTree();
            }, arg =>
            {
                TargetRegexPattern!.Value = arg.before;
                Style.RegexPattern = arg.before;
                RequestRerenderTree();
            }, (before: Style.RegexPattern, after: v))).AddTo(Disposables);
            DecorationType = new ReactivePropertySlim<BranchDecorationType>(Style.DecorationType, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.homeViewModel.OperateAsUndoable(arg =>
            {
                DecorationType!.Value = arg.after;
                Style.DecorationType = arg.after;
                RequestRerenderTree();
            }, arg =>
            {
                DecorationType!.Value = arg.before;
                Style.DecorationType = arg.before;
                RequestRerenderTree();
            }, (before: Style.DecorationType, after: v))).AddTo(Disposables);
            ShapeSize = new ReactivePropertySlim<int>(style.ShapeSize, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.homeViewModel.OperateAsUndoable(arg =>
            {
                ShapeSize!.Value = arg.after;
                Style.ShapeSize = arg.after;
                RequestRerenderTree();
            }, arg =>
            {
                ShapeSize!.Value = arg.before;
                Style.ShapeSize = arg.before;
                RequestRerenderTree();
            }, (before: Style.ShapeSize, after: v))).AddTo(Disposables);
            ShapeColor = new ReactivePropertySlim<string>(Style.ShapeColor, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.homeViewModel.OperateAsUndoable(arg =>
            {
                ShapeColor!.Value = arg.after;
                Style.ShapeColor = arg.after;
                RequestRerenderTree();
            }, arg =>
            {
                ShapeColor!.Value = arg.before;
                Style.ShapeColor = arg.before;
                RequestRerenderTree();
            }, (before: Style.ShapeColor, after: v))).AddTo(Disposables);
            Visible = new ReactivePropertySlim<bool>(Style.Enabled, mode: ReactivePropertyMode.DistinctUntilChanged).WithSubscribe(v => this.homeViewModel.OperateAsUndoable(arg =>
            {
                Visible!.Value = arg.after;
                Style.Enabled = arg.after;
                RequestRerenderTree();
            }, arg =>
            {
                Visible!.Value = arg.before;
                Style.Enabled = arg.before;
                RequestRerenderTree();
            }, (before: Style.Enabled, after: v))).AddTo(Disposables);

            DeleteSelfCommand = new AsyncReactiveCommand().WithSubscribe(DeleteSelf)
                                                          .AddTo(Disposables);
        }

        /// <summary>
        /// 自身を削除します。
        /// </summary>
        private void DeleteSelf()
        {
            int index = homeViewModel.BranchDecorations.IndexOf(this);

            homeViewModel.OperateAsUndoable((arg, tree) =>
            {
                TreeStyle treeStyle = tree.Style;
                homeViewModel.BranchDecorations.RemoveAtOnScheduler(index);
                treeStyle.DecorationStyles = Array.FindAll(treeStyle.DecorationStyles, x => x != Style);

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                TreeStyle treeStyle = tree.Style;
                homeViewModel.BranchDecorations.InsertOnScheduler(arg, this);
                treeStyle.DecorationStyles = ArrayHelpers.Inserted(treeStyle.DecorationStyles, arg, Style);

                RequestRerenderTree();
            }, index);
        }

        /// <summary>
        /// 系統樹の再描画をリクエストします。
        /// </summary>
        private void RequestRerenderTree() => homeViewModel.RerenderTreeCommand.Execute();
    }
}
