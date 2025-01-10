using Reactive.Bindings;
using TreeViewer.Core.Trees;
using TreeViewer.Data;
using TreeViewer.TestUtilities;

namespace TreeViewer.Models
{
    public class StyleSidebarModelTest
    {
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade cladeBA;
        private readonly Tree tree;
        private readonly MainModel mainModel;
        private readonly StyleSidebarModel styleSidebarModel;
        private readonly List<string?> updatedProperties = [];

        public StyleSidebarModelTest()
        {
            tree = DummyData.CreateTree(out _, out leafA, out cladeB, out cladeBA, out _, out _, out _, out _, out _, out _, out _, out _);

            mainModel = new MainModel();
            mainModel.TargetTree.Value = tree;
            mainModel.Trees.Add(tree);
            mainModel.ApplyTreeStyle(tree);

            mainModel.PropertyChanged += (_, e) => updatedProperties.Add(e.PropertyName);
            styleSidebarModel = new StyleSidebarModel(mainModel);
        }

        /// <summary>
        /// プロパティの設定を検証します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型</typeparam>
        /// <param name="property">プロパティ</param>
        /// <param name="value">設定する値</param>
        private async Task PropertySetTest<T>(ReactiveProperty<T> property, T value, Action<ReactiveProperty<T>, T>? assertionOnValueSet = null, Action<ReactiveProperty<T>, T>? assertionOnUndo = null)
        {
            T before = property.Value;
            property.Value = value;

            Assert.Multiple(() =>
            {
                Assert.Equal(value, property.Value);
                assertionOnValueSet?.Invoke(property, value);
            });

            updatedProperties.Clear();
            bool undoSuccess = await mainModel.Undo();

            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Equal(before, property.Value);
                assertionOnUndo?.Invoke(property, before);
            });

            updatedProperties.Clear();
            bool redoSuccess = await mainModel.Redo();

            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Equal(value, property.Value);
                assertionOnValueSet?.Invoke(property, value);
            });
        }

        /// <summary>
        /// プロパティの設定を検証します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型</typeparam>
        /// <param name="property">プロパティ</param>
        /// <param name="value">設定する値</param>
        private async Task PropertySetTestAsOnlyTargetTreeUpdated<T>(ReactiveProperty<T> property, T value, Action<ReactiveProperty<T>, T>? assertionOnValueSet = null, Action<ReactiveProperty<T>, T>? assertionOnUndo = null)
        {
            T before = property.Value;
            property.Value = value;

            Assert.Multiple(() =>
            {
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(value, property.Value);
                assertionOnValueSet?.Invoke(property, value);
            });

            updatedProperties.Clear();
            bool undoSuccess = await mainModel.Undo();

            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(before, property.Value);
                assertionOnUndo?.Invoke(property, before);
            });

            updatedProperties.Clear();
            bool redoSuccess = await mainModel.Redo();

            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(value, property.Value);
                assertionOnValueSet?.Invoke(property, value);
            });
        }

        /// <summary>
        /// 無効な状態にします。
        /// </summary>
        private void SetupAsUnabled()
        {
            mainModel.EditMode.Value = TreeEditMode.Reroot;
            updatedProperties.Clear();
        }

        /// <summary>
        /// クレードが選択されていない状態にします。
        /// </summary>
        private void SetupAsNothingFocused()
        {
            mainModel.UnfocusAll();
            updatedProperties.Clear();
        }

        /// <summary>
        /// 一つの葉が選択されている状態にします。
        /// </summary>
        private void SetupAsLeafAFocused()
        {
            mainModel.Focus(leafA);
            updatedProperties.Clear();
        }

        /// <summary>
        /// 一つの内部枝が選択されている状態にします。
        /// </summary>
        private void SetupAsCladeBAFocused()
        {
            mainModel.Focus(cladeBA);
            updatedProperties.Clear();
        }

        /// <summary>
        /// 一つの葉と内部枝が選択されている状態にします。
        /// </summary>
        private void SetupAsLeafAAndCladeBAFocused()
        {
            mainModel.Focus(leafA, cladeBA);
            updatedProperties.Clear();
        }

        /// <summary>
        /// 複数の内部枝が選択されている状態にします。
        /// </summary>
        private void SetupAsMultipleBipartitionsFocused()
        {
            mainModel.Focus(cladeB, cladeBA);
            updatedProperties.Clear();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var styleSidebarModel = new StyleSidebarModel(mainModel);

            Assert.Multiple(() =>
            {
                Assert.True(styleSidebarModel.IsEnable.Value);
                Assert.Equal(SelectionMode.Node, styleSidebarModel.SelectionTarget.Value);
                Assert.Equal(0, styleSidebarModel.FocusedCount.Value);
                Assert.Null(styleSidebarModel.FirstSelectedElement.Value.Clade);
                Assert.Null(styleSidebarModel.CladeLabel.Value);
                Assert.Equal("black", styleSidebarModel.BranchColor.Value);
                Assert.Equal("black", styleSidebarModel.LeafColor.Value);
            });
        }

        #endregion Ctors

        #region Properties

        [Theory]
        [InlineData(TreeEditMode.Reroot)]
        [InlineData(TreeEditMode.Swap)]
        [InlineData(TreeEditMode.Subtree)]
        public void IsEnable_Get_AsFalse(TreeEditMode mode)
        {
            mainModel.EditMode.Value = mode;

            Assert.False(styleSidebarModel.IsEnable.Value);
        }

        [Fact]
        public void IsEnable_Get_AsTrue()
        {
            SetupAsUnabled();
            mainModel.EditMode.Value = TreeEditMode.Select;

            Assert.True(styleSidebarModel.IsEnable.Value);
        }

        [Theory]
        [InlineData(SelectionMode.Node)]
        [InlineData(SelectionMode.Clade)]
        [InlineData(SelectionMode.Taxa)]
        public void SelectionTarget_Get(SelectionMode mode)
        {
            mainModel.SelectionTarget.Value = mode;

            Assert.Equal(mode, styleSidebarModel.SelectionTarget.Value);
        }

        [Fact]
        public void Properties_Get_OnNothingSelected()
        {
            SetupAsNothingFocused();

            Assert.Multiple(() =>
            {
                Assert.Equal(0, styleSidebarModel.FocusedCount.Value);
                Assert.Null(styleSidebarModel.FirstSelectedElement.Value.Clade);
                Assert.False(styleSidebarModel.LeafSelected.Value);
                Assert.Equal("black", styleSidebarModel.BranchColor.Value);
            });
        }

        [Fact]
        public void Prpoerties_Get_OnSingleLeafSelected()
        {
            leafA.Style.CladeLabel = "Clade";
            leafA.Style.BranchColor = "red";
            leafA.Style.LeafColor = "red";
            SetupAsLeafAFocused();

            Assert.Multiple(() =>
            {
                Assert.Equal(1, styleSidebarModel.FocusedCount.Value);
                Assert.Equal(mainModel.FocusedSvgElementIdList.First(), styleSidebarModel.FirstSelectedElement.Value);
                Assert.True(styleSidebarModel.LeafSelected.Value);
                Assert.Same(leafA, styleSidebarModel.FirstSelectedElement.Value.Clade);
                Assert.Equal("red", styleSidebarModel.BranchColor.Value);
                Assert.Equal("red", styleSidebarModel.LeafColor.Value);
                Assert.Equal("Clade", styleSidebarModel.CladeLabel.Value);
            });
        }

        [Fact]
        public void Prpoerties_Get_OnSingleBipartitionSelected()
        {
            cladeBA.Style.CladeLabel = "CLADE";
            SetupAsCladeBAFocused();

            Assert.Multiple(() =>
            {
                Assert.Equal(1, styleSidebarModel.FocusedCount.Value);
                Assert.Equal(mainModel.FocusedSvgElementIdList.First(), styleSidebarModel.FirstSelectedElement.Value);
                Assert.False(styleSidebarModel.LeafSelected.Value);
                Assert.Same(cladeBA, styleSidebarModel.FirstSelectedElement.Value.Clade);
                Assert.Equal("black", styleSidebarModel.BranchColor.Value);
                Assert.Equal("CLADE", styleSidebarModel.CladeLabel.Value);
            });
        }

        [Fact]
        public void Properties_Get_OnLeafAndBipartitionsFocused()
        {
            cladeBA.Style.BranchColor = "red";
            leafA.Style.LeafColor = "red";
            SetupAsLeafAAndCladeBAFocused();

            Assert.Multiple(() =>
            {
                Assert.Equal(2, styleSidebarModel.FocusedCount.Value);
                Assert.Equal(mainModel.FocusedSvgElementIdList.First(), styleSidebarModel.FirstSelectedElement.Value);
                Assert.True(styleSidebarModel.LeafSelected.Value);
                Assert.Null(styleSidebarModel.BranchColor.Value);
                Assert.Equal("red", styleSidebarModel.LeafColor.Value);
            });
        }

        [Fact]
        public void Properties_Get_OnMultipleBipartitionsFocused()
        {
            cladeBA.Style.BranchColor = "red";
            SetupAsMultipleBipartitionsFocused();

            Assert.Multiple(() =>
            {
                Assert.Equal(2, styleSidebarModel.FocusedCount.Value);
                Assert.Equal(mainModel.FocusedSvgElementIdList.First(), styleSidebarModel.FirstSelectedElement.Value);
                Assert.False(styleSidebarModel.LeafSelected.Value);
                Assert.Null(styleSidebarModel.BranchColor.Value);
            });
        }

        [Fact]
        public async Task BranchColor_Set_OnSingleSelected()
        {
            SetupAsLeafAFocused();

            await PropertySetTestAsOnlyTargetTreeUpdated(styleSidebarModel.BranchColor,
                                                         "red",
                                                         (_, v) => Assert.Equal(v, leafA.Style.BranchColor),
                                                         (_, v) => Assert.Equal(v, leafA.Style.BranchColor));
        }

        [Fact]
        public async Task BranchColor_Set_OnMultipleSelected()
        {
            SetupAsMultipleBipartitionsFocused();

            await PropertySetTestAsOnlyTargetTreeUpdated(styleSidebarModel.BranchColor,
                                                         "red",
                                                         (_, v) => Assert.Multiple(() =>
                                                         {
                                                             Assert.Equal(v, cladeB.Style.BranchColor);
                                                             Assert.Equal(v, cladeBA.Style.BranchColor);
                                                         }),
                                                         (_, v) => Assert.Multiple(() =>
                                                         {
                                                             Assert.Equal(v, cladeB.Style.BranchColor);
                                                             Assert.Equal(v, cladeBA.Style.BranchColor);
                                                         }));
        }

        [Fact]
        public async Task BranchColor_Set_OnSingleLeafAndBipartitionSelected()
        {
            SetupAsLeafAAndCladeBAFocused();

            await PropertySetTestAsOnlyTargetTreeUpdated(styleSidebarModel.LeafColor,
                                                         "red",
                                                         (_, v) => Assert.Multiple(() =>
                                                         {
                                                             Assert.Equal(v, leafA.Style.LeafColor);
                                                             Assert.Equal("black", cladeBA.Style.LeafColor);
                                                         }),
                                                         (_, v) => Assert.Multiple(() =>
                                                         {
                                                             Assert.Equal(v, leafA.Style.LeafColor);
                                                             Assert.Equal("black", cladeBA.Style.LeafColor);
                                                         }));
        }

        [Fact]
        public async Task BranchCladeLabel_Set_OnSingleSelected()
        {
            SetupAsCladeBAFocused();

            await PropertySetTestAsOnlyTargetTreeUpdated(styleSidebarModel.CladeLabel,
                                                         "hoge",
                                                         (_, v) => Assert.Equal(v, cladeBA.Style.CladeLabel),
                                                         (_, v) => Assert.Equal(v, cladeBA.Style.CladeLabel));
        }

        #endregion Properties
    }
}
