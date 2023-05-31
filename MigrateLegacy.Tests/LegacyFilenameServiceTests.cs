using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrateLegacy.Services;
using System.Data.Common;

namespace MigrateLegacy.Tests {
    public class LegacyFilenameServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;

        #region ConstructorAndDispose

        public LegacyFilenameServiceTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;
            // create the schema & seed some data
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            if (modernContext.Database.EnsureCreated()) {
            }

            Wallet mediaWallet = new() {
                Id = 0,
                Name = "MoviesAndTV",
                Notes = "Movies & TV"
            };

            // create Discs 

            var discs = new Disc[] {
                new Disc { Name = "Mov2023-05-30" },
                new Disc { Name = "FreaksAndGeeks" },
            };
            // create FileGenres
            var fileGenres = new FileGenre[] {
                new FileGenre { Name = "Film" },
                new FileGenre { Name = "TV Show"}
            };

            mediaWallet.Discs.AddRange(discs);
            modernContext.Add(mediaWallet);
            modernContext.FileGenres.AddRange(fileGenres);
            modernContext.SaveChanges();
        }

        public void Dispose() => _modernConnection.Dispose();

        #endregion

        [Fact]
        public void Get_GetAll_ReturnsAllFilenames() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int expectedFilenameCount = 4711;

            // Act
            var results = service.Get();
            // Assert
            Assert.NotEmpty(results);
            Assert.Equal(expectedFilenameCount, results.Count());
        }

        [Fact]
        public void Get_GetById_ReturnsSingleFilename() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int fileNameIdToGet = 4377;
            string expectedFilename = "2012-09-24 WWE RAW - Layla & Alicia Fox vs. Eve & Beth Phoenix.mp4";

            // change these to the Genre & Series names when we add them
            int expectedGenreId = 4;
            string expectedGenreName = "Wrestling";
            int expectedSeriesId = 88;
            string expectedSeriesName = "WWE RAW";
            int expectedDiscId = 202;
            string expectedDiscName = "W2015-08-05b";
            // Act
            var result = service.Get(fileNameIdToGet);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFilename, result.Name);
            Assert.Equal(expectedGenreId, result.GenreId);
            Assert.Equal(expectedGenreName, result.Genre.Name);
            Assert.Equal(expectedSeriesId, result.SeriesId);
            Assert.Equal(expectedSeriesName, result.Series.Name);
            Assert.Equal(expectedDiscId, result.DiscId);
            Assert.Equal(expectedDiscName, result.Disc.Name);
        }

        [Fact]
        public void Get_GetById_ReturnsPerformers() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            int fileNameIdToGet = 1412;
            string expectedFilename = "Predator 3 - The Final Chapter (2009) - Scene 4 - India Summer & Abbey Brooks.avi";

            // change these to the Genre & Series names when we add them
            int expectedGenreId = 3;
            int expectedDiscId = 66;
            string expectedDiscName = "P2010-11-24a-LES";

            int expectedPerformerCount = 3;
            string expectedFirstPerformerName = "Abbey Brooks";
            // Act
            var result = service.Get(fileNameIdToGet);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFilename, result.Name);
            Assert.Equal(expectedGenreId, result.GenreId);
            Assert.Equal(expectedDiscId, result.DiscId);
            Assert.Equal(expectedDiscName, result.Disc.Name);

            Assert.Equal(expectedPerformerCount, result.Performers.Count);
            Assert.Equal(expectedFirstPerformerName, result.Performers.First().Name);
        }

        [Fact]
        public void GetByDisc_GetWrestlingDiscs_ReturnsWrestlingDiscs() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            string discNameToGet = "W2010-04-24";
            int expectedCount = 44;

            string expectedFirstFileName = "1984-01-23 - Hulk Hogan vs. The Iron Sheik.mp4";
            // Act

            var results = service.GetByDiscName(discNameToGet);
            // Assert

            Assert.NotEmpty(results);
            Assert.Equal(expectedCount, results.Count());
            Assert.Equal(expectedFirstFileName, results.First().Name);
        }

        [Fact]
        public void GetBySeries_GetPrisonBreakFiles_ReturnsFiles() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyFilenameService service = new(context);

            string seriesToGet = "Prison Break";
            int expectedCount = 12;

            string expectedFirstFileName = "Prison Break - 1.01 - Pilot.avi";
            // Act

            var results = service.GetBySeries(seriesToGet);
            // Assert

            Assert.NotEmpty(results);
            Assert.Equal(expectedCount, results.Count());
            Assert.Equal(expectedFirstFileName, results.First().Name);
        }

        [Fact]
        public void MigrateToMediaFiles_MigratesAllFiles() {
            // don't have to use production data for this test

            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacyGenre movieGenre = new() { Id = 6, Name = "Film" };
            LegacyGenre tvGenre = new() { Id = 5, Name = "TV Show" };
            LegacyDisc movieDisc = new() { Id = 40, Name = "Mov2023-05-30", Notes = "James Franco movies"};
            LegacyDisc freaksGeeksDisc = new() { Id = 41, Name = "FreaksAndGeeks", Notes = "Freaks & Geeks"};

            List<LegacyFilename> testLegacyFilenames = new() {
                new LegacyFilename() {Id = 23, Name = "This is the End.mkv", Genre = movieGenre, GenreId = 6, Disc = movieDisc, DiscId = 40},
                new LegacyFilename() {Id = 24, Name = "Freaks and Geeks - 1.01.mkv", Genre = tvGenre, GenreId = 5, Disc = freaksGeeksDisc, DiscId = 41},
                new LegacyFilename() {Id = 25, Name = "Spider-Man (2002).mp4", Genre = movieGenre, GenreId = 6, Disc = movieDisc, DiscId = 40}
            };

            using var modernContext = new MediaFilesContext(_modernContextOptions);
            DiscService discService = new(modernContext);
            FileGenreService fileGenreService = new(modernContext);

            var expectedMediaFiles = new MediaFile[] {
                new MediaFile("This is the End.mkv", null, "23.jpg"),
                new MediaFile("Freaks and Geeks - 1.01.mkv", null, "24.jpg"),
                new MediaFile("Spider-Man (2002).mp4", null, "25.jpg"),
            };

            string expectedMovieDiscName = "Mov2023-05-30";
            string expectedFreaksDiscName = "FreaksAndGeeks";
            string expectedFilmGenreName = "Film";
            string expectedTVGenreName = "TV Show";

            LegacyFilenameService legacyFilenameService = new(context);

            // Act

            var result = legacyFilenameService.MigrateToMediaFiles(testLegacyFilenames, discService, fileGenreService);
            modernContext.MediaFiles.AddRange(result);
            modernContext.SaveChanges();
            // Assert

            MediaFileService mfService = new(modernContext);
            var newMediaFiles = mfService.Get();

            Assert.Collection(newMediaFiles,
                m => {
                    Assert.Equal(expectedMediaFiles[0].Name, m.Name);
                    Assert.Equal(expectedMediaFiles[0].Screenshots.First().Url, m.Screenshots.First().Url);
                    Assert.Equal(expectedMovieDiscName, m.Disc.Name);
                    Assert.Equal(expectedFilmGenreName, m.FileGenre.Name);
                },
                m => {
                    Assert.Equal(expectedMediaFiles[1].Name, m.Name);
                    Assert.Equal(expectedMediaFiles[1].Screenshots.First().Url, m.Screenshots.First().Url);
                    Assert.Equal(expectedFreaksDiscName, m.Disc.Name);
                    Assert.Equal(expectedTVGenreName, m.FileGenre.Name);
                },
                m => {
                    Assert.Equal(expectedMediaFiles[2].Name, m.Name);
                    Assert.Equal(expectedMediaFiles[2].Screenshots.First().Url, m.Screenshots.First().Url);
                    Assert.Equal(expectedMovieDiscName, m.Disc.Name);
                    Assert.Equal(expectedFilmGenreName, m.FileGenre.Name);
                }
                );
                
        }

    }
}
