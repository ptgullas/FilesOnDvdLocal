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
    public class LegacySeriesServiceTests {
        const string pathToLegacyDb = @"Data Source=C:\temp\FilesOnDvd\MigrationToSqlLite\LegacyMediaFilesOnDvd.db";

        private readonly DbConnection _modernConnection;
        private readonly DbContextOptions<MediaFilesContext> _modernContextOptions;

        #region ConstructorAndDispose

        public LegacySeriesServiceTests() {
            _modernConnection = new SqliteConnection("Filename=:memory:");
            _modernConnection.Open();

            _modernContextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_modernConnection)
                .Options;
            // create the schema & seed some data
            using var modernContext = new MediaFilesContext(_modernContextOptions);
            if (modernContext.Database.EnsureCreated()) {
            }

            var genres = new FileGenre[] {
                new FileGenre { Name = "Music" },
                new FileGenre { Name = "Music Video" },
                new FileGenre { Name = "Porn" },
                new FileGenre { Name = "Wrestling" },
                new FileGenre { Name = "TV Show" },
                new FileGenre { Name = "Film" },
                new FileGenre { Name = "Videoclip" },
                new FileGenre { Name = "Podcast" },
                new FileGenre { Name = "MMA" },
                new FileGenre { Name = "Shoot interview" },
                new FileGenre { Name = "Boxing" },
                new FileGenre { Name = "Web series" }
            };
            var publishers = new SeriesPublisher[] {
                new SeriesPublisher { Name = "Brazzers" },
                new SeriesPublisher { Name = "RealityKings" },
                new SeriesPublisher { Name = "Elegant Angel" },
                new SeriesPublisher { Name = "Naughty America" },
                new SeriesPublisher { Name = "Bangbros" },
                new SeriesPublisher { Name = "World Wrestling Entertainment" },
                new SeriesPublisher { Name = "Impact Wrestling" },
                new SeriesPublisher { Name = "F4WOnline.com" },
                new SeriesPublisher { Name = "Geek & Sundry" },
                new SeriesPublisher { Name = "Mofos" },
                new SeriesPublisher { Name = "Nuru" },
                new SeriesPublisher { Name = "Combat Zone" },
                new SeriesPublisher { Name = "CumLouder" },
                new SeriesPublisher { Name = "DDF" },
                new SeriesPublisher { Name = "TeamSkeet" },
                new SeriesPublisher { Name = "Twistys" }
            };

            modernContext.FileGenres.AddRange(genres);
            modernContext.SeriesPublishers.AddRange(publishers);
            modernContext.SaveChanges();
        }

        public void Dispose() => _modernConnection.Dispose();

        #endregion

        [Fact]
        public void Get_ReturnsAllSeries() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacySeriesService legacySeriesService = new(context);

            int seriesIdToRetrieve = 45;
            int expectedSeriesCount = 137;
            string expectedSeriesName = "Wrestling Observer Live";
            string expectedPublisherName = "F4WOnline.com";
            string expectedGenreName = "Podcast";
            // Act

            var results = legacySeriesService.Get();

            var singleResult = results.First(i => i.Id == seriesIdToRetrieve);

            // Assert

            Assert.Equal(expectedSeriesCount, results.Count());
            Assert.Equal(expectedSeriesName, singleResult.Name);
            Assert.Equal(expectedPublisherName, singleResult.Publisher.Name);
            Assert.Equal(expectedGenreName, singleResult.Genre.Name);
        }

        [Fact]
        public void MigrateToNewSeries_CreatesNewSeriesItems() {
            // Arrange
            var options = new DbContextOptionsBuilder<LegacyMediaFilesContext>()
                .UseSqlite(pathToLegacyDb)
                .Options;
            var context = new LegacyMediaFilesContext(options);

            LegacySeriesService legacySeriesService = new(context);

            using var modernContext = new MediaFilesContext(_modernContextOptions);
            FileGenreService fileGenreService = new(modernContext);
            SeriesPublisherService seriesPublisherService = new(modernContext);

            int expectedNewSeriesCount = 137;
            string seriesNameToRetrieve = "Wrestling Observer Live";
            string expectedPublisherName = "F4WOnline.com";
            string expectedGenreName = "Podcast";
            // Act

            var legacySeriesItems = legacySeriesService.Get();
            var newSeriesItems = legacySeriesService.MigrateToNewSeries(legacySeriesItems, seriesPublisherService, fileGenreService);

            modernContext.Series.AddRange(newSeriesItems);
            modernContext.SaveChanges();

            // Assert
            SeriesService seriesService = new(modernContext);
            var newSeriesFromService = seriesService.Get();

            var newSeriesItem = newSeriesFromService.First(s => s.Name == seriesNameToRetrieve);

            Assert.Equal(expectedNewSeriesCount, newSeriesFromService.Count());

            Assert.Equal(expectedPublisherName, newSeriesItem.Publisher?.Name);
            Assert.Equal(expectedGenreName, newSeriesItem.FileGenre.Name);

        }
    }
}
