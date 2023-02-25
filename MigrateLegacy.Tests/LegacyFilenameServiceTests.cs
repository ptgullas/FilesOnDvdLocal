using LegacyMediaFilesOnDvd.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;

namespace MigrateLegacy.Tests {
    public class LegacyFilenameServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";
        [Fact]
        public void Get_GetAll_ReturnsAllFilenames() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int expectedFilenameCount = 4711;

            // Act
            var results = service.Get();
            // Assert
            Assert.NotEmpty(results);
            Assert.Equal(expectedFilenameCount, results.Count());
        }

        [Fact]
        public void Get_GetById_ReturnsSingleFilename() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int fileNameIdToGet = 4377;
            string expectedFilename = "2012-09-24 WWE RAW - Layla & Alicia Fox vs. Eve & Beth Phoenix.mp4";

            // change these to the Genre & Series names when we add them
            int expectedGenreId = 4;
            string expectedGenreName = "Wrestling";
            int expectedSeriesId = 88;
            string expectedSeriesName = "WWE RAW";
            int expectedDiscId = 202;
            string expectedDiscName = "W2015-08-05b";
            // Act
            var result = service.Get(fileNameIdToGet);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFilename, result.Name);
            Assert.Equal(expectedGenreId, result.GenreId);
            Assert.Equal(expectedGenreName, result.Genre.Name);
            Assert.Equal(expectedSeriesId, result.SeriesId);
            Assert.Equal(expectedSeriesName, result.Series.Name);
            Assert.Equal(expectedDiscId, result.DiscId);
            Assert.Equal(expectedDiscName, result.Disc.Name);
        }

        [Fact]
        public void Get_GetById_ReturnsPerformers() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int fileNameIdToGet = 1412;
            string expectedFilename = "Predator 3 - The Final Chapter (2009) - Scene 4 - India Summer & Abbey Brooks.avi";

            // change these to the Genre & Series names when we add them
            int expectedGenreId = 3;
            int expectedDiscId = 66;
            string expectedDiscName = "P2010-11-24a-LES";

            int expectedPerformerCount = 3;
            string expectedFirstPerformerName = "Abbey Brooks";
            // Act
            var result = service.Get(fileNameIdToGet);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFilename, result.Name);
            Assert.Equal(expectedGenreId, result.GenreId);
            Assert.Equal(expectedDiscId, result.DiscId);
            Assert.Equal(expectedDiscName, result.Disc.Name);

            Assert.Equal(expectedPerformerCount, result.Performers.Count);
            Assert.Equal(expectedFirstPerformerName, result.Performers.First().Name);
        }

        [Fact]
        public void GetByDisc_GetWrestlingDiscs_ReturnsWrestlingDiscs() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            string discNameToGet = "W2010-04-24";
            int expectedCount = 44;

            string expectedFirstFileName = "1984-01-23 - Hulk Hogan vs. The Iron Sheik.mp4";
            // Act

            var results = service.GetByDiscName(discNameToGet);
            // Assert

            Assert.NotEmpty(results);
            Assert.Equal(expectedCount, results.Count());
            Assert.Equal(expectedFirstFileName, results.First().Name);
        }

        [Fact]
        public void GetBySeries_GetPrisonBreakFiles_ReturnsFiles() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            string seriesToGet = "Prison Break";
            int expectedCount = 12;

            string expectedFirstFileName = "Prison Break - 1.01 - Pilot.avi";
            // Act

            var results = service.GetBySeries(seriesToGet);
            // Assert

            Assert.NotEmpty(results);
            Assert.Equal(expectedCount, results.Count());
            Assert.Equal(expectedFirstFileName, results.First().Name);
        }

    }
}
