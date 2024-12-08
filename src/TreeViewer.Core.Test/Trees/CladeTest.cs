using TreeViewer.Core.Styles;

namespace TreeViewer.Core.Trees
{
    public class CladeTest
    {
        private readonly Clade root;
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade cladeBA;
        private readonly Clade leafBAA;
        private readonly Clade leafBAB;
        private readonly Clade cladeBB;
        private readonly Clade cladeBBA;
        private readonly Clade leafBBAA;
        private readonly Clade leafBBAB;
        private readonly Clade leafBBB;
        private readonly Clade leafC;

        public CladeTest()
        {
            TreeTest.CreateDummyTree(out root, out leafA, out cladeB, out cladeBA, out leafBAA, out leafBAB, out cladeBB, out cladeBBA, out leafBBAA, out leafBBAB, out leafBBB, out leafC);
            root.TreeInternal = null;
        }

        private Clade[] GetAllClades() => [root, leafA, cladeB, cladeBA, leafBAA, leafBAB, cladeBB, cladeBBA, leafBBAA, leafBBAB, leafBBB, leafC];

        /// <summary>
        /// 二つの<see cref="Clade"/>の等価性を検証します。
        /// </summary>
        /// <param name="expected">予期される値</param>
        /// <param name="actual">実際の値</param>
        /// <remarks>親要素の比較は行いません</remarks>
        internal static void CompareClades(Clade expected, Clade actual)
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.Taxon, actual.Taxon);
                Assert.Equal(expected.Supports, actual.Supports);
                Assert.Equal(expected.BranchLength, actual.BranchLength);
                Assert.Equal(expected.ChildrenInternal.Count, actual.ChildrenInternal.Count);
                CladeStyleTest.CompareStyles(expected.Style, actual.Style);
            });

            for (int i = 0; i < expected.ChildrenInternal.Count; i++) CompareClades(expected.ChildrenInternal[i], actual.ChildrenInternal[i]);
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var clade = new Clade();

            Assert.Multiple(() =>
            {
                Assert.Null(clade.Taxon);
                Assert.Null(clade.Supports);
                Assert.Equal(double.NaN, clade.BranchLength);
                Assert.Null(clade.Parent);
                Assert.Empty(clade.ChildrenInternal);
                Assert.Null(clade.TreeInternal);
                CladeStyleTest.CompareStyles(new CladeStyle(), clade.Style);
            });
        }

        #endregion Ctors

        #region Properties

        [Fact]
        public void IsRoot_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.True(root.IsRoot);
                Assert.False(leafA.IsRoot);
                Assert.False(cladeB.IsRoot);
                Assert.False(cladeBA.IsRoot);
                Assert.False(leafBAA.IsRoot);
                Assert.False(leafBAB.IsRoot);
                Assert.False(cladeBB.IsRoot);
                Assert.False(cladeBBA.IsRoot);
                Assert.False(leafBBAA.IsRoot);
                Assert.False(leafBBAB.IsRoot);
                Assert.False(leafBBB.IsRoot);
                Assert.False(leafC.IsRoot);
            });
        }

        [Fact]
        public void IsLeaf_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.False(root.IsLeaf);
                Assert.True(leafA.IsLeaf);
                Assert.False(cladeB.IsLeaf);
                Assert.False(cladeBA.IsLeaf);
                Assert.True(leafBAA.IsLeaf);
                Assert.True(leafBAB.IsLeaf);
                Assert.False(cladeBB.IsLeaf);
                Assert.False(cladeBBA.IsLeaf);
                Assert.True(leafBBAB.IsLeaf);
                Assert.True(leafBBAA.IsLeaf);
                Assert.True(leafBBB.IsLeaf);
                Assert.True(leafC.IsLeaf);
            });
        }

        [Fact]
        public void Children_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(root.ChildrenInternal, root.Children);
                Assert.Equal(leafA.ChildrenInternal, leafA.Children);
                Assert.Equal(cladeB.ChildrenInternal, cladeB.Children);
                Assert.Equal(cladeBA.ChildrenInternal, cladeBA.Children);
                Assert.Equal(leafBAA.ChildrenInternal, leafBAA.Children);
                Assert.Equal(leafBAB.ChildrenInternal, leafBAB.Children);
                Assert.Equal(cladeBB.ChildrenInternal, cladeBB.Children);
                Assert.Equal(cladeBBA.ChildrenInternal, cladeBBA.Children);
                Assert.Equal(leafBBAA.ChildrenInternal, leafBBAA.Children);
                Assert.Equal(leafBBAB.ChildrenInternal, leafBBAB.Children);
                Assert.Equal(leafBBB.ChildrenInternal, leafBBB.Children);
                Assert.Equal(leafC.ChildrenInternal, leafC.Children);
            });
        }

        [Fact]
        public void Tree_Get_OnDefault()
        {
            Assert.Multiple(() =>
            {
                Assert.Null(root.Tree);
                Assert.Null(leafA.Tree);
                Assert.Null(cladeB.Tree);
                Assert.Null(cladeBA.Tree);
                Assert.Null(leafBAA.Tree);
                Assert.Null(leafBAB.Tree);
                Assert.Null(cladeBB.Tree);
                Assert.Null(cladeBBA.Tree);
                Assert.Null(leafBBAA.Tree);
                Assert.Null(leafBBAB.Tree);
                Assert.Null(leafBBB.Tree);
                Assert.Null(leafC.Tree);
            });
        }

        #endregion Properties

        #region Methods

        [Fact]
        public void Clone_WithBoolAsOnlyDescendants()
        {
            _ = new Tree(root);

            foreach (Clade current in GetAllClades())
            {
                Clade cloned = current.Clone(true);

                Assert.Null(cloned.TreeInternal);
                Assert.Null(cloned.Parent);
                CompareClades(current, cloned);
            }
        }

        [Fact]
        public void Clone_WithBoolAsCloneAll()
        {
            _ = new Tree(root);
            Array.ForEach(GetAllClades(), x => x.Style.BranchColor = "red");

            foreach (Clade current in GetAllClades())
            {
                Clade cloned = current.Clone(false);

                Assert.Null(cloned.TreeInternal);
                CompareClades(current.FindRoot(), cloned.FindRoot());
            }
        }

        [Fact]
        public void Interface_ICloneable_Clone()
        {
            _ = new Tree(root);

            foreach (Clade current in GetAllClades())
            {
                var cloned = (Clade)((ICloneable)current).Clone();

                Assert.Null(cloned.TreeInternal);
                CompareClades(current.FindRoot(), cloned.FindRoot());
            }
        }

        [Fact]
        public void AddChild_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => root.AddChild(null!));
        }

        [Fact]
        public void AddChild_WithCladeBelongingToOtherClade()
        {
            var clade = new Clade()
            {
                Parent = cladeB,
            };

            Assert.Throws<ArgumentException>(() => root.AddChild(clade));
        }

        [Fact]
        public void AddChild_AsPositive()
        {
            var child = new Clade()
            {
                Taxon = "leafD",
            };
            root.AddChild(child);

            Assert.Multiple(() =>
            {
                Assert.Same(root, child.Parent);
                Assert.Equal(child, root.ChildrenInternal[^1]);
            });
        }

        [Fact]
        public void ClearChildren()
        {
            root.ClearChildren();

            Assert.Multiple(() =>
            {
                Assert.Null(leafA.Parent);
                Assert.Null(cladeB.Parent);
                Assert.Empty(root.ChildrenInternal);
                Assert.Null(leafC.Parent);
            });
        }

        [Fact]
        public void RemoveChild_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => root.RemoveChild(null!));
        }

        [Fact]
        public void RemoveChild_AsPositive_WithNotContainedClade()
        {
            Assert.False(cladeB.RemoveChild(leafA));
            Assert.NotNull(leafA.Parent);
        }

        [Fact]
        public void RemoveChild_AsPositive_WithContainedClade()
        {
            Assert.True(cladeB.RemoveChild(cladeBA));

            Assert.Multiple(() =>
            {
                Assert.Null(cladeBA.Parent);
                Assert.Equal([cladeBB], cladeB.ChildrenInternal);
            });
        }

        [Fact]
        public void FindRoot()
        {
            Assert.Multiple(() =>
            {
                Assert.Same(root, root.FindRoot());
                Assert.Same(root, leafA.FindRoot());
                Assert.Same(root, cladeB.FindRoot());
                Assert.Same(root, cladeBA.FindRoot());
                Assert.Same(root, leafBAA.FindRoot());
                Assert.Same(root, leafBAB.FindRoot());
                Assert.Same(root, cladeBB.FindRoot());
                Assert.Same(root, cladeBBA.FindRoot());
                Assert.Same(root, leafBBAA.FindRoot());
                Assert.Same(root, leafBBAB.FindRoot());
                Assert.Same(root, leafBBB.FindRoot());
                Assert.Same(root, leafC.FindRoot());
            });
        }

        [Fact]
        public void GetDescendants()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal([leafA, cladeB, cladeBA, leafBAA, leafBAB, cladeBB, cladeBBA, leafBBAA, leafBBAB, leafBBB, leafC], root.GetDescendants());
                Assert.Empty(leafA.GetDescendants());
                Assert.Equal([cladeBA, leafBAA, leafBAB, cladeBB, cladeBBA, leafBBAA, leafBBAB, leafBBB], cladeB.GetDescendants());
                Assert.Equal([leafBAA, leafBAB], cladeBA.GetDescendants());
                Assert.Empty(leafBAA.GetDescendants());
                Assert.Empty(leafBAB.GetDescendants());
                Assert.Equal([cladeBBA, leafBBAA, leafBBAB, leafBBB], cladeBB.GetDescendants());
                Assert.Equal([leafBBAA, leafBBAB], cladeBBA.GetDescendants());
                Assert.Empty(leafBBB.GetDescendants());
                Assert.Empty(leafC.GetDescendants());
            });
        }

        [Fact]
        public void GetLeavesCount()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(7, root.GetLeavesCount());
                Assert.Equal(1, leafA.GetLeavesCount());
                Assert.Equal(5, cladeB.GetLeavesCount());
                Assert.Equal(2, cladeBA.GetLeavesCount());
                Assert.Equal(1, leafBAA.GetLeavesCount());
                Assert.Equal(1, leafBAB.GetLeavesCount());
                Assert.Equal(3, cladeBB.GetLeavesCount());
                Assert.Equal(2, cladeBBA.GetLeavesCount());
                Assert.Equal(1, leafBBAA.GetLeavesCount());
                Assert.Equal(1, leafBBAB.GetLeavesCount());
                Assert.Equal(1, leafBBB.GetLeavesCount());
                Assert.Equal(1, leafC.GetLeavesCount());
            });
        }

        [Fact]
        public void GetTotalBranchLength()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(0, root.GetTotalBranchLength());
                Assert.Equal(2, leafA.GetTotalBranchLength());
                Assert.Equal(2, cladeB.GetTotalBranchLength());
                Assert.Equal(3, cladeBA.GetTotalBranchLength());
                Assert.Equal(8, leafBAA.GetTotalBranchLength());
                Assert.Equal(6, leafBAB.GetTotalBranchLength());
                Assert.Equal(4, cladeBB.GetTotalBranchLength());
                Assert.Equal(5, cladeBBA.GetTotalBranchLength());
                Assert.Equal(7, leafBBAA.GetTotalBranchLength());
                Assert.Equal(6, leafBBAB.GetTotalBranchLength());
                Assert.Equal(7, leafBBB.GetTotalBranchLength());
                Assert.Equal(1, leafC.GetTotalBranchLength());
            });
        }

        [Fact]
        public void GetTotalBranchLength_OnNaNLength()
        {
            cladeB.BranchLength = double.NaN;

            Assert.Multiple(() =>
            {
                Assert.Equal(3, cladeBB.GetTotalBranchLength(1));
                Assert.Equal(double.NaN, cladeBB.GetTotalBranchLength());
            });
        }

        #endregion Methods
    }
}
