using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVParsedDataRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Services
{
    public class CvParsingServiceTests
    {
        private readonly Mock<IFileTextExtractor> _mockExtractor = new();
        private readonly Mock<IGeminiClient> _mockGemini = new();
        private readonly Mock<ICVParsedDataRepository> _mockRepo = new();
        private readonly CvParsingService _service;
        public CvParsingServiceTests()
        {
            _service = new CvParsingService(_mockExtractor.Object, _mockGemini.Object, _mockRepo.Object);
        }
        private string ValidJson => @"
    {
      ""FullName"": ""Nguyen Van A"",
      ""Email"": ""test@example.com"",
      ""Phone"": ""123456789"",
      ""YearsOfExperience"": 3.5,
      ""Skills"": ""C#, SQL"",
      ""EducationLevel"": ""Bachelor"",
      ""JobTitles"": ""Developer"",
      ""Languages"": ""English"",
      ""Certifications"": ""Azure"",
      ""ParsedAt"": ""2025-08-12T15:30:00Z"",
      ""Address"": ""Hanoi"",
      ""Summary"": ""Good dev"",
      ""WorkHistory"": ""Company A"",
      ""Projects"": ""Project 1"",
      ""isDelete"": false
    }";

        [Fact]
        public async Task ParseAndSaveAsync_HappyPath_ReturnsEntityAndCallsRepo()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("some text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ValidJson);

            CvparsedDatum result = await _service.ParseAndSaveAsync(123, fileMock.Object, null);

            Assert.NotNull(result);
            Assert.Equal(123, result.CvId);
            Assert.False(result.IsDelete);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<CvparsedDatum>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ParseAndSaveAsync_UsesDefaultPrompt_WhenPromptIsNull()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.Is<string>(p => p.Contains("Bạn là trình phân tích CV")),
                                                      It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ValidJson);

            await _service.ParseAndSaveAsync(1, fileMock.Object, null);

            _mockGemini.VerifyAll();
        }

        [Fact]
        public async Task ParseAndSaveAsync_UsesCustomPrompt_WhenPromptProvided()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync("custom prompt", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ValidJson);

            await _service.ParseAndSaveAsync(1, fileMock.Object, "custom prompt");

            _mockGemini.VerifyAll();
        }

        [Fact]
        public async Task ParseAndSaveAsync_InvalidJson_ThrowsInvalidOperationException()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync("not a valid json");

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ParseAndSaveAsync(1, fileMock.Object, null));
        }

        [Fact]
        public async Task ParseAndSaveAsync_ExtractorThrows_PropagatesException()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("extract fail"));

            await Assert.ThrowsAsync<Exception>(() =>
                _service.ParseAndSaveAsync(1, fileMock.Object, null));
        }

        [Fact]
        public async Task ParseAndSaveAsync_RepoThrows_PropagatesException()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ValidJson);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<CvparsedDatum>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("db fail"));

            await Assert.ThrowsAsync<Exception>(() =>
                _service.ParseAndSaveAsync(1, fileMock.Object, null));
        }

        [Fact]
        public async Task ParseAndSaveAsync_JsonWithNullFields_ShouldDeserializeCorrectly()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            var jsonWithNulls = ValidJson.Replace("\"Azure\"", "null");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(jsonWithNulls);

            var result = await _service.ParseAndSaveAsync(1, fileMock.Object, null);

            Assert.Null(result.Certifications);
        }

        [Fact]
        public async Task ParseAndSaveAsync_JsonWithExtraField_ShouldIgnoreExtraField()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            var jsonWithExtra = ValidJson.Insert(ValidJson.Length - 1, ", \"ExtraField\": \"ignored\"");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(jsonWithExtra);

            var result = await _service.ParseAndSaveAsync(1, fileMock.Object, null);

            Assert.Equal("Nguyen Van A", result.FullName);
        }

        [Fact]
        public async Task ParseAndSaveAsync_JsonWithWrongDataType_ShouldThrow()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            var badJson = ValidJson.Replace("3.5", "\"five\"");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(badJson);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ParseAndSaveAsync(1, fileMock.Object, null));
        }

        [Fact]
        public async Task ParseAndSaveAsync_EmptyJson_ShouldReturnEntityWithNullFields()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync("{}");

            var result = await _service.ParseAndSaveAsync(1, fileMock.Object, null);

            Assert.Equal(1, result.CvId);
            Assert.False(result.IsDelete);
        }

        [Fact]
        public async Task ParseAndSaveAsync_RepoThrowsTaskCanceled_ShouldPropagate()
        {
            var fileMock = new Mock<IFormFile>();
            _mockExtractor.Setup(x => x.ExtractTextAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                          .ReturnsAsync("text");
            _mockGemini.Setup(x => x.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(ValidJson);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<CvparsedDatum>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new TaskCanceledException());

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _service.ParseAndSaveAsync(1, fileMock.Object, null));
        }
    }
}
