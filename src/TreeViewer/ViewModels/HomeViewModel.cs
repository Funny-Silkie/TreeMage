using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Text.RegularExpressions;
using TreeViewer.Core.Exporting;
using TreeViewer.Core.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.Core.Trees.Parsers;
using TreeViewer.Data;
using TreeViewer.Window;

namespace TreeViewer.ViewModels
{
    /// <summary>
    /// ホーム画面のViewModelのクラスです。
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        private readonly MainWindow window;

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
        public AsyncReactiveCommand<string> SvgElementClickedCommand { get; }

        /// <summary>
        /// エクスポートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<(string path, ExportType type)> ExportWithExporterCommand { get; }

        #region Focus

        /// <summary>
        /// 選択されているSVG要素のID一覧を取得します。
        /// </summary>
        public HashSet<string> FocusedSvgElementIdList { get; }

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
        /// 選択モードを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<SelectionMode> SelectionTarget { get; }

        #endregion Topbar

        #region Sidebar

        #region Layout

        /// <summary>
        /// リルートを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand RerootCommand { get; }

        /// <summary>
        /// 並び替えを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand OrderByBranchLengthCommand { get; }

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

        #region LeafLavels

        /// <summary>
        /// 系統名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowLeafLabels { get; }

        /// <summary>
        /// 系統名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> LeafLabelsFontSize { get; }

        #endregion LeafLavels

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
        public HomeViewModel()
        {
            window = MainWindow.Instance;
            window.SetViewModel(this);

            Trees = new ReactiveCollection<Tree>().AddTo(Disposables);
            TreeIndex = new ReactivePropertySlim<int>(1).WithSubscribe(OnTreeIndexChanged)
                                                        .AddTo(Disposables);
            MaxTreeIndex = new ReactivePropertySlim<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = new ReactivePropertySlim<Tree?>().AddTo(Disposables);
            SvgElementClickedCommand = new AsyncReactiveCommand<string>().WithSubscribe(OnSvgElementClicked)
                                                                         .AddTo(Disposables);

            ExportWithExporterCommand = new AsyncReactiveCommand<(string path, ExportType type)>().WithSubscribe(async x => await ExportWithExporter(x.path, x.type))
                                                                                                  .AddTo(Disposables);

            FocusedSvgElementIdList = new HashSet<string>(StringComparer.Ordinal);
            FocusAllCommand = new AsyncReactiveCommand().WithSubscribe(FocusAll)
                                                          .AddTo(Disposables);
            UnfocusAllCommand = new AsyncReactiveCommand().WithSubscribe(UnfocusAll)
                                                          .AddTo(Disposables);

            SelectionTarget = new ReactivePropertySlim<SelectionMode>(SelectionMode.Node).WithSubscribe(OnSelectionTargetChanged)
                                                                                         .AddTo(Disposables);

            RerootCommand = new AsyncReactiveCommand().WithSubscribe(Reroot)
                                                      .AddTo(Disposables);
            OrderByBranchLengthCommand = new AsyncReactiveCommand().WithSubscribe(OrderByBranchLength)
                                                                   .AddTo(Disposables);

            XScale = new ReactivePropertySlim<int>(300).AddTo(Disposables);
            YScale = new ReactivePropertySlim<int>(30).AddTo(Disposables);
            BranchThickness = new ReactivePropertySlim<int>(1).AddTo(Disposables);

            SearchQuery = new ReactivePropertySlim<string>(string.Empty).AddTo(Disposables);
            SearchTarget = new ReactivePropertySlim<TreeSearchTarget>().AddTo(Disposables);
            SearchOnIgnoreCase = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            SearchWithRegex = new ReactivePropertySlim<bool>().AddTo(Disposables);
            SearchCommand = new AsyncReactiveCommand().WithSubscribe(Search)
                                                      .AddTo(Disposables);

            ShowLeafLabels = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            LeafLabelsFontSize = new ReactivePropertySlim<int>(20).AddTo(Disposables);

            ShowNodeValues = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            NodeValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).AddTo(Disposables);
            NodeValueFontSize = new ReactivePropertySlim<int>(15).AddTo(Disposables);

            ShowBranchValues = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            BranchValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).AddTo(Disposables);
            BranchValueFontSize = new ReactivePropertySlim<int>(15).AddTo(Disposables);

            ShowBranchDecorations = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            BranchDecorations = new ReactiveCollection<BranchDecorationViewModel>().AddTo(Disposables);
            AddBranchDecorationCommand = new AsyncReactiveCommand().WithSubscribe(AddNewBranchDecoration)
                                                                   .AddTo(Disposables);

            ShowScaleBar = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            ScaleBarValue = new ReactivePropertySlim<double>(0.1).AddTo(Disposables);
            ScaleBarFontSize = new ReactivePropertySlim<int>(25).AddTo(Disposables);
            ScaleBarThickness = new ReactivePropertySlim<int>(5).AddTo(Disposables);

            StyleSidebarViewModel = new StyleSidebarViewModel(this);
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
            tree.Style.ShowNodeValues = ShowNodeValues.Value;
            tree.Style.NodeValueType = NodeValueType.Value;
            tree.Style.NodeValueFontSize = NodeValueFontSize.Value;
            tree.Style.ShowBranchValues = ShowBranchValues.Value;
            tree.Style.BranchValueType = BranchValueType.Value;
            tree.Style.BranchValueFontSize = BranchValueFontSize.Value;
            tree.Style.ShowBranchDecorations = ShowBranchDecorations.Value;
            tree.Style.DecorationStyles = BranchDecorations.Where(x => x.Regex is not null && x.Visible.Value).Select(x => new BranchDecorationStyle()
            {
                Regex = x.Regex!,
                ShapeSize = x.ShapeSize.Value,
                ShapeColor = x.ShapeColor.Value,
                DecorationType = x.DecorationType.Value,
            }).ToArray();
            tree.Style.ShowScaleBar = ShowScaleBar.Value;
            tree.Style.ScaleBarValue = ScaleBarValue.Value;
            tree.Style.ScaleBarFontSize = ScaleBarFontSize.Value;
            tree.Style.ScaleBarThickness = ScaleBarThickness.Value;
        }

        /// <summary>
        /// <see cref="TreeIndex"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnTreeIndexChanged(int value)
        {
            value--;
            if ((uint)value >= (uint)Trees.Count) return;

            UnfocusAll();
            TargetTree.Value = Trees[value];
        }

        /// <summary>
        /// SVG要素がクリックされた際に実行されます。
        /// </summary>
        /// <param name="id">SVG要素のID</param>
        private void OnSvgElementClicked(string id)
        {
            switch (SelectionTarget.Value)
            {
                case SelectionMode.Node:
                    {
                        Focus(CladeIdManager.FromId(id));
                    }
                    break;

                case SelectionMode.Clade:
                    {
                        Clade target = CladeIdManager.FromId(id);
                        Focus(target.GetDescendants().Prepend(target));
                    }
                    break;

                case SelectionMode.Taxa:
                    {
                        Clade target = CladeIdManager.FromId(id);
                        if (target.IsLeaf) Focus(target);
                        else Focus(target.GetDescendants().Where(x => x.IsLeaf));
                    }
                    break;
            }

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
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
                string? idSuffix = SelectionTarget.Value switch
                {
                    SelectionMode.Node or SelectionMode.Clade => "branch",
                    SelectionMode.Taxa => "leaf",
                    _ => null,
                };
                if (idSuffix is null) continue;

                FocusedSvgElementIdList.Add(current.GetId(idSuffix));
            }

            StyleSidebarViewModel.Update();
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

            StyleSidebarViewModel.Update();
            OnPropertyChanged(nameof(FocusedSvgElementIdList));
        }

        /// <summary>
        /// <see cref="SelectionTarget"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnSelectionTargetChanged(SelectionMode value)
        {
            if (FocusedSvgElementIdList.Count == 0) return;

            HashSet<Clade> selectedClades = FocusedSvgElementIdList.Select(CladeIdManager.FromId).ToHashSet();

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
        private void Reroot()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null || SelectionTarget.Value != SelectionMode.Node || FocusedSvgElementIdList.Count != 1) return;
            string focusedElement = FocusedSvgElementIdList.First();

            if (!focusedElement.EndsWith("-branch")) return;

            Clade clade = CladeIdManager.FromId(focusedElement);
            if (clade.IsLeaf || clade.Tree != tree) return;

            tree.Reroot(clade);
            TargetTree.Value = tree.Clone();

            OnPropertyChanged(nameof(TargetTree));
            UnfocusAll();
        }

        /// <summary>
        /// 枝長で並び替えます。
        /// </summary>
        private void OrderByBranchLength()
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            Tree cloned = tree.Clone();
            cloned.OrderByLength();

            TargetTree.Value = cloned;

            UnfocusAll();
            OnPropertyChanged(nameof(TargetTree));
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
        /// 系統樹を読み込みます。
        /// </summary>
        /// <param name="path">読み込む系統樹ファイルのパス</param>
        /// <param name="format">読み込む系統樹のフォーマット</param>
        public async Task ImportTree(string path, TreeFormat format)
        {
            using var reader = new StreamReader(path);
            Tree[] trees = await Tree.ReadAsync(reader, format);
            if (trees.Length == 0) return;

            Trees.AddRangeOnScheduler(trees);

            if (Trees.Count == 0)
            {
                TargetTree.Value = trees[0];
                OnPropertyChanged(nameof(TargetTree));
            }
            else OnPropertyChanged(nameof(MaxTreeIndex));
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
            await exporter.ExportAsync(tree, stream, new ExportOptions());
        }

        /// <summary>
        /// 枝の装飾を追加します。
        /// </summary>
        private void AddNewBranchDecoration()
        {
            BranchDecorations.AddOnScheduler(new BranchDecorationViewModel(this));
        }

        /// <summary>
        /// 色の更新を行います。
        /// </summary>
        /// <param name="color">更新する色</param>
        public void SetColorToFocusedObject(string color)
        {
            foreach (Clade currentClade in FocusedSvgElementIdList.Select(CladeIdManager.FromId))
                switch (SelectionTarget.Value)
                {
                    case SelectionMode.Node:
                    case SelectionMode.Clade:
                        currentClade.Style.BranchColor = color;
                        break;

                    case SelectionMode.Taxa:
                        currentClade.Style.LeafColor = color;
                        break;
                }

            OnPropertyChanged(nameof(TargetTree));
        }
    }
}
