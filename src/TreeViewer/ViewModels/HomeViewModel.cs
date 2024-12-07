﻿using Reactive.Bindings;
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
        /// SVG要素のクリック時に実行されるコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<string> SvgElementClickedCommand { get; }

        /// <summary>
        /// 選択されているSVG要素のID一覧を取得します。
        /// </summary>
        public HashSet<string> FocusedSvgElementIdList { get; }

        #region Sidebar

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
            TreeIndex = new ReactivePropertySlim<int>(1).AddTo(Disposables);
            MaxTreeIndex = new ReactivePropertySlim<int>().AddTo(Disposables);
            Trees.ToCollectionChanged().Subscribe(x => MaxTreeIndex.Value = Trees.Count);
            TargetTree = new ReactivePropertySlim<Tree?>().AddTo(Disposables);
            SvgElementClickedCommand = new AsyncReactiveCommand<string>().WithSubscribe(OnSvgElementClicked)
                                                                         .AddTo(Disposables);
            FocusedSvgElementIdList = new HashSet<string>(StringComparer.Ordinal);

            XScale = new ReactivePropertySlim<int>(300).AddTo(Disposables);
            YScale = new ReactivePropertySlim<int>(30).AddTo(Disposables);
            BranchThickness = new ReactivePropertySlim<int>(1).AddTo(Disposables);

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

            TreeIndex.Subscribe(OnTreeIndexChanged);
        }

        /// <summary>
        /// <see cref="TreeIndex"/>が変更されたときに実行されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnTreeIndexChanged(int value)
        {
            value--;
            if ((uint)value >= (uint)Trees.Count) return;
            TargetTree.Value = Trees[value];
        }

        private async Task OnSvgElementClicked(string id)
        {
            await Console.Out.WriteLineAsync($"Clicked: {id}");

            FocusedSvgElementIdList.Clear();
            FocusedSvgElementIdList.Add(id);

            OnPropertyChanged(nameof(FocusedSvgElementIdList));
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
        public async Task ExportCurrentTree(string path, TreeFormat format)
        {
            Tree? tree = TargetTree.Value;
            if (tree is null) return;

            using var writer = new StreamWriter(path);
            await tree.WriteAsync(writer, format);
        }

        /// <summary>
        /// 枝の装飾を追加します。
        /// </summary>
        private async Task AddNewBranchDecoration()
        {
            BranchDecorations.AddOnScheduler(new BranchDecorationViewModel(this));
            await Task.CompletedTask;
        }
    }
}
