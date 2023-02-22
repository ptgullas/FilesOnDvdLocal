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
            int expectedSeries = 88;
            int expectedDisc = 202;
            // string expectedDiscName = "W2015-08-05b";
            // Act
            var result = service.Get(fileNameIdToGet);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFilename, result.Name);
            Assert.Equal(expectedGenreId, result.GenreId);
            Assert.Equal(expectedGenreName, result.Genre.Name);
            Assert.Equal(expectedSeries, result.Series);
            Assert.Equal(expectedDisc, result.Disc);
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
    }
}
