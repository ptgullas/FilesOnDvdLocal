using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace MediaFilesOnDvd.Tests {
    public class FileTagServiceTests : IDisposable {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<MediaFilesContext> _contextOptions;

        public FileTagServiceTests() {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<MediaFilesContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new MediaFilesContext(_contextOptions);
            context.Database.EnsureCreated();

            var tags = new List<FileTag> {
                new FileTag("Cinematic"),
                new FileTag("Action"),
                new FileTag("Comedy")
            };

            context.FileTags.AddRange(tags);
            context.SaveChanges();
        }

        public void Dispose() => _connection.Dispose();

        [Fact]
        public void Get_ReturnsAllTagsInOrder() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new FileTagService(context);

            // Act
            var result = service.Get().ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Action", result[0].Name);
            Assert.Equal("Cinematic", result[1].Name);
            Assert.Equal("Comedy", result[2].Name);
        }

        [Fact]
        public void GetByName_ReturnsCorrectTag() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new FileTagService(context);

            // Act
            var result = service.Get("cinematic");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cinematic", result.Name);
        }

        [Fact]
        public void Add_NewTag_ReturnsSuccess() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new FileTagService(context);

            // Act
            var result = service.Add("Documentary");

            // Assert
            Assert.True(result.Success);
            Assert.Equal(4, context.FileTags.Count());
            Assert.NotNull(context.FileTags.FirstOrDefault(t => t.Name == "Documentary"));
        }

        [Fact]
        public void Add_DuplicateTag_ReturnsFailure() {
            // Arrange
            using var context = new MediaFilesContext(_contextOptions);
            var service = new FileTagService(context);

            // Act
            var result = service.Add("Cinematic");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("already exists", result.Message);
        }
    }
}
