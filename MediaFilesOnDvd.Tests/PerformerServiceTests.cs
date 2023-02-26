using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Tests {
    public class PerformerServiceTests : IDisposable {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<MediaFilesContext> _contextOptions;

        #region ConstructorAndDispose
        public PerformerServiceTests() {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();


            _contextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_connection)
                .Options;

            // uncomment this to test with an actual file:
            // _contextOptions = CreateDbContextOptions();

            // create the schema & seed some data
            using var context = new MediaFilesContext(_contextOptions);
            if (context.Database.EnsureCreated()) {
                // OK, EnsureCreated creates the tables based on the EF model in the DbContext,
                // so we don't need to execute viewCommand

                //using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                //viewCommand.CommandText = GetSqlCommand();
                //viewCommand.ExecuteNonQuery();
            }

            FileGenre tvGenre = new() {
                Name = "TV show"
            };
            context.FileGenres.Add(tvGenre);

            var performers = new Performer[] {
                new Performer {Name = "Aubrey Plaza", HeadshotUrls = { new HeadshotUrl("aubrey_plaza.jpg") } },
                new Performer {Name = "Jennifer Coolidge", HeadshotUrls = { new HeadshotUrl("jennifer_coolidge.jpg") }},
                new Performer {Name = "Sabrina Impacciatore", HeadshotUrls = { new HeadshotUrl("sabrina_impacciatore.jpg") }},
                new Performer {Name = "Cecily Strong", HeadshotUrls = { new HeadshotUrl("cecily_strong.jpg") }},
                new Performer {Name = "Keegan-Michael Key", HeadshotUrls = { new HeadshotUrl("keegan-michael_key.jpg") }},
                new Performer {Name = "John Cena", HeadshotUrls = { new HeadshotUrl("john_cena.jpg") }},
                new Performer {Name = "Jeremy Renner", HeadshotUrls = { new HeadshotUrl("jeremy_renner.jpg") }},
                new Performer {Name = "Kenan Thompson", HeadshotUrls = { new HeadshotUrl("kenan_thompson.jpg") }},
                new Performer {Name = "Jordan Peele"},
                new Performer {Name = "Heidi Gardner"},
                new Performer {Name = "Zazie Beetz"}
            };
            context.Performers.AddRange(performers);
            context.SaveChanges();

            var aubrey = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Aubrey"));
            var jennifer = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jennifer"));
            var sabrina = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Sabrina"));
            var cecily = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Cecily"));
            var kenan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Kenan"));
            var keegan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Keegan"));
            var john = context.Performers.FirstOrDefault(p => p.Name.StartsWith("John"));
            var jeremy = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jeremy"));
            var jordan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jordan"));
            var heidi = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Heidi"));

#pragma warning disable CS8604 // Possible null reference argument.
            var mediaFiles = new MediaFile[] {
                new MediaFile {
                    Name = "The White Lotus - 2.01 - Ciao.mkv",
                    Performers = new List<Performer> {aubrey, jennifer, sabrina},
                    Notes = "Second season premiere of this fantastic show, filmed in Sicily!"
                },
                new MediaFile {
                    Name = "Schmigadoon! - 1.06 - How We Change.mkv",
                    Performers = {cecily, keegan},
                    Notes = "Fun musical show"
                },
                new MediaFile {
                    Name = "Peacemaker - 1.01 - A Whole New Whirled.mkv",
                    Performers = {john},
                    Notes = "Follow-up to James Gunn's 'The Suicide Squad'"
                },
                new MediaFile {
                    Name = "Hawkeye (2021) - 1.04 - Partners, Am I Right.mkv",
                    Performers = {jeremy},
                    Notes = "MCU show on Disney+"
                },
                new MediaFile {
                    Name = "The Hurt Locker (2008).avi",
                    Performers = {jeremy},
                    Notes = "Kathryn Bigelow film about effects of war or whatever"
                },
                new MediaFile {
                    Name = "Saturday Night Live - 44.01 - Adam Driver",
                    Performers = {cecily, kenan, heidi},
                    Notes = "Kathryn Bigelow film about effects of war or whatever"
                },
                new MediaFile {
                    Name = "Key & Peele - 1.01",
                    Performers = {keegan, jordan},
                    Notes = "Series premiere of great sketch show"
                }

            };
#pragma warning restore CS8604 // Possible null reference argument.

            // context.MediaFiles.AddRange(mediaFiles);
            // context.SaveChanges();
            Wallet tvWallet = new() { Name = "TV", Notes = "TV wallet in work drawer" };
            WalletService walletService = new(context);
            walletService.Add(tvWallet);

            Disc miscTV2023a = new() {
                Name = "MiscTV2023-02-17",
                Notes = "Misc TV episodes",
                Files = mediaFiles
            };
            foreach (var file in mediaFiles) {
                tvGenre.MediaFiles.Add(file);
            }
            DiscService discService = new(context);
            discService.AddToWallet(miscTV2023a, tvWallet);

        }
        public void Dispose() => _connection.Dispose();
        #endregion

        [Fact]
        public void Get_GetAllWithoutMediaFiles_GetsPerformers() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            int expectedCount = 11;
            string expectedName1 = "Aubrey Plaza";
            string expectedName2 = "Cecily Strong";
            string expectedName3 = "Heidi Gardner";
            string expectedName4 = "Jennifer Coolidge";
            string expectedName5 = "Jeremy Renner";
            string expectedName6 = "John Cena";
            string expectedName7 = "Jordan Peele";
            string expectedName8 = "Keegan-Michael Key";
            string expectedName9 = "Kenan Thompson";
            string expectedName10 = "Sabrina Impacciatore";
            string expectedName11 = "Zazie Beetz";
            // Act

            var performers = service.Get();
            // Assert
            Assert.NotNull(performers);
            Assert.Equal(expectedCount, performers.Count());

            Assert.Collection(performers,
                p => Assert.Equal(expectedName1, p.Name),
                p => Assert.Equal(expectedName2, p.Name),
                p => Assert.Equal(expectedName3, p.Name),
                p => Assert.Equal(expectedName4, p.Name),
                p => Assert.Equal(expectedName5, p.Name),
                p => Assert.Equal(expectedName6, p.Name),
                p => Assert.Equal(expectedName7, p.Name),
                p => Assert.Equal(expectedName8, p.Name),
                p => Assert.Equal(expectedName9, p.Name),
                p => Assert.Equal(expectedName10, p.Name),
                p => Assert.Equal(expectedName11, p.Name)
                );

            // Get does not include a Performer's MediaFiles
            Assert.Empty(performers.First().MediaFiles);
        }

        [Fact]
        public void Get_GetAllWithMediaFiles_GetsPerformersAndTheirMediaFiles() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            int expectedCount = 11;
            string expectedName1 = "Aubrey Plaza";
            string expectedName2 = "Cecily Strong";
            string expectedName3 = "Heidi Gardner";
            string expectedName4 = "Jennifer Coolidge";
            string expectedName5 = "Jeremy Renner";
            string expectedName6 = "John Cena";
            string expectedName7 = "Jordan Peele";
            string expectedName8 = "Keegan-Michael Key";
            string expectedName9 = "Kenan Thompson";
            string expectedName10 = "Sabrina Impacciatore";
            string expectedName11 = "Zazie Beetz";

            string performerToGet = "Cecily Strong";
            string expectedMediaName01 = "Saturday Night Live - 44.01 - Adam Driver";
            string expectedMediaName02 = "Schmigadoon! - 1.06 - How We Change.mkv";
            // Act

            var performers = service.GetWithMediaFiles();
            // Assert
            Assert.NotNull(performers);
            Assert.Equal(expectedCount, performers.Count());

            Assert.Collection(performers,
                p => Assert.Equal(expectedName1, p.Name),
                p => Assert.Equal(expectedName2, p.Name),
                p => Assert.Equal(expectedName3, p.Name),
                p => Assert.Equal(expectedName4, p.Name),
                p => Assert.Equal(expectedName5, p.Name),
                p => Assert.Equal(expectedName6, p.Name),
                p => Assert.Equal(expectedName7, p.Name),
                p => Assert.Equal(expectedName8, p.Name),
                p => Assert.Equal(expectedName9, p.Name),
                p => Assert.Equal(expectedName10, p.Name),
                p => Assert.Equal(expectedName11, p.Name)
                );

            // a Performer's MediaFiles should be ordered by Name
            var cecilyMediaFiles = performers.Where(p => p.Name == performerToGet)
                .First()
                .MediaFiles;

            Assert.Collection(cecilyMediaFiles,
                m => Assert.Equal(expectedMediaName01, m.Name),
                m => Assert.Equal(expectedMediaName02, m.Name)
                );
        }

        [Fact]
        public void Get_GetAllWithMediaFiles_PerformerWithNoMediaFilesHasEmptyCollection() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            int expectedCount = 11;
            string expectedName1 = "Aubrey Plaza";
            string expectedName2 = "Cecily Strong";
            string expectedName3 = "Heidi Gardner";
            string expectedName4 = "Jennifer Coolidge";
            string expectedName5 = "Jeremy Renner";
            string expectedName6 = "John Cena";
            string expectedName7 = "Jordan Peele";
            string expectedName8 = "Keegan-Michael Key";
            string expectedName9 = "Kenan Thompson";
            string expectedName10 = "Sabrina Impacciatore";
            string expectedName11 = "Zazie Beetz";

            string performerToGet = "Zazie Beetz";
            
            // Act

            var performers = service.GetWithMediaFiles();
            // Assert
            Assert.NotNull(performers);
            Assert.Equal(expectedCount, performers.Count());

            Assert.Collection(performers,
                p => Assert.Equal(expectedName1, p.Name),
                p => Assert.Equal(expectedName2, p.Name),
                p => Assert.Equal(expectedName3, p.Name),
                p => Assert.Equal(expectedName4, p.Name),
                p => Assert.Equal(expectedName5, p.Name),
                p => Assert.Equal(expectedName6, p.Name),
                p => Assert.Equal(expectedName7, p.Name),
                p => Assert.Equal(expectedName8, p.Name),
                p => Assert.Equal(expectedName9, p.Name),
                p => Assert.Equal(expectedName10, p.Name),
                p => Assert.Equal(expectedName11, p.Name)
                );

            // a Performer's MediaFiles should be ordered by Name
            var zazieMediaFiles = performers.Where(p => p.Name == performerToGet)
                .First()
                .MediaFiles;

            Assert.Empty(zazieMediaFiles);
        }
        
        [Fact]
        public void GetWithMediaFiles_GetSinglePerformerByIdWithMediaFiles_GetsPerformer() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            int idToGet = 3;
            string expectedName = "Sabrina Impacciatore";
            string expectedMediaName1 = "The White Lotus - 2.01 - Ciao.mkv";

            // Act

            var performer = service.GetWithMediaFiles(idToGet);
            // Assert
            Assert.NotNull(performer);
            Assert.Equal(expectedName, performer.Name);

            Assert.Collection(performer.MediaFiles,
                m => Assert.Equal(expectedMediaName1, m.Name)
                );
        }

        [Fact]
        public void GetWithMediaFiles_GetSinglePerformerByNameWithMediaFiles_GetsPerformer() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            string performerNameToGet = "aubrey plaza";
            string expectedName = "Aubrey Plaza";
            string expectedMediaName1 = "The White Lotus - 2.01 - Ciao.mkv";

            // Act

            var performer = service.GetWithMediaFiles(performerNameToGet);
            // Assert
            Assert.NotNull(performer);
            Assert.Equal(expectedName, performer.Name);

            Assert.Collection(performer.MediaFiles,
                m => Assert.Equal(expectedMediaName1, m.Name)
                );
        }

        [Fact]
        public void GetWithMissingHeadshots_GetsPerformersThatDoNotHaveHeadshots() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            int expectedCount = 3;
            string expectedName1 = "Zazie Beetz";
            string expectedName2 = "Heidi Gardner";
            string expectedName3 = "Jordan Peele";

            // Act

            // returned in reverse Id order
            var performers = service.GetWithMissingHeadshots();

            // Assert
            Assert.NotEmpty(performers);
            Assert.Equal(expectedCount, performers.Count());

            Assert.Collection(performers,
                m => Assert.Equal(expectedName1, m.Name),
                m => Assert.Equal(expectedName2, m.Name),
                m => Assert.Equal(expectedName3, m.Name)
                );
        }

        [Fact]
        public void Add_AddPerformerObjectNotInDb_AddsPerformer() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            Performer performerToAdd = new("F. Murray Abraham", "f_murray_abraham.jpg");

            string expectedHeadshotUrl = "f_murray_abraham.jpg";
            // Act
            var result = service.Add(performerToAdd);
            var performerRetrieved = service.GetWithMediaFiles("f. murray abraham");
            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedHeadshotUrl, performerRetrieved.HeadshotUrls.First().Url);
            Assert.Empty(performerRetrieved.MediaFiles);
        }

        [Fact]
        public void Add_AddPerformerObjectAlreadyInDb_ReturnsFalseResult() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            Performer performerToAdd = new("Aubrey Plaza", "aubrey_plaza99.jpg");

            string expectedResultMessage = "Performer Aubrey Plaza already exists in database";

            // when retrieving the Performer, it should contain the properties that were 
            // already in the DB:
            string expectedHeadshotUrl = "aubrey_plaza.jpg";
            string expectedMediaName1 = "The White Lotus - 2.01 - Ciao.mkv";

            // Act
            var result = service.Add(performerToAdd);
            var performerRetrieved = service.GetWithMediaFiles("aubrey plaza");
            // Assert
            Assert.False(result.Success);
            Assert.Equal(expectedResultMessage, result.Message);

            Assert.Equal(expectedHeadshotUrl, performerRetrieved.HeadshotUrls.First().Url);

            Assert.Collection(performerRetrieved.MediaFiles,
                m => Assert.Equal(expectedMediaName1, m.Name)
                );
        }

        [Fact]
        public void Add_AddPerformerStringNotInDb_AddsPerformer() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            string performerStringToAdd = "Michael Imperioli";
            string expectedPerformerName = performerStringToAdd;
            // Act
            var result = service.Add(performerStringToAdd);
            var performerRetrieved = service.GetWithMediaFiles("michael imperioli");
            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedPerformerName, performerRetrieved.Name);
            Assert.Empty(performerRetrieved.MediaFiles);
        }
        [Fact]
        public void Add_AddPerformerStringAlreadyInDb_ReturnsFalseResult() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new PerformerService(context);

            string performerStringToAdd = "Aubrey Plaza";

            string expectedResultMessage = "Performer Aubrey Plaza already exists in database";

            // when retrieving the Performer, it should contain the properties that were 
            // already in the DB:
            string expectedHeadshotUrl = "aubrey_plaza.jpg";
            string expectedMediaName1 = "The White Lotus - 2.01 - Ciao.mkv";

            // Act
            var result = service.Add(performerStringToAdd);
            var performerRetrieved = service.GetWithMediaFiles("aubrey plaza");
            // Assert
            Assert.False(result.Success);
            Assert.Equal(expectedResultMessage, result.Message);

            Assert.Equal(expectedHeadshotUrl, performerRetrieved.HeadshotUrls.First().Url);

            Assert.Collection(performerRetrieved.MediaFiles,
                m => Assert.Equal(expectedMediaName1, m.Name)
                );
        }
    }
}
