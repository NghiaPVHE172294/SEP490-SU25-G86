using Xunit;
using Moq;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository;
using SEP490_SU25_G86_API.Models;

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Services
{
    public class AccountServiceTests
    {
        [Fact]
        public void Authenticate_ValidCredentials_ReturnsAccount()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            var expectedAccount = new Account { Email = "test@mail.com", Password = AccountService.GetMd5HashStatic("123456") };
            mockRepo.Setup(r => r.GetByEmail("test@mail.com")).Returns(expectedAccount);
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Authenticate("test@mail.com", "123456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@mail.com", result.Email);
        }

        [Fact]
        public void Authenticate_WrongPassword_ReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            var expectedAccount = new Account { Email = "test@mail.com", Password = AccountService.GetMd5HashStatic("123456") };
            mockRepo.Setup(r => r.GetByEmail("test@mail.com")).Returns(expectedAccount);
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Authenticate("test@mail.com", "wrongpass");

            // Assert
            Assert.Null(result);
        }



        [Fact]
        public void Authenticate_NullEmail_ReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Authenticate(null, "123456");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Authenticate_EmptyPassword_ReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            var expectedAccount = new Account { Email = "test@mail.com", Password = AccountService.GetMd5HashStatic("123456") };
            mockRepo.Setup(r => r.GetByEmail("test@mail.com")).Returns(expectedAccount);
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Authenticate("test@mail.com", "");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Authenticate_AccountNotFound_ReturnsNull()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetByEmail("notfound@mail.com")).Returns((Account)null);
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Authenticate("notfound@mail.com", "any");

            // Assert
            Assert.Null(result);
        }
    }
}
