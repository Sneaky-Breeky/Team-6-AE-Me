using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using DAMBackend.Models;
using DAMBackend.SubmissionEngine;
using File = System.IO.File;

namespace backendTests.SubmissionEngineTests 
{   
    // for before/after all
    public class DatabaseFixture : IDisposable
    {
        public SubmissionEngine submissionEngine;
        public DatabaseFixture()
        {
            // Setup code (runs once before all tests)
            Console.WriteLine("BeforeAll: Database setup");
            // Arrange
            
            // Act
            submissionEngine = new SubmissionEngine();
        }

        public void Dispose()
        {
            // Teardown code (runs once after all tests)
            Console.WriteLine("AfterAll: Database cleanup");
        }
    }

    // for mock file testing (no need to use actual photo/video for some testing purpose)
    public class MockFormFile : IFormFile
    {

        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }

        public Stream OpenReadStream()
        {
            return new MemoryStream(new byte[Length]);
        }

        public void CopyTo(Stream target)
        {
            OpenReadStream().CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return OpenReadStream().CopyToAsync(target, cancellationToken);
        }
    }

    // to wrap actual image with Formfile for testing purpose
    public static class FileHelper
    {
        public static IFormFile GetTestFormFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open);

            var formFile = new FormFile(fileStream, 0, fileStream.Length, "files", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" // You can set the content type accordingly
            };

            return formFile;
        }
    }

    // for list of IFormFile
    public class FormFileCollection : List<IFormFile>, IFormFileCollection
    {
        public IFormFile this[string name] => this.FirstOrDefault(f => f.Name == name);

        public IFormFile GetFile(string name)
        {
            return this.FirstOrDefault(f => f.Name == name);
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return this.Where(f => f.Name == name).ToList();
        }
    }

    public class SubmissionEngineTests : IClassFixture<DatabaseFixture>
    //, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly DatabaseFixture _fixture;
        private readonly string _uploadPath = "../../../TestOutput"; //hard coded value

        public SubmissionEngineTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        // [Fact]
        // public async Task UploadFiles_AcceptsUpTo100Files()
        // {
        //     // Arrange
        //     var mockFiles = new List<IFormFile>();

        //     for (int i = 0; i < 100; i++)
        //     {
        //         var fileMock = new MockFormFile
        //         {
        //             FileName = $"file{i}.jpg",
        //             Length = 1024 // 1 KB per file
        //         };
        //         mockFiles.Add(fileMock);
        //     }

        //     // Act
        //     var uploadedFiles = await _fixture.submissionEngine.UploadFiles(mockFiles);

        //     // Assert
        //     Assert.Equal(100, uploadedFiles.Count);
        //     Assert.Contains("file0.jpg", uploadedFiles);
        //     Assert.Contains("file99.jpg", uploadedFiles);

        //     // Clean up
        //     foreach (var file in uploadedFiles)
        //     {
        //         File.Delete(Path.Combine(_uploadPath, file));
        //     }
        // }

        // [Fact]
        // public async Task UploadFiles_RejectsMoreThan100Files()
        // {
        //     // Arrange
        //     var mockFiles = new List<IFormFile>();

        //     for (int i = 0; i < 101; i++)
        //     {
        //         var fileMock = new MockFormFile
        //         {
        //             FileName = $"file{i}.jpg",
        //             Length = 1024
        //         };
        //         mockFiles.Add(fileMock);
        //     }

        //     // Act & Assert
        //     var exception = await Assert.ThrowsAsync<Exception>(() => _fixture.submissionEngine.UploadFiles(mockFiles));
        //     Assert.Equal("You can upload a maximum of 100 files at once.", exception.Message);
        // }

        // [Fact]
        // public async Task UploadFiles_RejectsFilesOver500MB()
        // {
        //     // Arrange
        //     var mockFiles = new List<IFormFile>
        //     {
        //         new MockFormFile
        //         {
        //             FileName = "largefile.jpg",
        //             Length = 500 * 1024 * 1024 + 1 // 500MB + 1 byte
        //         }
        //     };

        //     // Act & Assert
        //     var exception = await Assert.ThrowsAsync<Exception>(() => _fixture.submissionEngine.UploadFiles(mockFiles));
        //     Assert.Equal("File largefile.jpg exceeds the maximum allowed size.", exception.Message);
        // }

        // [Fact]
        // public async Task UploadFiles_RejectsUnsupportedFileType()
        // {
        //     // Arrange
        //     var mockFiles = new List<IFormFile>
        //     {
        //         new MockFormFile
        //         {
        //             FileName = "unsupported.mov",
        //             Length = 1024
        //         }
        //     };

        //     // Act & Assert
        //     var exception = await Assert.ThrowsAsync<Exception>(() =>_fixture.submissionEngine.UploadFiles(mockFiles));
        //     Assert.Equal("File unsupported.mov has an unsupported file type.", exception.Message);
        // }

        // [Fact]
        // public async Task UploadFiles_SavesFilesToCorrectDirectory()
        // {
        //     // Arrange
        //     var testFiles = new List<string>
        //     {
        //         "DSC05589.ARW", "DSC03135.JPG", "yeti_classic.png", "DSC03135.ARW", 
        //         "DSC04569.ARW", "image1.jpeg", "jpgsample.JPG", "C0004.MP4"
        //     };

        //     var mockFiles = new List<IFormFile>();

        //     foreach (var file in testFiles)
        //     {
        //         var filePath = Path.Combine("../../../TestFiles", file);
        //         var testFile = FileHelper.GetTestFormFile(filePath);
        //         mockFiles.Add(testFile);
        //     }

        //     // Act
        //     var uploadedFiles = await _fixture.submissionEngine.UploadFiles(mockFiles);

        //     // Assert
        //     foreach (var file in testFiles)
        //     {
        //         var savedPath = Path.Combine(_uploadPath, file);
        //         Assert.True(File.Exists(savedPath), $"File {file} was not found in the upload directory.");
        //     }

        //     // Clean up
        //     foreach (var file in uploadedFiles)
        //     {
        //         File.Delete(Path.Combine(_uploadPath, file));
        //     }
        // }
        // // ---------------------------- Compression Test Start Here ------------------------------
        // [Fact]
        // public async Task UploadJpgPng_InvalidFormat_ThrowsError()
        // {
            
        //     // Arrange
        //     var filePath = Path.Combine("../../../TestFiles", "DSC03135.ARW");
        //     var testFile = FileHelper.GetTestFormFile(filePath);

        //     // Act & Assert
        //     var exception = await Assert.ThrowsAsync<Exception>( () =>
        //          _fixture.submissionEngine.UploadJpgPng(testFile, CompressionLevel.Medium));
        //     Assert.Equal("Only JPG or PNG files are allowed.", exception.Message);
        // }


        // [Theory]
        // [InlineData(CompressionLevel.Low)]
        // [InlineData(CompressionLevel.Medium)]
        // [InlineData(CompressionLevel.High)]
        // public async Task UploadJpgPng_ValidJpgCompression_Success(CompressionLevel compressionLevel)
        // {
        //     // Arrange
        //     var filePath = Path.Combine("../../../TestFiles", "DSC03135.JPG");
        //     var testFile = FileHelper.GetTestFormFile(filePath);
        //     // Act
        //     string outputFile = await  _fixture.submissionEngine.UploadJpgPng(testFile, compressionLevel);
            
        //     // Assert
        //     Assert.NotNull(outputFile);
        //     Assert.Equal("DSC03135.JPG", outputFile);
        // }

        [Theory]
        [InlineData(CompressionLevel.Low)]
        [InlineData(CompressionLevel.Medium)]
        [InlineData(CompressionLevel.High)]
        public async Task UploadJpgPng_ValidPngCompression_Success(CompressionLevel compressionLevel)
        {
            // Arrange
            var filePath = Path.Combine("../../../TestFiles", "yeti_classic.png");
            var testFile = FileHelper.GetTestFormFile(filePath);
            // Act
            var outputFile = await  _fixture.submissionEngine.UploadJpgPng(testFile, compressionLevel);
            

            // Assert
            Assert.NotNull(outputFile);
            if (compressionLevel == CompressionLevel.Low)
            {
                Assert.Equal(9884, outputFile.Length);
            } 
            else if (compressionLevel == CompressionLevel.Medium)
            {
                Assert.Equal(38816, outputFile.Length);
            }
            else
            {
                Assert.Equal(80321, outputFile.Length);
            }
        }

        [Theory]
        [InlineData(CompressionLevel.Low)]
        [InlineData(CompressionLevel.Medium)]
        [InlineData(CompressionLevel.High)]
        public async Task UploadMp4_ValidMp4Compression_Success(CompressionLevel compressionLevel)
        {
            // Arrange
            var filePath = Path.Combine("../../../TestFiles", "C0004.MP4");
            var testFile = FileHelper.GetTestFormFile(filePath);
            // Act
            var outputFile = await  _fixture.submissionEngine.UploadMp4(testFile, compressionLevel);

            // Assert
            Assert.NotNull(outputFile);
            if (compressionLevel == CompressionLevel.Low)
            {
                Assert.Equal(17232867, outputFile.Length);
            } 
            else if (compressionLevel == CompressionLevel.Medium)
            {
                Assert.Equal(36407348, outputFile.Length);
            }
            else
            {
                Assert.Equal(83613564, outputFile.Length);
            }
        }

        [Theory]
        [InlineData(CompressionLevel.Low)]
        [InlineData(CompressionLevel.Medium)]
        [InlineData(CompressionLevel.High)]
        public async Task UploadRaw_ValidRawCompression_Success(CompressionLevel compressionLevel)
        {
            // Arrange
            var filePath = Path.Combine("../../../TestFiles", "DSC03135.ARW");
            var testFile = FileHelper.GetTestFormFile(filePath);
            // Act
            var outputFile = await  _fixture.submissionEngine.UploadRaw(testFile, compressionLevel);

            // Assert
            Assert.NotNull(outputFile);
            if (compressionLevel == CompressionLevel.Low)
            {
                Assert.Equal(240472, outputFile.Length);
            } 
            else if (compressionLevel == CompressionLevel.Medium)
            {
                Assert.Equal(490166, outputFile.Length);
            }
            else
            {
                Assert.Equal(24791808, outputFile.Length);
            }
        }




        // ---------------------------- Compression Test Finish Here ------------------------------

        [Fact]
        public void ProcessImageMetadata_WithRealImage_ReturnsPopulatedModel()
        {
            // Arrange
            var filePath = Path.Combine("../../../TestFiles", "image1.jpeg");
            var testFile = FileHelper.GetTestFormFile(filePath);

            var dummyUser = new UserModel
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashed",
                Role = Role.User,
                Status = true
            };

            string tempBasePath = "palette/user1";

            // Act
            var result = _fixture.submissionEngine.ProcessImageMetadataJpgPng(testFile, tempBasePath, dummyUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dummyUser.Id, result.UserId);
            Assert.Equal( "image1", result.Name);
            Assert.Equal( ".jpeg", result.Extension);
            Assert.Equal( 4032, result.PixelWidth);
            Assert.Equal( 3024, result.PixelHeight);
            Assert.Equal( 1, result.UserId);
            Assert.Equal("palette/user1/thumbnails/image1.jpeg", result.ThumbnailPath);
            Assert.Equal( "palette/user1/originals/image1.jpeg", result.OriginalPath);
            Assert.Equal("palette/user1/views/image1.jpeg", result.ViewPath);
            Assert.Equal(null, result.DateTimeOriginal);
            Assert.Equal("Apple", result.Make);
            Assert.Equal("iPhone 13 Pro Max", result.Model);
            Assert.Equal((int) 5, result.FocalLength);
            Assert.Equal((float) 1.5, result.Aperture);
            Assert.Equal(null, result.Copyright);
        }
        
        [Fact]
        public void ProcessImageMetadataPng_WithRealImage_ReturnsPopulatedModel()
        {
            // Arrange
            var filePath = Path.Combine("../../../TestFiles", "yeti_classic.png");
            var testFile = FileHelper.GetTestFormFile(filePath);

            var dummyUser = new UserModel
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashed",
                Role = Role.User,
                Status = true
            };

            string tempBasePath = "palette/user1";

            // Act
            var result = _fixture.submissionEngine.ProcessImageMetadataJpgPng(testFile, tempBasePath, dummyUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dummyUser.Id, result.UserId);
            Assert.Equal( "yeti_classic", result.Name);
            Assert.Equal( ".png", result.Extension);
            Assert.Equal( 271, result.PixelWidth);
            Assert.Equal( 665, result.PixelHeight);
            Assert.Equal( 1, result.UserId);
            Assert.Equal("palette/user1/thumbnails/yeti_classic.png", result.ThumbnailPath);
            Assert.Equal( "palette/user1/originals/yeti_classic.png", result.OriginalPath);
            Assert.Equal("palette/user1/views/yeti_classic.png", result.ViewPath);
            Assert.Equal(null, result.DateTimeOriginal);
            Assert.Equal(null, result.Make);
            Assert.Equal(null, result.Model);
            Assert.Equal(null, result.FocalLength);
            Assert.Equal(null, result.Aperture);
            Assert.Equal(null, result.Copyright);
        }

        [Fact]
        public async Task exifTest()
        {
            // Arrange
            string[] filePath =
            [
                "DSC05589.ARW", "DSC03135.JPG", "yeti_classic.png", "DSC03135.ARW", "DSC04569.ARW", "image1.jpeg",
                "jpgsample.JPG", "jpgsample.JPG"
            ];

            var testImagePath = Path.Combine("../../../TestFiles", filePath[5]);
            // Act
            _fixture.submissionEngine.PrintImageMetadata(testImagePath);
        }
    }
}