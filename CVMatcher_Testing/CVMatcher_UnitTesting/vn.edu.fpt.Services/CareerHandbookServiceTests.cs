using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CareerHandbookRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CareerHandbookService;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SEP490_SU25_G86_Tests.Services
{
    public class CareerHandbookServiceTests
    {
        private readonly Mock<ICareerHandbookRepository> _mockRepo;
        private readonly CareerHandbookService _service;

        public CareerHandbookServiceTests()
        {
            _mockRepo = new Mock<ICareerHandbookRepository>();
            _service = new CareerHandbookService(_mockRepo.Object);
        }


        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            // Arrange
            int id = 1;
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CareerHandbook)null);

            // Act
            var result = await _service.UpdateAsync(id, new CareerHandbookUpdateDTO());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenSlugExists()
        {
            // Arrange
            int id = 1;
            var dto = new CareerHandbookUpdateDTO { Slug = "dup-slug" };
            var handbook = new CareerHandbook { HandbookId = id, Slug = "old-slug" };

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(handbook);
            _mockRepo.Setup(r => r.ExistsBySlugAsync(dto.Slug, id)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(id, dto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenValid()
        {
            // Arrange
            int id = 1;
            var dto = new CareerHandbookUpdateDTO
            {
                Title = "Updated Title",
                Slug = "updated-slug",
                Content = "Updated content",
                CategoryId = 2,
                IsPublished = true,
                Tags = "tag1,tag2",
                ThumbnailFile = null
            };

            var handbook = new CareerHandbook
            {
                HandbookId = id,
                Title = "Old Title",
                Slug = "old-slug",
                Content = "Old Content",
                CategoryId = 1,
                IsPublished = false,
                Tags = ""
            };

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(handbook);
            _mockRepo.Setup(r => r.ExistsBySlugAsync(dto.Slug, id)).ReturnsAsync(false);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<CareerHandbook>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(id, dto);

            // Assert
            Assert.True(result);
            Assert.Equal("Updated Title", handbook.Title);
            Assert.Equal("updated-slug", handbook.Slug);
            Assert.Equal("Updated content", handbook.Content);
            Assert.Equal(2, handbook.CategoryId);
            Assert.True(handbook.IsPublished);
            Assert.Equal("tag1,tag2", handbook.Tags);
            _mockRepo.Verify(r => r.UpdateAsync(handbook), Times.Once);
        }
    }
}
