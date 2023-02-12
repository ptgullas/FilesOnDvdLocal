using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Tests {
    public class DiscServiceTests : IDisposable {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<MediaFilesContext> _contextOptions;

        #region ConstructorAndDispose

        public DiscServiceTests() {
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

            Wallet tvWallet = new (){
                Name = "TV",
                Notes = "In work desk drawer. Contains DVDs from mid-2000s to now."
            };

            var discs = new Disc[] {
                new Disc {Name = "PB_1x01-2x12", Notes = "Prison Break Season 1"},
                new Disc {Name = "Flash_S1", Notes = "Flash Season 1"},
                new Disc {Name = "Arrow_S1", Notes = "The season that kicked off the Arrowverse"},
                new Disc {Name = "DoctorWho_S1", Notes = "Eccleston's only season"}
            };
            tvWallet.Discs.AddRange(discs);

            Wallet moviesWallet = new() {
                Name = "Movies",
                Notes = "In work cabinet"
            };

            var moviesDiscs = new Disc[] {
                new Disc { Name = "M2022-12-30a", Notes = "MCU 2022"},
                new Disc { Name = "M2022-12-30b", Notes = "Indies 2022"}
            };
            moviesWallet.Discs.AddRange(moviesDiscs);
            // context.Add(tvWallet);
            context.AddRange(tvWallet, moviesWallet);
            context.SaveChanges();
        }

        public void Dispose() => _connection.Dispose();
        #endregion

        [Fact]
        public void GetDisc_ReturnsDisc() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new DiscService(context);
            string expectedNotes = "Prison Break Season 1";

            // Act
            var result = service.Get("PB_1x01-2x12");
            // Assert
            Assert.Equal(expectedNotes, result.Notes);
        }

        [Fact]
        public void GetAllDiscs_ReturnsAll() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new DiscService(context);

            var expectedCount = 6;
            var expectedName1 = "PB_1x01-2x12";
            var expectedName2 = "Flash_S1";
            var expectedName3 = "Arrow_S1";
            var expectedName4 = "DoctorWho_S1";
            var expectedName5 = "M2022-12-30a";
            var expectedName6 = "M2022-12-30b";

            // Act
            var discs = service.Get();

            // Assert
            Assert.Equal(expectedCount, discs.Count());
            Assert.Collection(discs,
                d => Assert.Equal(expectedName1, d.Name),
                d => Assert.Equal(expectedName2, d.Name),
                d => Assert.Equal(expectedName3, d.Name),
                d => Assert.Equal(expectedName4, d.Name),
                d => Assert.Equal(expectedName5, d.Name),
                d => Assert.Equal(expectedName6, d.Name)
                );
        }

        [Fact]
        public void GetDiscsByWalletName_ReturnsOnlyDiscsInWallet() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new DiscService(context);

            var expectedCount = 2;
            var expectedName1 = "M2022-12-30a";
            var expectedName2 = "M2022-12-30b";

            // Act
            var discs = service.GetByWallet("Movies");
            // Assert
            Assert.Equal(expectedCount, discs.Count());
            Assert.Collection(discs,
                d => Assert.Equal(expectedName1, d.Name),
                d => Assert.Equal(expectedName2, d.Name)
                );
        }

        [Fact]
        public void AddToWallet_SingleDisc_DiscDoesNotAlreadyExist_AddsDisc() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new DiscService(context);

            var walletService = new WalletService(context);

            Disc disc = new() { Name = "Nolan01", Notes = "Christopher Nolan's early work" };
            string expectedNotes = "Christopher Nolan's early work";
            var wallet = walletService.Get("Movies");
            // Act
            var opResult = service.AddToWallet(disc, wallet);

            // Assert
            Assert.True(opResult.Success);
            var result = context.Discs.FirstOrDefault(d => d.Name == "Nolan01");
            Assert.Equal(expectedNotes, result.Notes);
        }

        [Fact]
        public void AddToWallet_SingleDisc_DiscAlreadyExists_DoesNotAddDisc() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new DiscService(context);

            var walletService = new WalletService(context);

            Disc disc = new() { Name = "M2022-12-30a", Notes = "Notes field doesn't matter" };
            var wallet = walletService.Get("Movies");
            string expectedMessage = "Disc M2022-12-30a already exists in database.";
            // Act
            var opResult = service.AddToWallet(disc, wallet);

            // Assert
            Assert.False(opResult.Success);
            Assert.Equal(expectedMessage, opResult.Message);
        }
    }
}
