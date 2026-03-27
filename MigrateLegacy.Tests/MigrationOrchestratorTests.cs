using LegacyMediaFilesOnDvd.Data.Context;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MigrateLegacy.Services;
using System;
using System.Data.Common;
using System.Linq;

namespace MigrateLegacy.Tests {
    public class MigrationOrchestratorTests : IDisposable {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;
        private readonly DbContextOptions<LegacyMediaFilesContext> _legacyContextOptions;

        public MigrationOrchestratorTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;

            _legacyContextOptions = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;

            using var modernContext = new MediaFilesContext(_modernContextOptions);
            modernContext.Database.EnsureCreated();
        }

        public void Dispose() => _modernConnection.Dispose();

        [Fact]
        public void MigrateAll_FullMigration_Succeeds() {
            // Arrange
            using var legacyContext = new LegacyMediaFilesContext(_legacyContextOptions);
            using var modernContext = new MediaFilesContext(_modernContextOptions);

            var orchestrator = new MigrationOrchestrator(
                new LegacyWalletService(legacyContext), new WalletService(modernContext),
                new LegacyGenreService(legacyContext), new FileGenreService(modernContext),
                new LegacyPublisherService(legacyContext), new SeriesPublisherService(modernContext),
                new LegacyPerformerService(legacyContext), new PerformerService(modernContext),
                new PerformerTypeService(modernContext),
                new LegacyDiscService(legacyContext), new DiscService(modernContext),
                new LegacySeriesService(legacyContext), new SeriesService(modernContext),
                new LegacyFilenameService(legacyContext), new MediaFileService(modernContext)
            );

            string lastProgress = "";
            orchestrator.ProgressReported += (s, e) => lastProgress = e;

            // Act
            orchestrator.MigrateAll();

            // Assert
            Assert.Equal("Migration complete!", lastProgress);
            
            // Verify counts
            Assert.Equal(legacyContext.LegacyWallets.Count(), modernContext.Wallets.Count());
            Assert.Equal(legacyContext.LegacyGenres.Count(), modernContext.FileGenres.Count());
            Assert.Equal(legacyContext.LegacyPublishers.Count(), modernContext.SeriesPublishers.Count());
            Assert.Equal(legacyContext.LegacyPerformerTypes.Count(), modernContext.PerformerTypes.Count());
            Assert.Equal(legacyContext.LegacyPerformers.Count(), modernContext.Performers.Count());
            Assert.Equal(legacyContext.LegacyDiscs.Count(), modernContext.Discs.Count());
            Assert.Equal(legacyContext.LegacySeries.Count(), modernContext.Series.Count());
            Assert.Equal(legacyContext.LegacyFilenames.Count(), modernContext.MediaFiles.Count());

            // Verify a specific relationship (Step 8)
            var sampleFile = modernContext.MediaFiles
                .Include(m => m.Performers)
                .Include(m => m.Disc)
                .FirstOrDefault(m => m.Name.Contains("WWE RAW"));
            
            if (sampleFile != null) {
                Assert.NotNull(sampleFile.Disc);
                // In legacy, WWE RAW files have performers associated. 
                // Let's verify at least one has performers.
                var filesWithPerformers = modernContext.MediaFiles.Include(m => m.Performers).Where(m => m.Performers.Any()).ToList();
                Assert.NotEmpty(filesWithPerformers);
            }
        }
    }
}
