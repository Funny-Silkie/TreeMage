﻿using System.IO.Compression;
using TreeMage.TestUtilities.Assertions;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.ProjectData.Json;
using TreeMage.Core.Trees;
using TreeMage.TestUtilities;

namespace TreeMage.Core.ProjectData
{
    public class ProjectDataTest
    {
        private const string outputPath = "data.treeprj";

        private readonly ProjectData data;

        public ProjectDataTest()
        {
            Tree tree1 = DummyData.CreateTree();
            foreach ((int i, Clade clade) in tree1.GetAllClades().Index()) clade.Style.LeafColor = i.ToString();
            tree1.Style.DecorationStyles = [
                new BranchDecorationStyle()
                {
                    DecorationType = BranchDecorationType.OpenedRectangle,
                    RegexPattern = "100",
                },
            ];

            data = new ProjectData()
            {
                Trees = [tree1],
            };
        }

        #region Ctors

        [Fact]
        public void Ctor_WithoutArgs()
        {
            var projectData = new ProjectData();

            Assert.Empty(projectData.Trees);
        }

        #endregion Ctors

        #region Static Methods

        [Fact]
        public void Load_WithString_AsPositive()
        {
            ProjectData result = ProjectData.Load(CreateTestDataPath("Core", "ProjectData", "data.treeprj"));

            Assert.Multiple(() =>
            {
                Tree tree = Assert.Single(result.Trees);
                CustomizedAssertions.Equal(data.Trees[0], tree);
            });
        }

        [Fact]
        public void Load_WithStream_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectData.Load(stream: null!));
        }

        [Fact]
        public void Load_WithStream_AsPositive()
        {
            ProjectData result;
            using (var stream = new FileStream(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), FileMode.Open))
            {
                result = ProjectData.Load(stream);
            }

            Assert.Multiple(() =>
            {
                Tree tree = Assert.Single(result.Trees);
                CustomizedAssertions.Equal(data.Trees[0], tree);
            });
        }

        [Fact]
        public async Task LoadAsync_WithString_AsPositive()
        {
            ProjectData result = await ProjectData.LoadAsync(CreateTestDataPath("Core", "ProjectData", "data.treeprj"));

            Assert.Multiple(() =>
            {
                Tree tree = Assert.Single(result.Trees);
                CustomizedAssertions.Equal(data.Trees[0], tree);
            });
        }

        [Fact]
        public async Task LoadAsync_WithStream_WithNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ProjectData.LoadAsync(stream: null!));
        }

        [Fact]
        public async Task LoadAsync_WithStream_AsPositive()
        {
            ProjectData result;
            using (var stream = new FileStream(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), FileMode.Open))
            {
                result = await ProjectData.LoadAsync(stream);
            }

            Assert.Multiple(() =>
            {
                Tree tree = Assert.Single(result.Trees);
                CustomizedAssertions.Equal(data.Trees[0], tree);
            });
        }

        #endregion Static Methods

        #region Instance Methods

        [Fact]
        public void ToJson()
        {
            JsonRootData json = data.ToJson();

            Assert.Multiple(() =>
            {
                Assert.Equal("1", json.Version);
                Assert.NotNull(json.Data);
            });
        }

        [Fact]
        public void Save_WithString_AsPositive()
        {
            data.Save(outputPath);

            CustomizedAssertions.EqualProjectFiles(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), outputPath);
        }

        [Fact]
        public void Save_WithStream_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => data.Save(stream: null!));
        }

        [Fact]
        public void Save_WithStream_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                data.Save(stream);
            }

            CustomizedAssertions.EqualProjectFiles(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), outputPath);
        }

        [Fact]
        public async Task SaveAsync_WithString_AsPositive()
        {
            await data.SaveAsync(outputPath);

            CustomizedAssertions.EqualProjectFiles(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), outputPath);
        }

        [Fact]
        public async Task SaveAsync_WithStream_WithNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => data.SaveAsync(stream: null!));
        }

        [Fact]
        public async Task SaveAsync_WithStream_AsPositive()
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                await data.SaveAsync(stream);
            }

            CustomizedAssertions.EqualProjectFiles(CreateTestDataPath("Core", "ProjectData", "data.treeprj"), outputPath);
        }

        #endregion Instance Methods
    }
}
