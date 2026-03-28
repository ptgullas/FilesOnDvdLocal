using MediaFilesOnDvd.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO;
using Xunit;

namespace MediaFilesOnDvd.Tests
{
    public class PathResolverServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly PathResolverService _service;

        public PathResolverServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _service = new PathResolverService(_mockConfig.Object);
        }

        [Fact]
        public void GetPerformerHeadshotPath_ReturnsCorrectPath()
        {
            // Arrange
            string basePath = @"C:\Images\Headshots";
            string filename = "performer1.jpg";
            string expected = Path.Combine(basePath, filename);
            _mockConfig.Setup(c => c["ImagePaths:PerformerHeadshots"]).Returns(basePath);

            // Act
            var result = _service.GetPerformerHeadshotPath(filename);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetMediaScreenshotPath_ReturnsCorrectPath()
        {
            // Arrange
            string basePath = @"C:\Images\Screenshots";
            string filename = "movie1.jpg";
            string expected = Path.Combine(basePath, filename);
            _mockConfig.Setup(c => c["ImagePaths:MediaScreenshots"]).Returns(basePath);

            // Act
            var result = _service.GetMediaScreenshotPath(filename);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPerformerGalleryPath_ReturnsCorrectPath()
        {
            // Arrange
            string basePath = @"C:\Images\Gallery";
            string filename = "gallery1.jpg";
            string expected = Path.Combine(basePath, filename);
            _mockConfig.Setup(c => c["ImagePaths:PerformerGallery"]).Returns(basePath);

            // Act
            var result = _service.GetPerformerGalleryPath(filename);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPerformerHeadshotPath_HandlesMissingConfig()
        {
            // Arrange
            string filename = "performer1.jpg";
            _mockConfig.Setup(c => c["ImagePaths:PerformerHeadshots"]).Returns((string?)null);

            // Act
            var result = _service.GetPerformerHeadshotPath(filename);

            // Assert
            // Path.Combine("", "filename") results in "filename"
            Assert.Equal(filename, result);
        }
    }
}
