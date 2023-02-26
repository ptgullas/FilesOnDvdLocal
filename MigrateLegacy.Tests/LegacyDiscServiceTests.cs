using LegacyMediaFilesOnDvd.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using MediaFilesOnDvd.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;

namespace MigrateLegacy.Tests {
    public class LegacyDiscServiceTests : IDisposable {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;

        #region ConstructorAndDispose
        public LegacyDiscServiceTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;
            // create the schema & seed some data
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            if (modernContext.Database.EnsureCreated()) {
            }

            var wallets = new Wallet[] {
                new Wallet {Name = "Porn"},
                new Wallet {Name = "Movies"},
                new Wallet {Name = "Wrestling"}
            };
            modernContext.Wallets.AddRange(wallets);
            modernContext.SaveChanges();
        }

        public void Dispose() => _modernConnection.Dispose();
        #endregion

        [Fact]
        public void Get_ReturnsAllDiscs() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyDiscService discService = new(context);

            int expectedDiscCount = 233;
            // Act
            var discs = discService.Get();
            // Assert
            Assert.Equal(expectedDiscCount, discs.Count);
        }

        [Fact]
        public void FindByWalletId_ReturnsDiscs() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyDiscService discService = new(context);

            int walletId = 1;
            int expectedDiscCount = 177;
            // Act
            var discs = discService.FindByWalletId(walletId);
            // Assert
            Assert.Equal(expectedDiscCount, discs.Count);
        }

        [Fact]
        public void MigrateToNewDiscs_MigrateEveryDisc_CreatesNewDiscs() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyDiscService discService = new(context);

            using var modernContext = new MediaFilesContext(_modernContextOptions);

            WalletService newWalletService = new(modernContext);

            string discNameToTest = "W2012-07-11a";

            int expectedCount = 233;
            string expectedNotes = "WWE Smackdown";
            string expectedWalletName = "Wrestling";

            int expectedWalletCount1 = 177;
            int expectedWalletCount2 = 16;
            int expectedWalletCount3 = 40;
            // Act
            var legacyDiscs = discService.Get();
            var newDiscs = discService.MigrateToNewDiscs(legacyDiscs, newWalletService);
            var disc = newDiscs.FirstOrDefault(d => d.Name.ToLower() == discNameToTest.ToLower());

            modernContext.SaveChanges();
            var newWallets = newWalletService.Get();

            Wallet wallet1 = newWallets.First();
            Wallet wallet2 = newWallets.Skip(1).First();
            Wallet wallet3 = newWallets.Skip(2).First();

            // Assert
            Assert.Equal(expectedCount, newDiscs.Count());
            Assert.NotNull(disc);
            Assert.Equal(expectedNotes, disc.Notes);
            Assert.Equal(expectedWalletName, disc.Wallet.Name);

            Assert.Equal(expectedWalletCount1, wallet1.Discs.Count);
            Assert.Equal(expectedWalletCount2, wallet2.Discs.Count);
            Assert.Equal(expectedWalletCount3, wallet3.Discs.Count);
        }
    }
}