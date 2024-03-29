﻿using MediaFilesOnDvd.Data;
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
    public class MediaFileServiceTests : IDisposable {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<MediaFilesContext> _contextOptions;

        #region ConstructorAndDispose

        public MediaFileServiceTests() {
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
                new Performer {Name = "Connie Britton"},
                new Performer {Name = "Jennifer Coolidge"},
                new Performer {Name = "Cecily Strong"},
                new Performer {Name = "Keegan-Michael Key"},
                new Performer {Name = "John Cena"},
                new Performer {Name = "Jeremy Renner"},
                new Performer {Name = "Kenan Thompson"},
                new Performer {Name = "Jordan Peele"},
                new Performer {Name = "Heidi Gardner"}
            };
            context.Performers.AddRange(performers);
            context.SaveChanges();

            var connie = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Connie"));
            var jennifer = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jennifer"));
            var cecily = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Cecily"));
            var kenan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Kenan"));
            var keegan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Keegan"));
            var john = context.Performers.FirstOrDefault(p => p.Name.StartsWith("John"));
            var jeremy = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jeremy"));
            var jordan = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Jordan"));
            var heidi = context.Performers.FirstOrDefault(p => p.Name.StartsWith("Heidi"));

#pragma warning disable CS8604 // Possible null reference argument.
            var mediaFiles = new MediaFile[] {
                new MediaFile("The White Lotus - 1.01 - Arrivals.mkv",
                    new List<Performer> {connie, jennifer},
                    "Series premiere of this show about rich ppl",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "01.jpg"},
                        new ScreenshotUrl() { Url = "01a.jpg"}
                    })
                ,
                new MediaFile("Schmigadoon! - 1.06 - How We Change.mkv",
                    new List<Performer> {cecily, keegan},
                    "Fun musical show",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "02.jpg"},
                    }),
                new MediaFile("Peacemaker - 1.01 - A Whole New Whirled.mkv",
                    new List<Performer> {john},
                    "Follow-up to James Gunn's 'The Suicide Squad'",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "03.jpg"},
                    }),
                new MediaFile("Hawkeye (2021) - 1.04 - Partners, Am I Right.mkv",
                    new List<Performer> {jeremy},
                    "MCU show on Disney+",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "04.jpg"},
                    }),
                new MediaFile("The Hurt Locker (2008).avi",
                    new List<Performer> {jeremy},
                    "Kathryn Bigelow film about effects of war or whatever",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "05.jpg"},
                    }),
                new MediaFile("Saturday Night Live - 44.01 - Adam Driver",
                    new List<Performer> {cecily, kenan, heidi},
                    "That Star Wars Undercover Boss sketch",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "06.jpg"},
                    }),
                new MediaFile("Key & Peele - 1.01",
                    new List<Performer> {keegan, jordan},
                    "Series premiere of great sketch show",
                    new List<ScreenshotUrl> {
                        new ScreenshotUrl() { Url = "07.jpg"},
                        new ScreenshotUrl() { Url = "07a.jpg"}
                    })
            };
#pragma warning restore CS8604 // Possible null reference argument.

            // context.MediaFiles.AddRange(mediaFiles);

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
            // miscTV2023a.Files.Add(mediaFiles[0]);

            DiscService discService = new(context);
            discService.AddToWallet(miscTV2023a, tvWallet);
        }

        public void Dispose() => _connection.Dispose();

        private static string GetTestDbPath() {
            var folder = Environment.SpecialFolder.Personal;
            var path = Environment.GetFolderPath(folder);
            path = Path.Combine(path, "MediaFilesOnDvd");
            return Path.Join(path, "MediaFilesOnDvdTest.db");
        }

        private static DbContextOptions<MediaFilesContext> CreateDbContextOptions() {
            string dbPath = GetTestDbPath();
            return new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
        }
        #endregion

        [Fact]
        public void Add_UseConstructor_AllPropertiesCorrectlyAdded() {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Get_GetByName_RetrievesMediaFile() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);

            string expectedNotes = "Series premiere of this show about rich ppl";
            Performer expectedPerformer1 = context.Performers.First(p => p.Name == "Connie Britton");
            Performer expectedPerformer2 = context.Performers.First(p => p.Name == "Jennifer Coolidge");
            string expectedDiscName = "MiscTV2023-02-17";

            int expectedScreenshotCount = 2;
            string expectedScreenshotUrl = "01.jpg";
            // Act
            var results = service.Get("The White Lotus - 1.01 - Arrivals.mkv");

            // Assert

            Assert.Single(results);
            var result = results.First();
            Assert.Collection(result.Performers,
                p => Assert.Equal(expectedPerformer1, p),
                p => Assert.Equal(expectedPerformer2, p)
                );
            Assert.Equal(expectedNotes, result.Notes);
            Assert.Equal(expectedDiscName, result.Disc.Name);
            Assert.Equal(expectedScreenshotCount, result.Screenshots.Count);
            Assert.Equal(expectedScreenshotUrl, result.Screenshots.First().Url);
        }

        [Fact]
        public void Get_GetById_RetrievesMediaFile() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);

            // fun fact: This is a 1-based DB, meaning the first item has ID of 1
            int mediaFileIdToGet = 3;
            string expectedName = "Peacemaker - 1.01 - A Whole New Whirled.mkv";

            Performer expectedPerformer1 = context.Performers.First(p => p.Name == "John Cena");
            string expectedDiscName = "MiscTV2023-02-17";

            // Act
            var result = service.Get(mediaFileIdToGet);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedName, result.Name);
            Assert.Collection(result.Performers,
                p => Assert.Equal(expectedPerformer1, p)
                );
            Assert.Equal(expectedDiscName, result.Disc.Name);
        }

        [Fact]
        public void Get_RetrievesAllMediaFiles() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);

            int expectedCount = 7;
            string expectedName1 = "The White Lotus - 1.01 - Arrivals.mkv";
            string expectedName2 = "Schmigadoon! - 1.06 - How We Change.mkv";
            string expectedName3 = "Peacemaker - 1.01 - A Whole New Whirled.mkv";
            string expectedName4 = "Hawkeye (2021) - 1.04 - Partners, Am I Right.mkv";
            string expectedName5 = "The Hurt Locker (2008).avi";
            string expectedName6 = "Saturday Night Live - 44.01 - Adam Driver";
            string expectedName7 = "Key & Peele - 1.01";


            Performer expectedPerformer1 = context.Performers.First(p => p.Name == "John Cena");
            // Act
            var mediaFiles = service.Get();

            // Assert
            Assert.NotEmpty(mediaFiles);
            Assert.Equal(expectedCount, mediaFiles.Count());
            Assert.Collection(mediaFiles,
                m => Assert.Equal(expectedName1, m.Name),
                m => Assert.Equal(expectedName2, m.Name),
                m => Assert.Equal(expectedName3, m.Name),
                m => Assert.Equal(expectedName4, m.Name),
                m => Assert.Equal(expectedName5, m.Name),
                m => Assert.Equal(expectedName6, m.Name),
                m => Assert.Equal(expectedName7, m.Name)
                );
        }

        [Fact]
        public void AddPerformerToMediaFile_PerformerIsNotAlreadyInFile_AddsPerformer() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);
            var performerService = new PerformerService(context);

            string mediaFileNameToGet = "The White Lotus - 1.01 - Arrivals.mkv";

            string expectedName1 = "John Cena";
            int expectedPerformerCountBeforeRefresh = 1;
            int expectedPerformerCountAfterRefresh = 3;
            // Act
            var mediaFiles = service.Get();
            var whiteLotusPremiere = mediaFiles.FirstOrDefault(m => m.Name == mediaFileNameToGet);
            var cena = performerService.GetWithMediaFiles("John Cena");

            var result = service.AddPerformerToMediaFile(whiteLotusPremiere, cena);
            // Assert

            Assert.True(result.Success);

            // should test to see if the whiteLotusPremiere entity retrieved above gets updated, or if
            // we need to do another Get() from the context to get the updated one

            // The whiteLotusPremiere object has only 1 Performer (Cena), probably because the original
            // one taken from the context didn't Include Performers, so it only tracks the one that was added

            Assert.Equal(expectedPerformerCountBeforeRefresh, whiteLotusPremiere.Performers.Count);

            Assert.Contains(whiteLotusPremiere.Performers, p => p.Name == "John Cena");

            // Retrieve the MediaFile again, with the Performers
            var whiteLotusPremiereWithPerformers = service.Get(whiteLotusPremiere.Id);

            // even though we got another object, the original object is now updated with
            // this should have all 3 Performers
            Assert.Equal(expectedPerformerCountAfterRefresh, whiteLotusPremiere.Performers.Count);
            Assert.Equal(expectedPerformerCountAfterRefresh, whiteLotusPremiereWithPerformers.Performers.Count);

            Assert.Contains(whiteLotusPremiere.Performers, p => p.Name == "John Cena");

            Assert.Equal(2, cena.MediaFiles.Count);
        }

        [Fact]
        public void AddPerformerToMediaFile_PerformerIsAlreadyInFile_ReturnsError() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);
            var performerService = new PerformerService(context);

            string mediaFileNameToGet = "Saturday Night Live - 44.01 - Adam Driver";

            string expectedError = "MediaFile 'Saturday Night Live - 44.01 - Adam Driver' already contains Performer Cecily Strong";
            // Act
            var mediaFiles = service.Get();
            var snlAdamDriver = mediaFiles.FirstOrDefault(m => m.Name == mediaFileNameToGet);
            var cecily = performerService.GetWithMediaFiles("Cecily Strong");

            var result = service.AddPerformerToMediaFile(snlAdamDriver, cecily);
            // Assert

            Assert.False(result.Success);
            Assert.Equal(expectedError, result.Message);

        }
        // eventually will want a GetMediaFilesBySeries? Or would that go in a SeriesService?
    }
}
