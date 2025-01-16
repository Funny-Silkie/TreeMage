using TreeMage.Core.Drawing;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Trees;

namespace TreeMage.TestUtilities
{
    /// <summary>
    /// ダミーデータを管理します。
    /// </summary>
    public static class DummyData
    {
        /// <inheritdoc cref="CreateTree(out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade, out Clade)"/>
        public static Tree CreateTree()
        {
            return CreateTree(out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        }

        /// <summary>
        /// ダミーの系統樹を生成します。
        /// </summary>
        /// <param name="root">根を表す<see cref="Clade"/>インスタンス</param>
        /// <param name="leafA"></param>
        /// <param name="cladeB"></param>
        /// <param name="cladeBA"></param>
        /// <param name="leafBAA"></param>
        /// <param name="leafBAB"></param>
        /// <param name="cladeBB"></param>
        /// <param name="cladeBBA"></param>
        /// <param name="leafBBAA"></param>
        /// <param name="leafBBAB"></param>
        /// <param name="leafBBB"></param>
        /// <param name="leafC"></param>
        /// <returns>ダミーの系統樹</returns>
        /// <remarks>
        /// <code>
        ///          2                              5
        ///      +------leafA    1         +---------------leafBAA
        /// root-|             +---cladeBA-|20/30
        ///      |             |           +---------leafBAB
        ///      |   2         |                 3            2
        ///      +------cladeB-|30/45           1          +------leafBBAA
        ///      |             |   2          +---cladeBBA-|85/95
        ///      |             +------cladeBB-|100/100     +---leafBBAB
        ///      | 1                          |    3         1
        ///      +---leafC                    +---------leafBBB
        /// </code>
        /// </remarks>
        public static Tree CreateTree(out Clade root,
                                           out Clade leafA,
                                           out Clade cladeB,
                                           out Clade cladeBA,
                                           out Clade leafBAA,
                                           out Clade leafBAB,
                                           out Clade cladeBB,
                                           out Clade cladeBBA,
                                           out Clade leafBBAA,
                                           out Clade leafBBAB,
                                           out Clade leafBBB,
                                           out Clade leafC)
        {
            root = new Clade();

            leafA = new Clade()
            {
                Taxon = "A",
                BranchLength = 2,
                Parent = root,
            };
            cladeB = new Clade()
            {
                Supports = "30/45",
                BranchLength = 2,
                Parent = root,
            };
            leafC = new Clade()
            {
                Taxon = "C",
                BranchLength = 1,
                Parent = root,
            };
            root.ChildrenInternal.AddRange(leafA, cladeB, leafC);

            cladeBA = new Clade()
            {
                Supports = "20/30",
                BranchLength = 1,
                Parent = cladeB,
            };
            cladeBB = new Clade()
            {
                BranchLength = 2,
                Supports = "100/100",
                Parent = cladeB,
            };
            cladeB.ChildrenInternal.AddRange(cladeBA, cladeBB);

            leafBAA = new Clade()
            {
                Taxon = "BAA",
                BranchLength = 5,
                Parent = cladeBA,
            };
            leafBAB = new Clade()
            {
                Taxon = "BAB",
                BranchLength = 3,
                Parent = cladeBA,
            };
            cladeBA.ChildrenInternal.AddRange(leafBAA, leafBAB);

            cladeBBA = new Clade()
            {
                BranchLength = 1,
                Supports = "85/95",
                Parent = cladeBB,
            };
            leafBBB = new Clade()
            {
                Taxon = "BBB",
                BranchLength = 3,
                Parent = cladeBB,
            };
            cladeBB.ChildrenInternal.AddRange(cladeBBA, leafBBB);

            leafBBAA = new Clade()
            {
                Taxon = "BBAA",
                BranchLength = 2,
                Parent = cladeBBA,
            };
            leafBBAB = new Clade()
            {
                Taxon = "BBAB",
                BranchLength = 1,
                Parent = cladeBBA,
            };
            cladeBBA.ChildrenInternal.AddRange(leafBBAA, leafBBAB);

            return new Tree(root);
        }

        /// <summary>
        /// ダミーのツリースタイルを生成します。
        /// </summary>
        /// <returns>ダミーのツリースタイル</returns>
        public static TreeStyle CreateTreeStyle()
        {
            return new TreeStyle()
            {
                XScale = 10,
                YScale = 10,
                BranchThickness = 10,
                DefaultBranchLength = 1,
                ShowLeafLabels = false,
                LeafLabelsFontSize = 1,
                ShowCladeLabels = false,
                CladeLabelsFontSize = 1,
                ShowNodeValues = false,
                CladeLabelsLineThickness = 1,
                NodeValueFontSize = 1,
                NodeValueType = CladeValueType.BranchLength,
                ShowBranchValues = false,
                BranchValueFontSize = 1,
                BranchValueType = CladeValueType.Supports,
                BranchValueHideRegexPattern = "100/100",
                ShowBranchDecorations = false,
                DecorationStyles = [
                    new BranchDecorationStyle()
                    {
                        RegexPattern = "100/100",
                        ShapeSize = 1,
                        DecorationType = BranchDecorationType.OpenCircle,
                        ShapeColor = "red",
                    }],
                ShowScaleBar = false,
                ScaleBarValue = 3,
                ScaleBarFontSize = 50,
                ScaleBarThickness = 10,
                CollapseType = CladeCollapseType.Constant,
                CollapsedConstantWidth = 2,
            };
        }
    }
}
