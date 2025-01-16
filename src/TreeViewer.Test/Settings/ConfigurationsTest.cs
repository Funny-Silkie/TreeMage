﻿using TreeViewer.Core.Drawing;
using TreeViewer.Core.Exporting;
using TreeViewer.TestUtilities.Assertions;

namespace TreeViewer.Settings
{
    public class ConfigurationsTest
    {
        private readonly Configurations config;

        public ConfigurationsTest()
        {
            config = new Configurations();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            var config = new Configurations();

            Assert.Multiple(() =>
            {
                Assert.Equal(960, config.MainWindowWidth);
                Assert.Equal(720, config.MainWindowHeight);
                Assert.Equal(BranchColoringType.Both, config.BranchColoring);
                Assert.Equal(Data.AutoOrderingMode.Descending, config.AutoOrderingMode);
            });
        }

        #endregion Ctors

        #region Static Properties

        [Fact]
        public void Location_Get()
        {
            Assert.Equal(Path.GetFullPath(@"config.json"), Configurations.Location);
        }

        #endregion Static Properties

        #region Static Methods

        [Fact]
        public void LoadOrCreate_OnFileExisting()
        {
            config.BranchColoring = BranchColoringType.Horizontal;
            config.Save();

            Configurations loaded = Configurations.LoadOrCreate();

            CustomizedAssertions.Equal(loaded, config);
        }

        [Fact]
        public void LoadOrCreate_OnFileMissing()
        {
            config.BranchColoring = BranchColoringType.Horizontal;
            File.Delete(Configurations.Location);

            Configurations loaded = Configurations.LoadOrCreate();

            CustomizedAssertions.Equal(new Configurations(), loaded);
        }

        [Fact]
        public async Task LoadOrCreateAsync_OnFileExisting()
        {
            config.BranchColoring = BranchColoringType.Horizontal;
            await config.SaveAsync();

            Configurations loaded = await Configurations.LoadOrCreateAsync();

            CustomizedAssertions.Equal(loaded, config);
        }

        [Fact]
        public async Task LoadOrCreateAsync_OnFileMissing()
        {
            config.BranchColoring = BranchColoringType.Horizontal;
            File.Delete(Configurations.Location);

            Configurations loaded = await Configurations.LoadOrCreateAsync();

            CustomizedAssertions.Equal(new Configurations(), loaded);
        }

        #endregion Static Methods

        #region Instance Methods

        [Fact]
        public void Save()
        {
            File.Delete(Configurations.Location);

            config.Save();

            Assert.Multiple(() =>
            {
                Assert.True(File.Exists(Configurations.Location));
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("View", "Settings", "config.json"), Configurations.Location);
            });
        }

        [Fact]
        public async Task SaveAsync()
        {
            File.Delete(Configurations.Location);

            await config.SaveAsync();

            Assert.Multiple(() =>
            {
                Assert.True(File.Exists(Configurations.Location));
                CustomizedAssertions.EqualTextFiles(CreateTestDataPath("View", "Settings", "config.json"), Configurations.Location);
            });
        }

        [Fact]
        public void ToExportOptions()
        {
            config.BranchColoring = BranchColoringType.Horizontal;
            ExportOptions options = config.ToExportOptions();

            Assert.Equal(config.BranchColoring, options.BranchColoring);
        }

        #endregion Instance Methods
    }
}
