using Microsoft.AspNetCore.Mvc;
using Moq;
using SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Controllers
{
    public class UserForAdminController_BanUser_Tests
    {
        private readonly Mock<IUserDetailOfAdminService> _mockUserService;
        private readonly Mock<IBanUserService> _mockBanUserService;
        private readonly UserForAdminController _controller;

        public UserForAdminController_BanUser_Tests()
        {
            _mockUserService = new Mock<IUserDetailOfAdminService>();
            _mockBanUserService = new Mock<IBanUserService>();
            _controller = new UserForAdminController(_mockUserService.Object, _mockBanUserService.Object);
        }

        [Theory]
        [InlineData(1, true, 200, "User banned successfully")]
        [InlineData(1, false, 404, "User not found")]
        [InlineData(0, false, 404, "User not found")]                // Biên dưới
        [InlineData(-1, false, 404, "User not found")]              // Trước biên dưới
        [InlineData(2, true, 200, "User banned successfully")]      // Sau biên dưới

        [InlineData(int.MaxValue - 1, true, 200, "User banned successfully")]  // Trước biên trên
        [InlineData(int.MaxValue, false, 404, "User not found")]              // Biên trên

        [InlineData(123, true, 200, "User banned successfully")]    // Giá trị bình thường

        [InlineData(-999, false, 404, "User not found")]            // Bất thường: âm sâu
        [InlineData(999999999, false, 404, "User not found")]       // Bất thường: lớn quá
        public async Task BanUser_TestCases(int userId, bool serviceResult, int expectedStatusCode, string expectedMessage)
        {
            // Arrange
            _mockBanUserService.Setup(s => s.BanUserAsync(userId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.BanUser(userId);

            // Assert
            if (serviceResult)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(expectedStatusCode, okResult.StatusCode);
                Assert.Contains(expectedMessage, okResult.Value.ToString());
            }
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(expectedStatusCode, notFoundResult.StatusCode);
                Assert.Contains(expectedMessage, notFoundResult.Value.ToString());
            }
        }
    }
}
