using Reactive.Bindings;
using TreeViewer.Core.Drawing.Styles;
using TreeViewer.Core.Trees;
using TreeViewer.TestUtilities;

namespace TreeViewer.Models
{
    public class BranchDecorationModelTest
    {
        private readonly BranchDecorationStyle style;
        private readonly Tree tree;
        private readonly MainModel mainModel;
        private readonly List<string?> updatedProperties = [];
        private readonly BranchDecorationModel decorationModel;

        public BranchDecorationModelTest()
        {
            tree = DummyData.CreateTree();
            mainModel = new MainModel();
            mainModel.Trees.Add(tree);
            mainModel.ApplyTreeStyle(tree);
            mainModel.TargetTree.Value = tree;
            mainModel.AddNewBranchDecoration();
            mainModel.AddNewBranchDecoration();
            mainModel.ClearUndoQueue();
            mainModel.PropertyChanged += (_, e) => updatedProperties.Add(e.PropertyName);

            decorationModel = mainModel.BranchDecorations[0];
            style = decorationModel.Style;
        }

        /// <summary>
        /// プロパティの設定を検証します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型</typeparam>
        /// <param name="property">プロパティ</param>
        /// <param name="value">設定する値</param>
        private async Task PropertySetTest<T>(ReactiveProperty<T> property, T value, Func<BranchDecorationStyle, T> stylePropertySelector)
        {
            T before = property.Value;
            property.Value = value;

            Assert.Multiple(() =>
            {
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(value, property.Value);
                Assert.Equal(value, stylePropertySelector.Invoke(style));
            });

            updatedProperties.Clear();
            bool undoSuccess = await mainModel.Undo();

            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(before, property.Value);
                Assert.Equal(before, stylePropertySelector.Invoke(style));
            });

            updatedProperties.Clear();
            bool redoSuccess = await mainModel.Redo();

            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Single(updatedProperties, "TargetTree");
                Assert.Equal(value, property.Value);
                Assert.Equal(value, stylePropertySelector.Invoke(style));
            });
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var style = new BranchDecorationStyle();
            var decorationModel = new BranchDecorationModel(mainModel, style);

            Assert.Multiple(() =>
            {
                Assert.Same(style, decorationModel.Style);
                Assert.Equal(style.RegexPattern, decorationModel.TargetRegexPattern.Value);
                Assert.Equal(style.DecorationType, decorationModel.DecorationType.Value);
                Assert.Equal(style.ShapeSize, decorationModel.ShapeSize.Value);
                Assert.Equal(style.ShapeColor, decorationModel.ShapeColor.Value);
                Assert.Equal(style.Enabled, decorationModel.Visible.Value);
            });
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public async Task TargetRegexPattern_Set()
        {
            await PropertySetTest(decorationModel.TargetRegexPattern, "hoge", x => x.RegexPattern);
        }

        [Fact]
        public async Task DecorationType_Set()
        {
            await PropertySetTest(decorationModel.DecorationType, BranchDecorationType.OpenedRectangle, x => x.DecorationType);
        }

        [Fact]
        public async Task ShapeSize_Set()
        {
            await PropertySetTest(decorationModel.ShapeSize, 10, x => x.ShapeSize);
        }

        [Fact]
        public async Task ShapeColor_Set()
        {
            await PropertySetTest(decorationModel.ShapeColor, "red", x => x.ShapeColor);
        }

        [Fact]
        public async Task Visible_Set()
        {
            await PropertySetTest(decorationModel.Visible, false, x => x.Enabled);
        }

        #endregion Properties

        #region Instance Methods

        [Fact]
        public async Task DeleteSelf()
        {
            BranchDecorationModel second = mainModel.BranchDecorations[1];
            decorationModel.DeleteSelf();

            Assert.Multiple(() =>
            {
                Assert.Single(mainModel.BranchDecorations, second);
                Assert.Single(tree.Style.DecorationStyles, second.Style);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool undoSuccess = await mainModel.Undo();
            Assert.Multiple(() =>
            {
                Assert.True(undoSuccess);
                Assert.Equal([decorationModel, second], mainModel.BranchDecorations);
                Assert.Equal([style, second.Style], tree.Style.DecorationStyles);
                Assert.Single(updatedProperties, "TargetTree");
            });

            updatedProperties.Clear();
            bool redoSuccess = await mainModel.Redo();
            Assert.Multiple(() =>
            {
                Assert.True(redoSuccess);
                Assert.Single(mainModel.BranchDecorations, second);
                Assert.Single(tree.Style.DecorationStyles, second.Style);
                Assert.Single(updatedProperties, "TargetTree");
            });
        }

        #endregion Instance Methods
    }
}
