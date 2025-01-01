using ElectronNET.API.Entities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TreeViewer.Core.Drawing;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.ProjectData;
using TreeViewer.Core.Trees;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.Data;
using TreeViewer.Services;
using TreeViewer.Settings;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : WindowViewModel<MainWindow, HomeViewModel>
    {
        private string? projectPath;
        private bool onUndoOperation = false;
        private readonly UndoService undoService = new UndoService();

        /// <summary>
        /// スタイル編集用のViewModelを取得します。
        /// </summary>
        public StyleSidebarViewModel StyleSidebarViewModel { get; }

        /// <summary>
        /// 読み込まれた系統樹一覧を取得します。
        /// </summary>
        private ReactiveCollection<Tree> Trees { get; }

        /// <summary>
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> MaxTreeIndex { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactivePropertySlim<Tree?> TargetTree { get; }

        /// <summary>
        /// SVG要素のクリック時に実行されるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<CladeId> SvgElementClickedCommand { get; }

        /// <summary>
        /// ツリーを強制的に再描画するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand RerenderTreeCommand { get; }

        /// <summary>
        /// rerootを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(Clade target, bool asRooted)> RerootCommand { get; }

        /// <summary>
        /// 姉妹同士の交換を行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(Clade target1, Clade target2)> SwapSisterCommand { get; }

        /// <summary>
        /// サブツリーの抽出を行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<Clade> ExtractSubtreeCommand { get; }

        #region Menu

        /// <summary>
        /// プロジェクトを新規作成するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CreateNewCommand { get; }

        /// <summary>
        /// プロジェクトファイルを開くコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand OpenProjectCommand { get; }

        /// <summary>
        /// プロジェクトファイルを保存するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> SaveProjectCommand { get; }

        /// <summary>
        /// エクスポートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(string path, ExportType type)> ExportWithExporterCommand { get; }

        /// <summary>
        /// undoを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand UndoCommand { get; }

        /// <summary>
        /// redoを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand RedoCommand { get; }

        #endregion Menu

        #region Focus

        /// <summary>
        /// 選択されているSVG要素のID一覧を取得します。
        /// </summary>
        public HashSet<CladeId> FocusedSvgElementIdList { get; }

        /// <summary>
        /// 全てを選択するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand FocusAllCommand { get; }

        /// <summary>
        /// 全選択を解除するコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand UnfocusAllCommand { get; }

        #endregion Focus

        #region Topbar

        /// <summary>
        /// 対象の系統樹のインデックスのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> TreeIndex { get; }

        /// <summary>
        /// 編集モードのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<TreeEditMode> EditMode { get; }

        /// <summary>
        /// 選択モードを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        #endregion Topbar

        #region Sidebar

        #region Layout

        /// <summary>
        /// 折り畳みのタイプのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<CladeCollapseType> CollapseType { get; }

        /// <summary>
        /// 折りたたまれた三角形の幅のプロパティを取得します。
        /// </summary>
        /// <remarks><see cref="CollapseType"/>が<see cref="CladeCollapseType.Constant"/>の際に使用される</remarks>
        public ReactivePropertySlim<double> CollapsedConstantWidth { get; }

        /// <summary>
        /// 折り畳み処理のコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CollapseCommand { get; }

        /// <summary>
        /// 並び替えを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> OrderByBranchLengthCommand { get; }

        #endregion Layout

        #region Tree

        /// <summary>
        /// X軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> XScale { get; }

        /// <summary>
        /// Y軸方向の拡大率のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> YScale { get; }

        /// <summary>
        /// 枝の太さのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> BranchThickness { get; }

        #endregion Tree

        #region Search

        /// <summary>
        /// 検索ワードのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string> SearchQuery { get; }

        /// <summary>
        /// 検索対象を表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<TreeSearchTarget> SearchTarget { get; }

        /// <summary>
        /// 大文字・小文字を無視して検索を行うかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> SearchOnIgnoreCase { get; }

        /// <summary>
        /// 検索に正規表現を使うかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> SearchWithRegex { get; }

        /// <summary>
        /// 検索コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand SearchCommand { get; }

        #endregion Search

        #region LeafLabels

        /// <summary>
        /// 系統名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowLeafLabels { get; }

        /// <summary>
        /// 系統名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> LeafLabelsFontSize { get; }

        #endregion LeafLabels

        #region CladeLabels

        /// <summary>
        /// クレード名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowCladeLabels { get; }

        /// <summary>
        /// クレード名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> CladeLabelsFontSize { get; }

        /// <summary>
        /// クレード名脇の線幅のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> CladeLabelsLineThickness { get; }

        #endregion CladeLabels

        #region NodeValues

        /// <summary>
        /// 結節点の値を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowNodeValues { get; }

        /// <summary>
        /// 結節点の値の種類のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<CladeValueType> NodeValueType { get; }

        /// <summary>
        /// 結節点の値のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> NodeValueFontSize { get; }

        #endregion NodeValues

        #region BranchValues

        /// <summary>
        /// 二分岐の値を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowBranchValues { get; }

        /// <summary>
        /// 二分岐の値の種類のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<CladeValueType> BranchValueType { get; }

        /// <summary>
        /// 二分岐の値のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> BranchValueFontSize { get; }

        /// <summary>
        /// 非表示にする枝の値の正規表現パターンのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<string?> BranchValueHideRegexPattern { get; }

        #endregion BranchValues

        #region BranchDecorations

        /// <summary>
        /// 二分岐の装飾を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowBranchDecorations { get; }

        /// <summary>
        /// 枝の装飾情報一覧を取得します。
        /// </summary>
        public ReactiveCollection<BranchDecorationViewModel> BranchDecorations { get; }

        /// <summary>
        /// 装飾の追加コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand AddBranchDecorationCommand { get; }

        #endregion BranchDecorations

        #region Scalebar

        /// <summary>
        /// スケールバーを表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowScaleBar { get; }

        /// <summary>
        /// スケールバーの数値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<double> ScaleBarValue { get; }

        /// <summary>
        /// スケールバーのフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> ScaleBarFontSize { get; }

        /// <summary>
        /// スケールバーの太さのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> ScaleBarThickness { get; }

        #endregion Scalebar

        #endregion Sidebar

        /// <summary>
        /// <see cref="HomeViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HomeViewModel() : base(MainWindow.Instance)
        {
            Trees = new ReactiveCollection<Tree>().AddTo(Disposables);
            TreeIndex = new ReactivePropertySlim<int>(1).WithSubscribe(OnTreeIndexChanged)
                                                        .AddTo(Disposables);
            EditMode = new ReactivePropertySlim<TreeEditMode>().AddTo(Disposables);
            EditMode.Zip(EditMode.Skip(1), (x, y) => (before: x, after: y)).Subscribe(v => OperateAsUndoable((arg, tree) =>
            {
                EditMode.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                EditMode.Value = arg.before;

                RequestRerenderTree();
            }, v));
            MaxTreeIndex = new ReactivePropertySlim<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = new ReactivePropertySlim<Tree?>().AddTo(Disposables);
            SvgElementClickedCommand = new AsyncReactiveCommand<CladeId>().WithSubscribe(OnSvgElementClicked)
                                                                          .AddTo(Disposables);
            RerenderTreeCommand = new AsyncReactiveCommand().WithSubscribe(RequestRerenderTree).AddTo(Disposables);
            RerootCommand = new AsyncReactiveCommand<(Clade target, bool asRooted)>().WithSubscribe(x => Reroot(x.target, x.asRooted))
                                                                                     .AddTo(Disposables);
            SwapSisterCommand = new AsyncReactiveCommand<(Clade target1, Clade target2)>().WithSubscribe(x => SwapSisters(x.target1, x.target2))
                                                                                          .AddTo(Disposables);
            ExtractSubtreeCommand = new AsyncReactiveCommand<Clade>().WithSubscribe(ExtractSubtree)
                                                                     .AddTo(Disposables);

            CreateNewCommand = new AsyncReactiveCommand().WithSubscribe(CreateNew)
                                                         .AddTo(Disposables);
            OpenProjectCommand = new AsyncReactiveCommand().WithSubscribe(OpenProject)
                                                           .AddTo(Disposables);
            SaveProjectCommand = new AsyncReactiveCommand<bool>().WithSubscribe(SaveProject)
                                                                 .AddTo(Disposables);
            ExportWithExporterCommand = new AsyncReactiveCommand<(string path, ExportType type)>().WithSubscribe(async x => await ExportWithExporter(x.path, x.type))
                                                                                                  .AddTo(Disposables);
            UndoCommand = new AsyncReactiveCommand().WithSubscribe(undoService.Undo)
                                                    .AddTo(Disposables);
            RedoCommand = new AsyncReactiveCommand().WithSubscribe(undoService.Redo)
                                                    .AddTo(Disposables);

            FocusedSvgElementIdList = [];
            FocusAllCommand = new AsyncReactiveCommand().WithSubscribe(FocusAll)
                                                          .AddTo(Disposables);
            UnfocusAllCommand = new AsyncReactiveCommand().WithSubscribe(UnfocusAll)
                                                          .AddTo(Disposables);

            SelectionTarget = new ReactivePropertySlim<SelectionMode>(SelectionMode.Node).WithSubscribe(OnSelectionTargetChanged)
                                                                                         .AddTo(Disposables);

            CollapseType = new ReactivePropertySlim<CladeCollapseType>(CladeCollapseType.TopMax).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapseType = arg.after;
                CollapseType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CollapseType = arg.before;
                CollapseType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CollapseType ?? CladeCollapseType.TopMax, after: v))).AddTo(Disposables);
            CollapsedConstantWidth = new ReactivePropertySlim<double>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.after;
                CollapsedConstantWidth!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CollapsedConstantWidth = arg.before;
                CollapsedConstantWidth!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CollapsedConstantWidth ?? 1, after: v))).AddTo(Disposables);
            CollapseCommand = new AsyncReactiveCommand().WithSubscribe(CollapseClade)
                                                        .AddTo(Disposables);
            OrderByBranchLengthCommand = new AsyncReactiveCommand<bool>().WithSubscribe(OrderByBranchLength)
                                                                         .AddTo(Disposables);

            XScale = new ReactivePropertySlim<int>(300).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.XScale = arg.after;
                XScale!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.XScale = arg.before;
                XScale!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.XScale ?? 0, after: v))).AddTo(Disposables);
            YScale = new ReactivePropertySlim<int>(30).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.YScale = arg.after;
                YScale!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.YScale = arg.before;
                YScale!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.YScale ?? 0, after: v))).AddTo(Disposables);
            BranchThickness = new ReactivePropertySlim<int>(1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchThickness = arg.after;
                BranchThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchThickness = arg.before;
                BranchThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchThickness ?? 0, after: v))).AddTo(Disposables);

            SearchQuery = new ReactivePropertySlim<string>(string.Empty).AddTo(Disposables);
            SearchTarget = new ReactivePropertySlim<TreeSearchTarget>().AddTo(Disposables);
            SearchOnIgnoreCase = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            SearchWithRegex = new ReactivePropertySlim<bool>().AddTo(Disposables);
            SearchCommand = new AsyncReactiveCommand().WithSubscribe(Search)
                                                      .AddTo(Disposables);

            ShowLeafLabels = new ReactivePropertySlim<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.after;
                ShowLeafLabels!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowLeafLabels = arg.before;
                ShowLeafLabels!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowLeafLabels ?? false, after: v))).AddTo(Disposables);
            LeafLabelsFontSize = new ReactivePropertySlim<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.after;
                LeafLabelsFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.LeafLabelsFontSize = arg.before;
                LeafLabelsFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.LeafLabelsFontSize ?? 0, after: v))).AddTo(Disposables);

            ShowCladeLabels = new ReactivePropertySlim<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.after;
                ShowCladeLabels!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowCladeLabels = arg.before;
                ShowCladeLabels!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowCladeLabels ?? false, after: v))).AddTo(Disposables);
            CladeLabelsFontSize = new ReactivePropertySlim<int>(20).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.after;
                CladeLabelsFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelsFontSize = arg.before;
                CladeLabelsFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CladeLabelsFontSize ?? 0, after: v))).AddTo(Disposables);
            CladeLabelsLineThickness = new ReactivePropertySlim<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.after;
                CladeLabelsLineThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.CladeLabelLineThickness = arg.before;
                CladeLabelsLineThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.CladeLabelLineThickness ?? 0, after: v)));

            ShowNodeValues = new ReactivePropertySlim<bool>(false).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.after;
                ShowNodeValues!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowNodeValues = arg.before;
                ShowNodeValues!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowNodeValues ?? false, after: v))).AddTo(Disposables);
            NodeValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueType = arg.after;
                NodeValueType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueType = arg.before;
                NodeValueType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.NodeValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            NodeValueFontSize = new ReactivePropertySlim<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.after;
                NodeValueFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.NodeValueFontSize = arg.before;
                NodeValueFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.NodeValueFontSize ?? 0, after: v))).AddTo(Disposables);

            ShowBranchValues = new ReactivePropertySlim<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.after;
                ShowBranchValues!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchValues = arg.before;
                ShowBranchValues!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowBranchValues ?? false, after: v))).AddTo(Disposables);
            BranchValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueType = arg.after;
                BranchValueType!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueType = arg.before;
                BranchValueType!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueType ?? CladeValueType.BranchLength, after: v))).AddTo(Disposables);
            BranchValueFontSize = new ReactivePropertySlim<int>(15).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.after;
                BranchValueFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueFontSize = arg.before;
                BranchValueFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueFontSize ?? 0, after: v))).AddTo(Disposables);
            BranchValueHideRegexPattern = new ReactivePropertySlim<string?>().WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.after;
                BranchValueHideRegexPattern!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.BranchValueHideRegexPattern = arg.before;
                BranchValueHideRegexPattern!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.BranchValueHideRegexPattern, after: v))).AddTo(Disposables);

            ShowBranchDecorations = new ReactivePropertySlim<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.after;
                ShowBranchDecorations!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowBranchDecorations = arg.before;
                ShowBranchDecorations!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowBranchDecorations ?? false, after: v))).AddTo(Disposables);
            BranchDecorations = new ReactiveCollection<BranchDecorationViewModel>().AddTo(Disposables);
            AddBranchDecorationCommand = new AsyncReactiveCommand().WithSubscribe(AddNewBranchDecoration)
                                                                   .AddTo(Disposables);

            ShowScaleBar = new ReactivePropertySlim<bool>(true).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.after;
                ShowScaleBar!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ShowScaleBar = arg.before;
                ShowScaleBar!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ShowScaleBar ?? false, after: v))).AddTo(Disposables);
            ScaleBarValue = new ReactivePropertySlim<double>(0.1).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.after;
                ScaleBarValue!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarValue = arg.before;
                ScaleBarValue!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarValue ?? 0.1, after: v))).AddTo(Disposables);
            ScaleBarFontSize = new ReactivePropertySlim<int>(25).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.after;
                ScaleBarFontSize!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarFontSize = arg.before;
                ScaleBarFontSize!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarFontSize ?? 0, after: v))).AddTo(Disposables);
            ScaleBarThickness = new ReactivePropertySlim<int>(5).WithSubscribe(v => OperateAsUndoable((arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.after;
                ScaleBarThickness!.Value = arg.after;

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.Style.ScaleBarThickness = arg.before;
                ScaleBarThickness!.Value = arg.before;

                RequestRerenderTree();
            }, (before: TargetTree.Value?.Style?.ScaleBarThickness ?? 0, after: v))).AddTo(Disposables);

            StyleSidebarViewModel = new StyleSidebarViewModel(this);

            undoService.Clear();
        }

        /// <summary>
        /// 系統樹の再描画をトリガーします。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RequestRerenderTree()
        {
            OnPropertyChanged(nameof(TargetTree));
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T> operation, Action<T> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            onUndoOperation = true;

            async Task AsyncOperation(T x)
            {
                onUndoOperation = true;

                operation.Invoke(x);

                onUndoOperation = false;
                await Task.CompletedTask;
            }
            async Task AsyncUndoOperation(T x)
            {
                onUndoOperation = true;

                undoOperation.Invoke(x);

                onUndoOperation = false;
                await Task.CompletedTask;
            }

            try
            {
                operation.Invoke(argument);

                undoService.AddOperation(AsyncUndoOperation, AsyncOperation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public void OperateAsUndoable<T>(Action<T, Tree> operation, Action<T, Tree> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            onUndoOperation = true;

            async Task AsyncOperation(T x)
            {
                onUndoOperation = true;

                operation.Invoke(x, tree);

                onUndoOperation = false;
                await Task.CompletedTask;
            }
            async Task AsyncUndoOperation(T x)
            {
                onUndoOperation = true;

                undoOperation.Invoke(x, tree);

                onUndoOperation = false;
                await Task.CompletedTask;
            }

            try
            {
                operation.Invoke(argument, tree);

                undoService.AddOperation(AsyncUndoOperation, AsyncOperation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// undo/redo可能な処理を実行し，<see cref="undoService"/>に登録します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="operation">処理</param>
        /// <param name="undoOperation">undo処理</param>
        /// <param name="argument">引数</param>
        public async Task OperateAsUndoable<T>(Func<T, Tree, Task> operation, Func<T, Tree, Task> undoOperation, T argument)
        {
            if (onUndoOperation) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            onUndoOperation = true;

            async Task Operation(T argument)
            {
                onUndoOperation = true;

                await operation.Invoke(argument, tree);

                onUndoOperation = false;
            }

            async Task UndoOperation(T argument)
            {
                onUndoOperation = true;

                await undoOperation.Invoke(argument, tree);

                onUndoOperation = false;
            }

            try
            {
                await operation.Invoke(argument, tree);

                undoService.AddOperation(UndoOperation, Operation, argument);
            }
            finally
            {
                onUndoOperation = false;
            }
        }

        /// <summary>
        /// 対象の系統樹にスタイル情報を適用します。
        /// </summary>
        /// <param name="tree">適用する対象</param>
        public void ApplyTreeStyle(Tree tree)
        {
            tree.Style.XScale = XScale.Value;
            tree.Style.YScale = YScale.Value;
            tree.Style.BranchThickness = BranchThickness.Value;
            tree.Style.ShowLeafLabels = ShowLeafLabels.Value;
            tree.Style.LeafLabelsFontSize = LeafLabelsFontSize.Value;
            tree.Style.ShowCladeLabels = ShowCladeLabels.Value;
            tree.Style.CladeLabelsFontSize = CladeLabelsFontSize.Value;
            tree.Style.CladeLabelLineThickness = CladeLabelsLineThickness.Value;
            tree.Style.ShowNodeValues = ShowNodeValues.Value;
            tree.Style.NodeValueType = NodeValueType.Value;
            tree.Style.NodeValueFontSize = NodeValueFontSize.Value;
            tree.Style.ShowBranchValues = ShowBranchValues.Value;
            tree.Style.BranchValueType = BranchValueType.Value;
            tree.Style.BranchValueFontSize = BranchValueFontSize.Value;
            tree.Style.BranchValueHideRegexPattern = BranchValueHideRegexPattern.Value;
            tree.Style.ShowBranchDecorations = ShowBranchDecorations.Value;
            tree.Style.DecorationStyles = BranchDecorations.Select(x => x.Style).ToArray();
            tree.Style.ShowScaleBar = ShowScaleBar.Value;
            tree.Style.ScaleBarValue = ScaleBarValue.Value;
            tree.Style.ScaleBarFontSize = ScaleBarFontSize.Value;
            tree.Style.ScaleBarThickness = ScaleBarThickness.Value;
            tree.Style.CollapseType = CollapseType.Value;
            tree.Style.CollapsedConstantWidth = CollapsedConstantWidth.Value;
        }

        /// <summary>
        /// スタイル情報を読み取ります。
        /// </summary>
        /// <param name="tree">読み取るツリー</param>
        private void LoadTreeStyle(Tree tree)
        {
            XScale.Value = tree.Style.XScale;
            YScale.Value = tree.Style.YScale;
            BranchThickness.Value = tree.Style.BranchThickness;
            ShowLeafLabels.Value = tree.Style.ShowLeafLabels;
            LeafLabelsFontSize.Value = tree.Style.LeafLabelsFontSize;
            ShowCladeLabels.Value = tree.Style.ShowCladeLabels;
            CladeLabelsFontSize.Value = tree.Style.CladeLabelsFontSize;
            CladeLabelsLineThickness.Value = tree.Style.CladeLabelLineThickness;
            ShowNodeValues.Value = tree.Style.ShowNodeValues;
            NodeValueType.Value = tree.Style.NodeValueType;
            NodeValueFontSize.Value = tree.Style.NodeValueFontSize;
            ShowBranchValues.Value = tree.Style.ShowBranchValues;
            BranchValueType.Value = tree.Style.BranchValueType;
            BranchValueFontSize.Value = tree.Style.BranchValueFontSize;
            BranchValueHideRegexPattern.Value = tree.Style.BranchValueHideRegexPattern;
            ShowBranchDecorations.Value = tree.Style.ShowBranchDecorations;
            BranchDecorations.ClearOnScheduler();
            BranchDecorations.AddRangeOnScheduler(tree.Style.DecorationStyles.Select(x => new BranchDecorationViewModel(this, x)));
            ShowScaleBar.Value = tree.Style.ShowScaleBar;
            ScaleBarValue.Value = tree.Style.ScaleBarValue;
            ScaleBarFontSize.Value = tree.Style.ScaleBarFontSize;
            ScaleBarThickness.Value = tree.Style.ScaleBarThickness;
            CollapseType.Value = tree.Style.CollapseType;
            CollapsedConstantWidth.Value = tree.Style.CollapsedConstantWidth;
        }

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
                    RequestRerenderTree();
                }, arg =>
                {
                    ApplyTreeStyle(arg.nextTree);

                    UnfocusAll();
                    TargetTree.Value = arg.prevTree;

                    LoadTreeStyle(arg.prevTree);

                    TreeIndex.Value = arg.prevIndex + 1;
                    RequestRerenderTree();
                }, (prevTree, nextTree, prevIndex, nextIndex: value));
            }
        }

        /// <summary>
        /// SVG要素がクリックされた際に実行されます。
        /// </summary>
        /// <param name="id">SVG要素のID</param>
        private void OnSvgElementClicked(CladeId id)
        {
            try
            {
                if (EditMode.Value is TreeEditMode.Select)
                {
                    switch (SelectionTarget.Value)
                    {
                        case SelectionMode.Node:
                            {
                                Focus(id.Clade);
                            }
                            break;

                        case SelectionMode.Clade:
                            {
                                Clade target = id.Clade;
                                Focus(target.GetDescendants().Prepend(target));
                            }
                            break;

                        case SelectionMode.Taxa:
                            {
                                Clade target = id.Clade;
                                if (target.IsLeaf) Focus(target);
                                else Focus(target.GetDescendants().Where(x => x.IsLeaf));
                            }
                            break;
                    }

                    OnPropertyChanged(nameof(FocusedSvgElementIdList));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Window.ShowErrorMessageAsync(e).Wait();
            }
        }

        /// <summary>
        /// 指定したクレードを選択します。
        /// </summary>
        /// <param name="targetClades">選択するクレード</param>
        private void Focus(params IEnumerable<Clade> targetClades)
        {
            FocusedSvgElementIdList.Clear();

            foreach (Clade current in targetClades)
            {
                CladeIdSuffix idSuffix;
                switch (SelectionTarget.Value)
                {
                    case SelectionMode.Node:
                    case SelectionMode.Clade:
                        idSuffix = CladeIdSuffix.Branch;
                        break;

                    case SelectionMode.Taxa:
                        idSuffix = CladeIdSuffix.Leaf;
                        break;

                    default: continue;
                }

                FocusedSvgElementIdList.Add(current.GetId(idSuffix));
            }

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// 全ての要素を選択します。
        /// </summary>
        private void FocusAll()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Focus(tree.GetAllClades());
        }

        /// <summary>
        /// 全ての選択を解除します。
        /// </summary>
        private void UnfocusAll()
        {
            FocusedSvgElementIdList.Clear();

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

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

        /// <summary>
        /// リルートを行います。
        /// </summary>
        /// <param name="clade">対象クレード</param>
        /// <param name="asRooted">Rootedな系統樹として処理するかどうかを表す値</param>
        private void Reroot(Clade clade, bool asRooted)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null || clade.IsLeaf || clade.Tree != tree) return;

            Tree rerooted = tree.Rerooted(clade, asRooted);
            ApplyTreeStyle(rerooted);
            int targetIndex = TreeIndex.Value - 1;

            OperateAsUndoable(arg =>
            {
                TargetTree.Value = arg.rerooted;
                Trees[arg.targetIndex] = arg.rerooted;

                UnfocusAll();
                RequestRerenderTree();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                RequestRerenderTree();
            }, (prev: tree, rerooted, targetIndex));
        }

        /// <summary>
        /// 姉妹の入れ替えを行います。
        /// </summary>
        /// <param name="target1">選択したクレード1</param>
        /// <param name="target2">選択したクレード2</param>
        private void SwapSisters(Clade target1, Clade target2)
        {
            if (target1.IsRoot || target2.IsRoot || target1 == target2) return;

            OperateAsUndoable((arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                tree.SwapSisters(arg.target1, arg.target2);

                RequestRerenderTree();
            }, (target1, target2));
        }

        /// <summary>
        /// サブツリーの抽出を行います。
        /// </summary>
        /// <param name="clade">対象のクレード</param>
        private void ExtractSubtree(Clade clade)
        {
            if (clade.IsRoot || clade.IsLeaf) return;

            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            var subtree = new Tree(clade.Clone(true));
            ApplyTreeStyle(tree);

            OperateAsUndoable((arg, tree) =>
            {
                Trees.InsertOnScheduler(arg.prevIndex + 1, arg.subtree);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 2;
                TargetTree.Value = arg.subtree;
                RequestRerenderTree();

                EditMode.Value = TreeEditMode.Select;
                OnPropertyChanged(nameof(EditMode));
            }, (arg, tree) =>
            {
                Trees.RemoveAtOnScheduler(arg.prevIndex + 1);
                OnPropertyChanged(nameof(MaxTreeIndex));

                TreeIndex.Value = arg.prevIndex + 1;
                TargetTree.Value = tree;
                RequestRerenderTree();

                EditMode.Value = TreeEditMode.Subtree;
                OnPropertyChanged(nameof(EditMode));
            }, (subtree, prevIndex: TreeIndex.Value - 1));
        }

        /// <summary>
        /// 折り畳みを行います。
        /// </summary>
        private void CollapseClade()
        {
            if (FocusedSvgElementIdList.Count != 1 || SelectionTarget.Value is not SelectionMode.Node) return;

            try
            {
                CladeId id = FocusedSvgElementIdList.First();
                if (id.Suffix != CladeIdSuffix.Branch) return;

                Clade clade = id.Clade;
                if (clade.IsLeaf || clade.IsRoot) return;

                bool prevValue = clade.Style.Collapsed;

                OperateAsUndoable((arg, tree) =>
                {
                    arg.clade.Style.Collapsed = !prevValue;

                    RequestRerenderTree();
                }, (arg, tree) =>
                {
                    arg.clade.Style.Collapsed = prevValue;

                    RequestRerenderTree();
                }, (clade, prevValue));
            }
            catch (Exception e)
            {
                Window.ShowErrorMessageAsync(e).Wait();
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 枝長で並び替えます。
        /// </summary>
        /// <param name="descending">降順ソートかどうかを表す値</param>
        private void OrderByBranchLength(bool descending)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Tree oredered = tree.Clone();
            oredered.OrderByLength(descending);
            ApplyTreeStyle(oredered);

            int targetIndex = TreeIndex.Value - 1;

            OperateAsUndoable(arg =>
            {
                TargetTree.Value = arg.next;
                Trees[arg.targetIndex] = arg.next;

                UnfocusAll();
                RequestRerenderTree();
            }, arg =>
            {
                TargetTree.Value = arg.prev;
                Trees[arg.targetIndex] = arg.prev;

                UnfocusAll();
                RequestRerenderTree();
            }, (prev: tree, next: oredered, targetIndex));
        }

        /// <summary>
        /// 検索を実行します。
        /// </summary>
        private void Search()
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

        /// <summary>
        /// プロジェクトファイルを新規作成します。
        /// </summary>
        private void CreateNew()
        {
            projectPath = null;
            UnfocusAll();

            TargetTree.Value = null;
            Trees.ClearOnScheduler();
            TreeIndex.Value = 1;
            undoService.Clear();

            RequestRerenderTree();
            OnPropertyChanged(nameof(Trees));
        }

        /// <summary>
        /// プロジェクトファイルを開きます。
        /// </summary>
        private async Task OpenProject()
        {
            string? path = await Window.ShowSingleFileOpenDialog([new FileFilter()
            {
                Name = "Tree viewer project file",
                Extensions = ["treeprj"],
            }]);
            if (path is null) return;

            projectPath = path;

            try
            {
                ProjectData data = await ProjectData.LoadAsync(path);

                UnfocusAll();
                Trees.ClearOnScheduler();
                Trees.AddRangeOnScheduler(data.Trees);

                TargetTree.Value = null;

                TreeIndex.Value = 1;
                if (data.Trees.Length > 0)
                {
                    Tree mainTree = data.Trees[0];
                    TargetTree.Value = mainTree;
                    LoadTreeStyle(mainTree);
                }

                undoService.Clear();

                RequestRerenderTree();
                OnPropertyChanged(nameof(Trees));
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
                projectPath = null;
            }
        }

        /// <summary>
        /// プロジェクトファイルを保存します。
        /// </summary>
        /// <param name="asNew">新しいファイルとして保存するかどうかを表す値</param>
        private async Task SaveProject(bool asNew)
        {
            if (asNew || projectPath is null)
            {
                string? selectedPath = await Window.ShowFileSaveDialog([new FileFilter()
                {
                    Name = "Tree viewer project file",
                    Extensions = ["treeprj"],
                }]);

                if (selectedPath is null) return;
                projectPath = selectedPath;
            }

            if (TargetTree.Value is not null) ApplyTreeStyle(TargetTree.Value);

            var projectData = new ProjectData()
            {
                Trees = [.. Trees],
            };

            try
            {
                await projectData.SaveAsync(projectPath);
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
            }
        }

        /// <summary>
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="path">読み込む系統樹ファイルのパス</param>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        public async Task ImportTree(string path, TreeFormat format)
        {
            try
            {
                using var reader = new StreamReader(path);
                Tree[] trees;
                try
                {
                    trees = await Tree.ReadAsync(reader, format);
                }
                catch (TreeFormatException e)
                {
                    await Window.ShowErrorMessageAsync("ツリーのフォーマットが無効です");
                    await Console.Out.WriteLineAsync(e.ToString());
                    return;
                }

                if (trees.Length == 0) return;
                Configurations config = await Configurations.LoadOrCreateAsync();

                for (int i = 0; i < trees.Length; i++)
                {
                    Tree tree = trees[i];

                    ApplyTreeStyle(tree);
                    switch (config.AutoOrderingMode)
                    {
                        case AutoOrderingMode.Ascending:
                            tree.OrderByLength(false);
                            break;

                        case AutoOrderingMode.Descending:
                            tree.OrderByLength(true);
                            break;
                    }
                }

                if (Trees.Count == 0)
                {
                    Trees.AddRangeOnScheduler(trees);

                    TargetTree.Value = trees[0];
                    RequestRerenderTree();
                }
                else
                    OperateAsUndoable(arg =>
                    {
                        Trees.AddRangeOnScheduler(arg.trees);
                        OnPropertyChanged(nameof(MaxTreeIndex));

                        TargetTree.Value = trees[0];
                        TreeIndex.Value = arg.addedAt + 1;
                        RequestRerenderTree();
                    }, arg =>
                    {
                        for (int i = arg.addedAt; i < arg.addedAt + arg.trees.Length; i++) Trees.RemoveAtOnScheduler(i);
                        OnPropertyChanged(nameof(MaxTreeIndex));

                        TargetTree.Value = Trees[arg.prevIndex];
                        TreeIndex.Value = arg.prevIndex + 1;
                        RequestRerenderTree();
                    }, (trees, addedAt: Trees.Count, prevIndex: TreeIndex.Value - 1));
            }
            catch (Exception e)
            {
                await Window.ShowErrorMessageAsync(e);
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        /// <summary>
        /// 表示している系統樹を出力します。
        /// </summary>
        /// <param name="path">出力する系統樹ファイルのパス</param>
        /// <param name="format">出力する系統樹のフォーマット</param>
        public async Task ExportCurrentTreeAsTreeFile(string path, TreeFormat format)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            using var writer = new StreamWriter(path);
            await tree.WriteAsync(writer, format);
        }

        /// <summary>
        /// <see cref="IExporter"/>によるエクスポートを行います。
        /// </summary>
        /// <param name="path">出力先のファイルパス</param>
        /// <param name="type">エクスポートのフォーマット</param>
        private async Task ExportWithExporter(string path, ExportType type)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            ApplyTreeStyle(tree);

            IExporter exporter = IExporter.Create(type);
            using var stream = new FileStream(path, FileMode.Create);
            await exporter.ExportAsync(tree, stream, (await Configurations.LoadOrCreateAsync()).ToExportOptions());
        }

        /// <summary>
        /// 枝の装飾を追加します。
        /// </summary>
        private void AddNewBranchDecoration()
        {
            var style = new BranchDecorationStyle();
            var decorationViewModel = new BranchDecorationViewModel(this, style);

            OperateAsUndoable((arg, tree) =>
            {
                BranchDecorations.AddOnScheduler(arg.vm);
                tree.Style.DecorationStyles = [.. tree.Style.DecorationStyles, arg.style];

                RequestRerenderTree();
            }, (arg, tree) =>
            {
                BranchDecorations.RemoveOnScheduler(arg.vm);
                tree.Style.DecorationStyles = tree.Style.DecorationStyles[..^1];

                RequestRerenderTree();
            }, (vm: decorationViewModel, style));
        }
    }
}
