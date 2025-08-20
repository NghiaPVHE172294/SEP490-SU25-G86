using Microsoft.EntityFrameworkCore;
using Moq;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobCriterionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobCriterionService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Services
{
    public class JobCriterionServiceTests
    {
        private async Task<SEP490_G86_CvMatchContext> GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<SEP490_G86_CvMatchContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var context = new SEP490_G86_CvMatchContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddJobCriterionAsync_ShouldAddSuccessfully()
        {
            // Arrange
            var userId = 1;
            var jobPostId = 99;
            var context = await GetInMemoryDbContextAsync();

            context.JobPosts.Add(new JobPost
            {
                JobPostId = jobPostId,
                EmployerId = userId,
                IsDelete = false,
                Title = "Test Job Title" // REQUIRED field to avoid DbUpdateException
            });
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IJobCriterionRepository>();
            mockRepo.Setup(repo => repo.AddJobCriterionAsync(It.IsAny<JobCriterion>()))
                    .ReturnsAsync((JobCriterion jc) => jc);

            var service = new JobCriterionService(mockRepo.Object, context);
            var dto = new AddJobCriterionDTO
            {
                JobPostId = jobPostId,
                RequiredSkills = "C#",
                EducationLevel = "Bachelor",
                Address = "Hanoi"
            };

            // Act
            var result = await service.AddJobCriterionAsync(dto, userId);

            // Assert
            Assert.Equal(dto.JobPostId, result.JobPostId);
            Assert.Equal(dto.RequiredSkills, result.RequiredSkills);
            Assert.Equal(dto.EducationLevel, result.EducationLevel);
            Assert.Equal(dto.Address, result.Address);
            mockRepo.Verify(r => r.AddJobCriterionAsync(It.IsAny<JobCriterion>()), Times.Once);
        }

        [Fact]
        public async Task AddJobCriterionAsync_ShouldThrowException_WhenJobPostNotFound()
        {
            // Arrange
            var userId = 1;
            var dto = new AddJobCriterionDTO
            {
                JobPostId = 123,
                RequiredSkills = "C#",
                EducationLevel = "Bachelor",
                Address = "Hanoi"
            };

            var context = await GetInMemoryDbContextAsync();
            var mockRepo = new Mock<IJobCriterionRepository>();
            var service = new JobCriterionService(mockRepo.Object, context);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.AddJobCriterionAsync(dto, userId));
        }

        [Fact]
        public async Task AddJobCriterionAsync_ShouldThrowUnauthorized_WhenUserNotOwner()
        {
            // Arrange
            var userId = 1;
            var jobPostId = 100;
            var context = await GetInMemoryDbContextAsync();

            context.JobPosts.Add(new JobPost
            {
                JobPostId = jobPostId,
                EmployerId = 999, // Not the same as userId => should trigger Unauthorized
                IsDelete = false,
                Title = "Unauthorized Post"
            });
            await context.SaveChangesAsync();

            var dto = new AddJobCriterionDTO
            {
                JobPostId = jobPostId,
                RequiredSkills = "C#",
                EducationLevel = "Bachelor",
                Address = "Hanoi"
            };

            var mockRepo = new Mock<IJobCriterionRepository>();
            var service = new JobCriterionService(mockRepo.Object, context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.AddJobCriterionAsync(dto, userId));
            Assert.Contains("Access Denied", ex.Message);
        }
    }
}
