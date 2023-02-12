using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace MediaFilesOnDvd.Tests {
    public class WalletServiceTests : IDisposable {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<MediaFilesContext> _contextOptions;

        #region ConstructorAndDispose
        public WalletServiceTests() {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_connection)
                .Options;

            // create the schema & seed some data
            using var context = new MediaFilesContext(_contextOptions);
            if (context.Database.EnsureCreated()) {
                // OK, EnsureCreated creates the tables based on the EF model in the DbContext,
                // so we don't need to execute viewCommand

                //using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                //viewCommand.CommandText = GetSqlCommand();
                //viewCommand.ExecuteNonQuery();
            }

            context.AddRange(
                new Wallet {
                    Name = "TV",
                    Notes = "In work desk drawer. Contains DVDs from mid-2000s to now."
                },
                new Wallet {
                    Name = "Wrestling",
                    Notes = "In work wide cabinet, top shelf"
                }
                );
            context.SaveChanges();
        }

        MediaFilesContext CreateContext() => new MediaFilesContext(_contextOptions);

        private static string GetSqlCommand() {
            return @"CREATE TABLE ""Wallets"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Wallets"" PRIMARY KEY AUTOINCREMENT,
    ""Name"" TEXT NOT NULL,
    ""ImageUrl"" TEXT NULL,
    ""Notes"" TEXT NULL,
    ""StorageUnitId"" INTEGER NULL
);

CREATE TABLE ""Discs"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Discs"" PRIMARY KEY AUTOINCREMENT,
    ""Name"" TEXT NOT NULL,
    ""Notes"" TEXT NULL,
    ""WalletId"" INTEGER NULL,
    CONSTRAINT ""FK_Discs_Wallets_WalletId"" FOREIGN KEY (""WalletId"") REFERENCES ""Wallets"" (""Id"")
);";
        }

        public void Dispose() => _connection.Dispose();

        #endregion

        private static string GetTestDbPath() {
            var folder = Environment.SpecialFolder.Personal;
            var path = Environment.GetFolderPath(folder);
            path = Path.Combine(path, "MediaFilesOnDvd");
            return Path.Join(path, "MediaFilesOnDvdTest.db");
        }

        [Fact]
        public void GetWallet() {
            using var context = CreateContext();
            var walletService = new WalletService(context);
            var wallet = walletService.Get("TV");

            string expectedNotes = "In work desk drawer. Contains DVDs from mid-2000s to now.";
            Assert.NotNull(wallet);
            Assert.Equal(expectedNotes, wallet.Notes);
        }

        [Fact]
        public void GetAllWallets() {
            using var context = CreateContext();
            var walletService = new WalletService(context);

            string expectedName1 = "TV";
            string expectedName2 = "Wrestling";

            var wallets = walletService.Get();
            Assert.Collection<Wallet>(
                wallets,
                w => Assert.Equal(expectedName1, w.Name),
                w => Assert.Equal(expectedName2, w.Name));
        }

        [Fact]
        public void AddWallet() {
            // Arrange
            using var context = CreateContext();
            var walletService = new WalletService(context);

            Wallet wallet = new() { Name = "Movies", Notes = "In work cabinet, second shelf" };
            string expectedNotes = "In work cabinet, second shelf";
            // Act
            walletService.Add(wallet);

            var result = context.Wallets.Single(w => w.Name == "Movies");

            // Assert
            Assert.Equal(expectedNotes, result.Notes);
        }
        /*
        [Fact]
        public void CreateFirstWallet() {
            // Arrange
            string dbPath = GetTestDbPath();
            var options = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            var context = new MediaFilesContext(options);

            Wallet wallet = new() {
                Name = "TV",
                Notes = "In work desk drawer. Contains DVDs from mid-2000s to now."
            };

            WalletService walletService = new(context);
            string expected = "Porn";

            // Act
            walletService.Add(wallet);
            // context.Database.EnsureCreated();
            var result = walletService.Get("porn");

            // Assert
            Assert.Equal(expected, result?.Name);
        }
        [Fact]
        public void CreateFirstDisc() {
            // Arrange
            string dbPath = GetTestDbPath();
            var options = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            var context = new MediaFilesContext(options);

            Disc disc = new() {
                Name = "2022-12-21a"
            };

            DiscService discService = new(context);

            WalletService walletService = new(context);
            var wallet = walletService.Get("TV");
            if (wallet is not null && wallet.Id is not null) {
                disc.WalletId = wallet.Id;
            }
            string expectedWalletName = "TV";

            // Act
            discService.Add(disc);
            // context.Database.EnsureCreated();
            var result = discService.Get("2022-12-21a");

            // Assert
            Assert.Equal(expectedWalletName, result?.Wallet.Name);
        }

        [Fact]
        public void AddMultipleDiscs() {
            // Arrange
            string dbPath = GetTestDbPath();
            var options = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            var context = new MediaFilesContext(options);

            WalletService walletService = new(context);
            var wallet = walletService.Get("TV");
            int? walletId = 0;
            if (wallet is not null && wallet.Id is not null) {
                walletId = wallet.Id;
            }


            var discs = new Disc[] {
                new Disc {Name = "PB_1x01-2x12", Notes = "Prison Break Season 1", WalletId = walletId},
                new Disc {Name = "Flash_S1", WalletId = walletId},
                new Disc {Name = "Arrow_S1", WalletId = walletId},
                new Disc {Name = "DoctorWho_S1", WalletId = walletId},

            };

            DiscService discService = new(context);
            string expectedDiscName01 = "Arrow_S0";
            string expectedDiscName02 = "DoctorWho_S0";


            // Act
            discService.Add(discs);
            // context.Database.EnsureCreated();
            var result = discService.GetByWallet(walletId);

            // Assert
            Assert.Contains(result, d => d.Name == expectedDiscName01);
            Assert.Contains(result, d => d.Name == expectedDiscName02);
        }
        */

    }
}