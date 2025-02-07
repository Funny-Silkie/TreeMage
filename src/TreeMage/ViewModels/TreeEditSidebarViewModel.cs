using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TreeMage.Core.Drawing;
using TreeMage.Core.Trees;
using TreeMage.Data;
using TreeMage.Models;
using TreeMage.Services;

namespace TreeMage.ViewModels
{
    /// <summary>
    /// ツリー編集サイドバーのViewModelのクラスです。
    /// </summary>
    public class TreeEditSidebarViewModel : ViewModelBase
    {
        private IElectronService electronService;
        private readonly MainModel model;

        #region Layout

        /// <inheritdoc cref="MainModel.CollapseType"/>
        public ReactivePropertySlim<CladeCollapseType> CollapseType { get; }

        /// <inheritdoc cref="MainModel.CollapsedConstantWidth"/>
        public ReactivePropertySlim<double> CollapsedConstantWidth { get; }

        /// <summary>
        /// 折り畳み処理のコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand CollapseCommand { get; }

        /// <inheritdoc cref="MainModel.CollapseClade"/>
        private async Task CollapseClade()
        {
            try
            {
                model.CollapseClade();
            }
            catch (Exception e)
            {
                await ErrorHandle.OutputErrorAsync(e, electronService);
            }
        }

        /// <summary>
        /// 並び替えを行うコマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand<bool> OrderByBranchLengthCommand { get; }

        /// <inheritdoc cref="MainModel.OrderByBranchLength(bool)"/>
        private async Task OrderByBranchLength(bool descending)
        {
            try
            {
                model.OrderByBranchLength(descending);
            }
            catch (Exception e)
            {
                await ErrorHandle.OutputErrorAsync(e, electronService);
            }
        }

        #endregion Layout

        #region Tree

        /// <inheritdoc cref="MainModel.XScale"/>
        public ReactivePropertySlim<int> XScale { get; }

        /// <inheritdoc cref="MainModel.YScale"/>
        public ReactivePropertySlim<int> YScale { get; }

        /// <inheritdoc cref="MainModel.BranchThickness"/>
        public ReactivePropertySlim<int> BranchThickness { get; }

        /// <inheritdoc cref="MainModel.DefaultBranchLength"/>
        public ReactivePropertySlim<double> DefaultBranchLength { get; }

        #endregion Tree

        #region Search

        /// <inheritdoc cref="MainModel.SearchQuery"/>
        public ReactivePropertySlim<string> SearchQuery { get; }

        /// <inheritdoc cref="MainModel.SearchTarget"/>
        public ReactivePropertySlim<TreeSearchTarget> SearchTarget { get; }

        /// <inheritdoc cref="MainModel.SearchOnIgnoreCase"/>
        public ReactivePropertySlim<bool> SearchOnIgnoreCase { get; }

        /// <inheritdoc cref="MainModel.SearchWithRegex"/>
        public ReactivePropertySlim<bool> SearchWithRegex { get; }

        /// <summary>
        /// 検索コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand SearchCommand { get; }

        #endregion Search

        #region LeafLabels

        /// <inheritdoc cref="MainModel.ShowLeafLabels"/>
        public ReactivePropertySlim<bool> ShowLeafLabels { get; }

        /// <inheritdoc cref="MainModel.LeafLabelsFontSize"/>
        public ReactivePropertySlim<int> LeafLabelsFontSize { get; }

        #endregion LeafLabels

        #region CladeLabels

        /// <inheritdoc cref="MainModel.ShowCladeLabels"/>
        public ReactivePropertySlim<bool> ShowCladeLabels { get; }

        /// <inheritdoc cref="MainModel.CladeLabelsFontSize"/>
        public ReactivePropertySlim<int> CladeLabelsFontSize { get; }

        /// <inheritdoc cref="MainModel.CladeLabelsLineThickness"/>
        public ReactivePropertySlim<int> CladeLabelsLineThickness { get; }

        #endregion CladeLabels

        #region NodeValues

        /// <inheritdoc cref="MainModel.ShowNodeValues"/>
        public ReactivePropertySlim<bool> ShowNodeValues { get; }

        /// <inheritdoc cref="MainModel.NodeValueType"/>
        public ReactivePropertySlim<CladeValueType> NodeValueType { get; }

        /// <inheritdoc cref="MainModel.NodeValueFontSize"/>
        public ReactivePropertySlim<int> NodeValueFontSize { get; }

        #endregion NodeValues

        #region BranchValues

        /// <inheritdoc cref="MainModel.ShowBranchValues"/>
        public ReactivePropertySlim<bool> ShowBranchValues { get; }

        /// <inheritdoc cref="MainModel.BranchValueType"/>
        public ReactivePropertySlim<CladeValueType> BranchValueType { get; }

        /// <inheritdoc cref="MainModel.BranchValueFontSize"/>
        public ReactivePropertySlim<int> BranchValueFontSize { get; }

        /// <inheritdoc cref="MainModel.BranchValueHideRegexPattern"/>
        public ReactivePropertySlim<string?> BranchValueHideRegexPattern { get; }

        #endregion BranchValues

        #region BranchDecorations

        /// <inheritdoc cref="MainModel.ShowBranchDecorations"/>
        public ReactivePropertySlim<bool> ShowBranchDecorations { get; }

        /// <inheritdoc cref="MainModel.BranchDecorations"/>
        public ReadOnlyReactiveCollection<BranchDecorationModel> BranchDecorations { get; }

        /// <summary>
        /// 装飾の追加コマンドを取得します。
        /// </summary>
        public AsyncReactiveCommand AddBranchDecorationCommand { get; }

        #endregion BranchDecorations

        #region Scalebar

        /// <inheritdoc cref="MainModel.ShowScaleBar"/>
        public ReactivePropertySlim<bool> ShowScaleBar { get; }

        /// <inheritdoc cref="MainModel.ScaleBarValue"/>
        public ReactivePropertySlim<double> ScaleBarValue { get; }

        /// <inheritdoc cref="MainModel.ScaleBarFontSize"/>
        public ReactivePropertySlim<int> ScaleBarFontSize { get; }

        /// <inheritdoc cref="MainModel.ScaleBarThickness"/>
        public ReactivePropertySlim<int> ScaleBarThickness { get; }

        #endregion Scalebar

        public TreeEditSidebarViewModel(MainModel model, IElectronService electronService)
        {
            this.electronService = electronService;
            this.model = model;
            model.PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);

            CollapseType = model.ToReactivePropertySlimAsSynchronized(x => x.CollapseType.Value)
                                .AddTo(Disposables);
            CollapsedConstantWidth = model.ToReactivePropertySlimAsSynchronized(x => x.CollapsedConstantWidth.Value)
                                          .AddTo(Disposables);
            CollapseCommand = new AsyncReactiveCommand().WithSubscribe(CollapseClade)
                                                        .AddTo(Disposables);
            OrderByBranchLengthCommand = new AsyncReactiveCommand<bool>().WithSubscribe(OrderByBranchLength)
                                                                         .AddTo(Disposables);

            XScale = model.ToReactivePropertySlimAsSynchronized(x => x.XScale.Value)
                          .AddTo(Disposables);
            YScale = model.ToReactivePropertySlimAsSynchronized(x => x.YScale.Value)
                          .AddTo(Disposables);
            BranchThickness = model.ToReactivePropertySlimAsSynchronized(x => x.BranchThickness.Value)
                                   .AddTo(Disposables);
            DefaultBranchLength = model.ToReactivePropertySlimAsSynchronized(x => x.DefaultBranchLength.Value)
                                       .AddTo(Disposables);

            SearchQuery = model.ToReactivePropertySlimAsSynchronized(x => x.SearchQuery.Value)
                               .AddTo(Disposables);
            SearchQuery.Subscribe(v => this.model.Search());
            SearchTarget = model.ToReactivePropertySlimAsSynchronized(x => x.SearchTarget.Value)
                                .AddTo(Disposables);
            SearchOnIgnoreCase = model.ToReactivePropertySlimAsSynchronized(x => x.SearchOnIgnoreCase.Value)
                                      .AddTo(Disposables);
            SearchWithRegex = model.ToReactivePropertySlimAsSynchronized(x => x.SearchWithRegex.Value)
                                   .AddTo(Disposables);
            SearchCommand = new AsyncReactiveCommand().WithSubscribe(model.Search)
                                                      .AddTo(Disposables);

            ShowLeafLabels = model.ToReactivePropertySlimAsSynchronized(x => x.ShowLeafLabels.Value)
                                  .AddTo(Disposables);
            LeafLabelsFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.LeafLabelsFontSize.Value)
                                      .AddTo(Disposables);

            ShowCladeLabels = model.ToReactivePropertySlimAsSynchronized(x => x.ShowCladeLabels.Value)
                                   .AddTo(Disposables);
            CladeLabelsFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.CladeLabelsFontSize.Value)
                                       .AddTo(Disposables);
            CladeLabelsLineThickness = model.ToReactivePropertySlimAsSynchronized(x => x.CladeLabelsLineThickness.Value)
                                            .AddTo(Disposables);

            ShowNodeValues = model.ToReactivePropertySlimAsSynchronized(x => x.ShowNodeValues.Value)
                                  .AddTo(Disposables);
            NodeValueType = model.ToReactivePropertySlimAsSynchronized(x => x.NodeValueType.Value)
                                 .AddTo(Disposables);
            NodeValueFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.NodeValueFontSize.Value)
                                     .AddTo(Disposables);

            ShowBranchValues = model.ToReactivePropertySlimAsSynchronized(x => x.ShowBranchValues.Value)
                                    .AddTo(Disposables);
            BranchValueType = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueType.Value)
                                   .AddTo(Disposables);
            BranchValueFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueFontSize.Value)
                                       .AddTo(Disposables);
            BranchValueHideRegexPattern = model.ToReactivePropertySlimAsSynchronized(x => x.BranchValueHideRegexPattern.Value)
                                               .AddTo(Disposables);

            ShowBranchDecorations = model.ToReactivePropertySlimAsSynchronized(x => x.ShowBranchDecorations.Value)
                                         .AddTo(Disposables);
            BranchDecorations = model.BranchDecorations.ToReadOnlyReactiveCollection()
                                                       .AddTo(Disposables);
            AddBranchDecorationCommand = new AsyncReactiveCommand().WithSubscribe(model.AddNewBranchDecoration)
                                                                   .AddTo(Disposables);

            ShowScaleBar = model.ToReactivePropertySlimAsSynchronized(x => x.ShowScaleBar.Value)
                                .AddTo(Disposables);
            ScaleBarValue = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarValue.Value)
                                 .AddTo(Disposables);
            ScaleBarFontSize = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarFontSize.Value)
                                    .AddTo(Disposables);
            ScaleBarThickness = model.ToReactivePropertySlimAsSynchronized(x => x.ScaleBarThickness.Value)
                                     .AddTo(Disposables);
        }
    }
}
