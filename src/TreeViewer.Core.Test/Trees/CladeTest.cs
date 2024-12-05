namespace TreeViewer.Core.Trees
{
    public class CladeTest
    {
        //          2
        //      +------leafA    1
        // root-|   2         +---leafBA
        //      +------cladeB-|    3
        //                    +---------leafBB

        private readonly Clade root;
        private readonly Clade leafA;
        private readonly Clade cladeB;
        private readonly Clade leafBA;
        private readonly Clade leafBB;

        public CladeTest()
        {
            root = new Clade()
            {
                Supports = "80/100",
                BranchLength = 0,
            };
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
            leafBA = new Clade()
            {
                Taxon = "BA",
                BranchLength = 1,
                Parent = cladeB,
            };
            leafBB = new Clade()
            {
                Taxon = "BB",
                BranchLength = 3,
                Parent = cladeB,
            };

            root.ChildrenInternal.AddRange(leafA, cladeB);
            cladeB.ChildrenInternal.AddRange(leafBA, leafBB);
        }

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
                Assert.False(leafBA.IsRoot);
                Assert.False(leafBB.IsRoot);
            });
        }

        [Fact]
        public void IsLeaf_Get()
        {
            Assert.False(root.IsLeaf);
            Assert.True(leafA.IsLeaf);
            Assert.False(cladeB.IsLeaf);
            Assert.True(leafBA.IsLeaf);
            Assert.True(leafBB.IsLeaf);
        }

        [Fact]
        public void Children_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(root.ChildrenInternal, root.Children);
                Assert.Equal(leafA.ChildrenInternal, leafA.Children);
                Assert.Equal(cladeB.ChildrenInternal, cladeB.Children);
                Assert.Equal(leafBA.ChildrenInternal, leafBA.Children);
                Assert.Equal(leafBB.ChildrenInternal, leafBB.Children);
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
                Assert.Null(leafBA.Tree);
                Assert.Null(leafBB.Tree);
            });
        }

        #endregion Properties

        #region Methods

        [Fact]
        public void Clone_WithBoolAsOnlyDescendants()
        {
            _ = new Tree(root);

            foreach (Clade? current in new[] { root, leafA, cladeB, leafBA, leafBB })
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

            foreach (Clade? current in new[] { root, leafA, cladeB, leafBA, leafBB })
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

            foreach (Clade? current in new[] { root, leafA, cladeB, leafBA, leafBB })
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
                Taxon = "leafC",
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
            Assert.True(cladeB.RemoveChild(leafBA));

            Assert.Multiple(() =>
            {
                Assert.Null(leafBA.Parent);
                Assert.Equal([leafBB], cladeB.ChildrenInternal);
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
                Assert.Same(root, leafBA.FindRoot());
                Assert.Same(root, leafBB.FindRoot());
            });
        }

        [Fact]
        public void GetDescendants()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal([leafA, cladeB, leafBA, leafBB], root.GetDescendants());
                Assert.Empty(leafA.GetDescendants());
                Assert.Equal([leafBA, leafBB], cladeB.GetDescendants());
                Assert.Empty(leafBA.GetDescendants());
                Assert.Empty(leafBB.GetDescendants());
            });
        }

        [Fact]
        public void GetLeavesCount()
        {
            Assert.Multiple(() =>
            {
                Assert.Equal(3, root.GetLeavesCount());
                Assert.Equal(1, leafA.GetLeavesCount());
                Assert.Equal(2, cladeB.GetLeavesCount());
                Assert.Equal(1, leafBA.GetLeavesCount());
                Assert.Equal(1, leafBB.GetLeavesCount());
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
                Assert.Equal(3, leafBA.GetTotalBranchLength());
                Assert.Equal(5, leafBB.GetTotalBranchLength());
            });
        }

        [Fact]
        public void GetTotalBranchLength_OnNaNLength()
        {
            cladeB.BranchLength = double.NaN;

            Assert.Multiple(() =>
            {
                Assert.Equal(4, leafBB.GetTotalBranchLength(1));
                Assert.Equal(double.NaN, leafBB.GetTotalBranchLength());
            });
        }

        #endregion Methods
    }
}
