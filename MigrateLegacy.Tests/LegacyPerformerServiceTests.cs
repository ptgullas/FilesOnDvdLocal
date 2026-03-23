using LegacyMediaFilesOnDvd.Data.Context;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;
using System.Data.Common;

namespace MigrateLegacy.Tests {
    public class LegacyPerformerServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;

        #region ConstructorAndDispose
        public LegacyPerformerServiceTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;
            // create the schema & seed some data
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            if (modernContext.Database.EnsureCreated()) {
            }

            var modernPerformerTypes = new PerformerType[] {
                new PerformerType { Name = "Wrestler" },
                new PerformerType { Name = "Model" },
                new PerformerType { Name = "Actor" }
            };

            modernContext.AddRange(modernPerformerTypes);
        }

        public void Dispose() => _modernConnection.Dispose();

        #endregion

        [Fact]
        public void MigrateLegacyPerformerTypes_MigratesToPerformerTypes() {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public void MigrateToPerformers() {

        }
    
    }
}
