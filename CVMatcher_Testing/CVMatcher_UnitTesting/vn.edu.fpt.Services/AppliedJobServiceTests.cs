using Xunit;
using Moq;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository;
using SEP490_SU25_G86_API.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;
using MockQueryable.Moq;

/*
 * ============================
 * HƯỚNG DẪN VỀ UNIT TEST VÀ GIẢI THÍCH LỖI
 * ============================
 *
 * 1. Cách hoạt động của Unit Test trong file này:
 * - Unit test kiểm thử từng hàm/service nhỏ, độc lập với hệ thống thật (không truy cập DB thật).
 * - Sử dụng Moq để "giả lập" (mock) các repository, context, ...
 * - Mỗi test case sẽ chuẩn bị dữ liệu đầu vào (Arrange), gọi hàm cần test (Act), và kiểm tra kết quả (Assert).
 * - Nếu hàm cần kiểm thử gọi tới DB, ta mock các phương thức như Add, SaveChangesAsync, ... để kiểm soát kết quả trả về hoặc lỗi.
 * ============================
 */

namespace CVMatcher_Testing.CVMatcher_UnitTesting.vn.edu.fpt.Services
{
    public class AppliedJobServiceTests
    {
        [Fact(DisplayName = "[TC_UT_AppliedJob_01] AddSubmissionAsync_ValidSubmission_SavesToDb")]
        // Test: Đảm bảo khi submission hợp lệ thì sẽ được lưu vào DbContext (Add + SaveChangesAsync được gọi đúng 1 lần)
        public async Task AddSubmissionAsync_ValidSubmission_SavesToDb()
        {
            // Arrange
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);

            var submission = new Cvsubmission { SubmissionId = 1, JobPostId = 2, SubmittedByUserId = 3, CvId = 4 };
            mockContext.Setup(c => c.Cvsubmissions.Add(submission));
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await service.AddSubmissionAsync(submission);

            // Assert
            mockContext.Verify(c => c.Cvsubmissions.Add(submission), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "[TC_UT_AppliedJob_02] AddSubmissionAsync_DbThrowsException_ThrowsException")]
        // Test: Đảm bảo nếu SaveChangesAsync bị lỗi (exception) thì exception sẽ được ném ra đúng cách
        public async Task AddSubmissionAsync_DbThrowsException_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            var submission = new Cvsubmission { SubmissionId = 1 };
            mockContext.Setup(c => c.Cvsubmissions.Add(submission));
            mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.AddSubmissionAsync(submission));
        }
        [Fact(DisplayName = "[TC_UT_AppliedJob_03] HasUserAppliedToJobAsync_UserHasApplied_ReturnsTrue")]
        // Test: Đảm bảo khi user đã nộp CV cho jobpost thì service trả về true
        public async Task HasUserAppliedToJobAsync_UserHasApplied_ReturnsTrue()
        {
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            mockRepo.Setup(r => r.HasUserAppliedToJobAsync(3, 2)).ReturnsAsync(true);
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            var result = await service.HasUserAppliedToJobAsync(3, 2);
            Assert.True(result);
        }

        [Fact(DisplayName = "[TC_UT_AppliedJob_04] HasUserAppliedToJobAsync_UserHasNotApplied_ReturnsFalse")]
        // Test: Đảm bảo khi user chưa từng nộp CV cho jobpost thì service trả về false
        public async Task HasUserAppliedToJobAsync_UserHasNotApplied_ReturnsFalse()
        {
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            mockRepo.Setup(r => r.HasUserAppliedToJobAsync(3, 2)).ReturnsAsync(false);
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            var result = await service.HasUserAppliedToJobAsync(3, 2);
            Assert.False(result);
        }

        [Fact(DisplayName = "[TC_UT_AppliedJob_05] AddSubmissionAsync_NullSubmission_ThrowsArgumentNullException")]
        // Test: Đảm bảo nếu truyền submission null vào thì service sẽ ném ra ArgumentNullException (bảo vệ input)
        public async Task AddSubmissionAsync_NullSubmission_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddSubmissionAsync(null));
        }

        [Fact(DisplayName = "[TC_UT_AppliedJob_06] AddCvAndGetIdAsync_OverMaxLimit_ThrowsException")]
        // Test: Đảm bảo nếu số lượng CV của candidate vượt quá giới hạn thì sẽ ném ra Exception
        public async Task AddCvAndGetIdAsync_OverMaxLimit_ThrowsException()
        {
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            var cv = new Cv { CandidateId = 5 };
            // Tạo danh sách CV vượt quá giới hạn
            var cvList = new List<Cv>();
            for (int i = 0; i < 20; i++)
            {
                cvList.Add(new Cv { CandidateId = 5, IsDelete = false });
            }
            var mockSet = cvList.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Cvs).Returns(mockSet.Object);
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            await Assert.ThrowsAsync<Exception>(() => service.AddCvAndGetIdAsync(cv));
        }

        [Fact(DisplayName = "[TC_UT_AppliedJob_07] WithdrawApplicationAsync_SubmissionNotFound_ReturnsFalse")]
        // Test: Đảm bảo khi submission cần rút không tồn tại thì service trả về false
        public async Task WithdrawApplicationAsync_SubmissionNotFound_ReturnsFalse()
        {
            var mockRepo = new Mock<IAppliedJobRepository>();
            var mockCvRepo = new Mock<ICvRepository>();
            var mockContext = new Mock<SEP490_G86_CvMatchContext>();
            // Danh sách submission rỗng để FirstOrDefaultAsync trả về null
            var submissionList = new List<Cvsubmission>();
            var mockSet = submissionList.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Cvsubmissions).Returns(mockSet.Object);
            var service = new AppliedJobService(mockRepo.Object, mockCvRepo.Object, mockContext.Object);
            var result = await service.WithdrawApplicationAsync(1, 2);
            Assert.False(result);
        }
    }
}
