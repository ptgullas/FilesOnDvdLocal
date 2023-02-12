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

            var mediaFiles = new MediaFile[] {
                new MediaFile {
                    Name = "The White Lotus - 1.01 - Arrivals.mkv",
                    Performers = new List<Performer> {connie, jennifer},
                    Notes = "Series premiere of this show about rich ppl"
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

            context.MediaFiles.AddRange(mediaFiles);
            context.SaveChanges();
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
        public void Get_GetByName_RetrievesMediaFile() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new MediaFileService(context);

            string expectedNotes = "Series premiere of this show about rich ppl";
            Performer expectedPerformer1 = context.Performers.First(p => p.Name == "Connie Britton");
            Performer expectedPerformer2 = context.Performers.First(p => p.Name == "Jennifer Coolidge");
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
            // Act
            var result = service.Get(mediaFileIdToGet);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedName, result.Name);
            Assert.Collection(result.Performers,
                p => Assert.Equal(expectedPerformer1, p)
                );
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

        // eventually will want a GetMediaFilesBySeries? Or would that go in a SeriesService?
    }
}
