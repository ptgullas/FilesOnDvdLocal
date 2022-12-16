using LegacyMediaFilesOnDvd.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;

namespace MigrateLegacy.Tests {
    public class LegacyDiscServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";
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

    }
}