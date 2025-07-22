using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Services
{
    public class SavedJobServiceTests
    {
        private readonly Mock<ISavedJobRepository> _mockRepo;
        private readonly SavedJobService _service;

        public SavedJobServiceTests()
        {
            _mockRepo = new Mock<ISavedJobRepository>();
            _service = new SavedJobService(_mockRepo.Object);
        }

        [Fact]
        public async Task SaveJobAsync_NotAlreadySaved_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int jobPostId = 100;
            _mockRepo.Setup(r => r.GetByUserAndJobPostAsync(userId, jobPostId)).ReturnsAsync((SavedJob)null);

            // Act
            var result = await _service.SaveJobAsync(userId, jobPostId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.CreateAsync(It.Is<SavedJob>(
                s => s.UserId == userId && s.JobPostId == jobPostId)), Times.Once);
        }

        [Fact]
        public async Task SaveJobAsync_AlreadySaved_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int jobPostId = 100;
            _mockRepo.Setup(r => r.GetByUserAndJobPostAsync(userId, jobPostId)).ReturnsAsync(new SavedJob());

            // Act
            var result = await _service.SaveJobAsync(userId, jobPostId);

            // Assert
            Assert.False(result);
            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<SavedJob>()), Times.Never);
        }

        [Fact]
        public async Task IsJobSavedAsync_JobIsSaved_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int jobPostId = 200;
            _mockRepo.Setup(r => r.GetByUserAndJobPostAsync(userId, jobPostId)).ReturnsAsync(new SavedJob());

            // Act
            var result = await _service.IsJobSavedAsync(userId, jobPostId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsJobSavedAsync_JobIsNotSaved_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int jobPostId = 200;
            _mockRepo.Setup(r => r.GetByUserAndJobPostAsync(userId, jobPostId)).ReturnsAsync((SavedJob)null);

            // Act
            var result = await _service.IsJobSavedAsync(userId, jobPostId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteSavedJobAsync_Success_ReturnsTrue()
        {
            // Arrange
            int saveJobId = 10;
            _mockRepo.Setup(r => r.DeleteAsync(saveJobId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteSavedJobAsync(saveJobId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteSavedJobAsync_NotFound_ReturnsFalse()
        {
            // Arrange
            int saveJobId = 10;
            _mockRepo.Setup(r => r.DeleteAsync(saveJobId)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteSavedJobAsync(saveJobId);

            // Assert
            Assert.False(result);
        }
    }
}
