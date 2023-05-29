using LegacyMediaFilesOnDvd.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;

namespace MigrateLegacy.Tests {
    public class LegacyPublisherServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        [Fact]
        public void Get_GetAll_ReturnsAll() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyPublisherService service = new(context);

            int expectedPublisherCount = 16;
            // Act
            var legacyPublishers = service.Get();
            // Assert
            Assert.Equal(expectedPublisherCount, legacyPublishers.Count());
        }

        [Fact]
        public void MigrateToSeriesPublishers_MigratesAllPublishers() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyPublisherService service = new(context);

            int expectedSeriesPublisherCount = 16;
            // Act
            var legacyPublishers = service.Get();
            var modernPublishers = service.MigrateToSeriesPublishers(legacyPublishers);
            // Assert
            Assert.Equal(expectedSeriesPublisherCount, modernPublishers.Count());
        }
    }
}
