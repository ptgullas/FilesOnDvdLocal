using LegacyMediaFilesOnDvd.Data.Context;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MigrateLegacy.Services;
using System.Data.Common;
using System.Linq;

namespace MigrateLegacy.Tests {
    public class LegacyPerformerServiceTests : IDisposable {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;
        private readonly DbContextOptions<LegacyMediaFilesContext> _legacyContextOptions;

        #region ConstructorAndDispose
        public LegacyPerformerServiceTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;

            _legacyContextOptions = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;

            // create the schema in the modern memory DB
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            modernContext.Database.EnsureCreated();

            // seed modern performer types from legacy DB
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            var legacyPerformerTypes = legacyContext.LegacyPerformerTypes.ToList();
            var modernPerformerTypes = LegacyPerformerService.MigrateToPerformerTypes(legacyPerformerTypes);
            modernContext.PerformerTypes.AddRange(modernPerformerTypes);
            modernContext.SaveChanges();
        }

        public void Dispose() => _modernConnection.Dispose();

        #endregion

        [Fact]
        public void Get_RetrievesAllPerformers() {
            // Arrange
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            var service = new LegacyPerformerService(legacyContext);
            int expectedCount = 2019;

            // Act
            var performers = service.Get();

            // Assert
            Assert.Equal(expectedCount, performers.Count());
        }

        [Fact]
        public void Get_PerformersHavePerformerTypes() {
            // Arrange
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            var service = new LegacyPerformerService(legacyContext);

            // Act
            var performers = service.Get();

            // Assert
            // Just check the first few to ensure PerformerType is loaded
            Assert.All(performers.Take(10), p => Assert.NotNull(p.PerformerType));
        }

        [Fact]
        public void MigrateLegacyPerformerTypes_MigratesToPerformerTypes() {
            // Arrange
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            var service = new LegacyPerformerService(legacyContext);
            var legacyPerformerTypes = service.GetPerformerTypes();

            // Act
            var modernPerformerTypes = LegacyPerformerService.MigrateToPerformerTypes(legacyPerformerTypes);

            // Assert
            Assert.Equal(legacyPerformerTypes.Count(), modernPerformerTypes.Count());
            Assert.Equal(legacyPerformerTypes.First().Name, modernPerformerTypes.First().Name);
        }

        [Fact]
        public void MigrateToPerformers_CorrectlyMapsBasicFields() {
            // Arrange
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            var service = new LegacyPerformerService(legacyContext);
            var legacyPerformers = service.Get().Take(10);
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            var modernPerformerTypes = modernContext.PerformerTypes.ToList();

            // Act
            var modernPerformers = LegacyPerformerService.MigrateToPerformers(legacyPerformers, modernPerformerTypes);

            // Assert
            Assert.Equal(legacyPerformers.Count(), modernPerformers.Count());
            var legacyFirst = legacyPerformers.First();
            var modernFirst = modernPerformers.First();
            Assert.Equal(legacyFirst.Name, modernFirst.Name);
            Assert.Equal((int)legacyFirst.Id, modernFirst.LegacyId);
            if (!string.IsNullOrEmpty(legacyFirst.Headshot)) {
                Assert.Single(modernFirst.HeadshotUrls);
                Assert.Equal(legacyFirst.Headshot, modernFirst.HeadshotUrls.First().Url);
            }
            if (legacyFirst.PerformerType != null) {
                Assert.NotNull(modernFirst.Type);
                Assert.Equal(legacyFirst.PerformerType.Name, modernFirst.Type.Name);
            }
        }
    }
}
