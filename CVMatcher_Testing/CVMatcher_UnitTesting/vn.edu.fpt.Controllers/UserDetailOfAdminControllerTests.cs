using Microsoft.AspNetCore.Mvc;
using Moq;
using SEP490_SU25_G86_API.vn.edu.fpt.Controllers;
using SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Controllers
{
    public class UserDetailOfAdminControllerTests
    {
        private readonly Mock<IUserDetailOfAdminService> _mockService;
        private readonly Mock<IBanUserService> _mockBanService;
        private readonly UserForAdminController _controller;

        public UserDetailOfAdminControllerTests()
        {
            _mockService = new Mock<IUserDetailOfAdminService>();
            _mockBanService = new Mock<IBanUserService>();
            _controller = new UserForAdminController(_mockService.Object, _mockBanService.Object);
        }

        [Fact]
        public async Task GetUserDetailByAccountId_ReturnsOk_WhenUserExists()
        {
            // Arrange
            int testAccountId = 1;
            var expectedDto = new UserDetailOfAdminDTO
            {
                AccountId = testAccountId,
                FullName = "John Doe",
                AccountEmail = "john@example.com"
            };

            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(testAccountId))
                        .ReturnsAsync(expectedDto);

            // Act
            var result = await _controller.GetUserDetailByAccountId(testAccountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDetailOfAdminDTO>(okResult.Value);
            Assert.Equal(expectedDto.AccountId, returnValue.AccountId);
            Assert.Equal(expectedDto.FullName, returnValue.FullName);
        }

        [Fact]
        public async Task GetUserDetailByAccountId_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int testAccountId = 999;

            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(testAccountId))
                        .ReturnsAsync((UserDetailOfAdminDTO)null);

            // Act
            var result = await _controller.GetUserDetailByAccountId(testAccountId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetUserDetailByAccountId_ThrowsException_WhenServiceFails()
        {
            // Arrange
            int testAccountId = 123;

            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(testAccountId))
                        .ThrowsAsync(new Exception("Database error"));

            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _controller.GetUserDetailByAccountId(testAccountId));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetUserDetailByAccountId_InvalidId_ReturnsNotFound(int accountId)
        {
            // Arrange
            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(accountId))
                        .ReturnsAsync((UserDetailOfAdminDTO)null);

            // Act
            var result = await _controller.GetUserDetailByAccountId(accountId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetUserDetailByAccountId_ReturnsFullDtoData()
        {
            // Arrange
            int testAccountId = 101;
            var expectedDto = new UserDetailOfAdminDTO
            {
                AccountId = testAccountId,
                FullName = "Nguyen Van A",
                Phone = "0901234567",
                Avatar = "avatar.png",
                Address = "Hanoi",
                Gender = "Male",
                LinkedIn = "linkedin.com/a",
                Facebook = "facebook.com/a",
                CreatedDate = new DateTime(2023, 1, 1),
                IsActive = true,
                IsBan = false,
                AccountEmail = "admin@domain.com",
                CompanyName = "FPT Software"
            };

            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(testAccountId))
                        .ReturnsAsync(expectedDto);

            // Act
            var result = await _controller.GetUserDetailByAccountId(testAccountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDetailOfAdminDTO>(okResult.Value);
            Assert.Equal(expectedDto.CompanyName, returnValue.CompanyName);
            Assert.True(returnValue.IsActive);
            Assert.False(returnValue.IsBan);
        }

        [Theory]
        [InlineData(1)]                          // Biên dưới + 1
        [InlineData(int.MaxValue - 1)]          // Trước biên trên
        [InlineData(int.MaxValue)]              // Biên trên
        [InlineData(999999999)]                 // Bất thường: quá lớn
        public async Task GetUserDetailByAccountId_BoundaryAndLargeValues_ReturnsResultOrNotFound(int accountId)
        {
            // Arrange
            _mockService.Setup(s => s.GetUserDetailByAccountIdAsync(accountId))
                        .ReturnsAsync((UserDetailOfAdminDTO)null); // Giả định không tồn tại

            // Act
            var result = await _controller.GetUserDetailByAccountId(accountId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }
    }
}
