using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        /// 読み込まれた系統樹一覧を取得します。
        /// </summary>
        private ReactiveCollection<Tree> Trees { get; }

        /// <summary>
        /// 対象の系統樹のインデックスのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> TreeIndex { get; }

        /// <summary>
        /// <see cref="TreeIndex"/>の最大値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> MaxTreeIndex { get; }

        /// <summary>
        /// 対象の樹形を取得します。
        /// </summary>
        public ReactivePropertySlim<Tree?> TargetTree { get; }

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

        /// <summary>
        /// 系統名を表示するかどうかを表す値のプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowLeaveLabels { get; }

        /// <summary>
        /// 系統名のフォントサイズのプロパティを取得します。
        /// </summary>
        public ReactivePropertySlim<int> LeaveLabelsFontSize { get; }

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

        /// <summary>
        /// <see cref="HomeViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HomeViewModel()
        {
            window = MainWindow.Instance;
            window.SetViewModel(this);

            Trees = new ReactiveCollection<Tree>().AddTo(Disposables);
            TreeIndex = new ReactivePropertySlim<int>(1).AddTo(Disposables);
            MaxTreeIndex = new ReactivePropertySlim<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = new ReactivePropertySlim<Tree?>().AddTo(Disposables);

            XScale = new ReactivePropertySlim<int>(300).AddTo(Disposables);
            YScale = new ReactivePropertySlim<int>(30).AddTo(Disposables);
            BranchThickness = new ReactivePropertySlim<int>(1).AddTo(Disposables);

            ShowLeaveLabels = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            LeaveLabelsFontSize = new ReactivePropertySlim<int>(20).AddTo(Disposables);

            ShowNodeValues = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            NodeValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).AddTo(Disposables);
            NodeValueFontSize = new ReactivePropertySlim<int>(15).AddTo(Disposables);

            ShowBranchValues = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            BranchValueType = new ReactivePropertySlim<CladeValueType>(CladeValueType.Supports).AddTo(Disposables);
            BranchValueFontSize = new ReactivePropertySlim<int>(15).AddTo(Disposables);

            ShowScaleBar = new ReactivePropertySlim<bool>(true).AddTo(Disposables);
            ScaleBarValue = new ReactivePropertySlim<double>(0.1).AddTo(Disposables);
            ScaleBarFontSize = new ReactivePropertySlim<int>(25).AddTo(Disposables);
            ScaleBarThickness = new ReactivePropertySlim<int>(5).AddTo(Disposables);

            TreeIndex.Subscribe(OnTreeIndexChanged);
        }

        private void OnTreeIndexChanged(int value)
        {
            value--;
            if ((uint)value >= (uint)Trees.Count) return;
            TargetTree.Value = Trees[value];
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
    }
}
