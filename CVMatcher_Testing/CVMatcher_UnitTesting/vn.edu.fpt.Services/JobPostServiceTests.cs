//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using SEP490_SU25_G86_API.Models;
//using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
//using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
//using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;
//using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
//using Xunit;

//namespace CVMatcher_Testing.vn.edu.fpt.Services
//{
//    public class JobPostServiceTests
//    {
//        private static SEP490_G86_CvMatchContext NewInMemoryContext()
//        {
//            var options = new DbContextOptionsBuilder<SEP490_G86_CvMatchContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;
//            var ctx = new SEP490_G86_CvMatchContext(options);
//            ctx.Database.EnsureCreated();
//            return ctx;
//        }

//        private static JobPostService BuildService(
//            SEP490_G86_CvMatchContext ctx,
//            out Mock<IJobPostRepository> repo,
//            out Mock<IBlockedCompanyRepository> blocked)
//        {
//            repo = new Mock<IJobPostRepository>();
//            blocked = new Mock<IBlockedCompanyRepository>();
//            return new JobPostService(repo.Object, blocked.Object, ctx);
//        }

//        [Fact]
//        public async Task AddJobPostAsync_WithExistingIds_ShouldCreateAndReturnDetail()
//        {
//            using var ctx = NewInMemoryContext();
//            var service = BuildService(ctx, out var repo, out _);

//            var dto = new AddJobPostDTO
//            {
//                Title = "Backend Developer",
//                EndDate = DateTime.UtcNow.AddDays(10),
//                Description = "Build APIs",
//                CandidaterRequirements = "C#/.NET",
//                Interest = "Great team",
//                WorkLocation = "Hanoi",
//                Status = "OPEN",
//                IndustryId = 1,
//                JobPositionId = 2,
//                SalaryRangeId = 3,
//                ProvinceId = 4,
//                ExperienceLevelId = 5,
//                JobLevelId = 6,
//                EmploymentTypeId = 7
//            };

//            JobPost? captured = null;
//            repo.Setup(r => r.AddJobPostAsync(It.IsAny<JobPost>()))
//                .Callback<JobPost>(jp => captured = jp)
//                .ReturnsAsync((JobPost jp) => { jp.JobPostId = 100; return jp; });

//            repo.Setup(r => r.GetJobPostByIdAsync(100))
//                .ReturnsAsync(() => captured!);

//            var result = await service.AddJobPostAsync(dto, employerId: 88);

//            Assert.NotNull(result);
//            Assert.Equal(100, result.JobPostId);
//            Assert.Equal("Backend Developer", result.Title);
//            Assert.Equal(88, captured!.EmployerId);
//            Assert.Equal(1, captured.IndustryId);
//            Assert.Equal(2, captured.JobPositionId);
//            Assert.Equal(3, captured.SalaryRangeId);
//            Assert.Equal(4, captured.ProvinceId);
//            Assert.Equal(5, captured.ExperienceLevelId);
//            Assert.Equal(6, captured.JobLevelId);
//            Assert.Equal(7, captured.EmploymentTypeId);

//            repo.Verify(r => r.AddJobPostAsync(It.IsAny<JobPost>()), Times.Once);
//            repo.Verify(r => r.GetJobPostByIdAsync(100), Times.Once);
//        }

//        [Fact]
//        public async Task AddJobPostAsync_WithNewLookupNames_ShouldCreateLookups_AndUseIds()
//        {
//            using var ctx = NewInMemoryContext();
//            var service = BuildService(ctx, out var repo, out _);

//            var dto = new AddJobPostDTO
//            {
//                Title = "Data Engineer",
//                EndDate = DateTime.UtcNow.AddDays(12),
//                Description = "Pipelines",
//                CandidaterRequirements = "SQL",
//                Interest = "Stock options",
//                WorkLocation = "HCM",
//                Status = "OPEN",
//                NewIndustryName = "  Tech  ",                   // phải Trim():contentReference[oaicite:2]{index=2}
//                NewJobPositionName = "Data Engineer",
//                NewSalaryRange = "1000-2000-USD",
//                NewProvinceName = "HCM",
//                NewExperienceLevelName = "Senior",
//                NewJobLevelName = "Lead",
//                NewEmploymentTypeName = "Full-time"
//            };

//            repo.Setup(r => r.AddIndustryIfNotExistsAsync("Tech"))
//                .ReturnsAsync(new Industry { IndustryId = 10, IndustryName = "Tech" });
//            repo.Setup(r => r.AddJobPositionIfNotExistsAsync("Data Engineer", 10))
//                .ReturnsAsync(new JobPosition { PositionId = 20, PostitionName = "Data Engineer", IndustryId = 10 });
//            repo.Setup(r => r.AddSalaryRangeIfNotExistsAsync(1000, 2000, "USD"))
//                .ReturnsAsync(new SalaryRange { SalaryRangeId = 30, MinSalary = 1000, MaxSalary = 2000, Currency = "USD" });
//            repo.Setup(r => r.AddProvinceIfNotExistsAsync("HCM"))
//                .ReturnsAsync(new Province { ProvinceId = 40, ProvinceName = "HCM" });
//            repo.Setup(r => r.AddExperienceLevelIfNotExistsAsync("Senior"))
//                .ReturnsAsync(new ExperienceLevel { ExperienceLevelId = 50, ExperienceLevelName = "Senior" });
//            repo.Setup(r => r.AddJobLevelIfNotExistsAsync("Lead"))
//                .ReturnsAsync(new JobLevel { JobLevelId = 60, JobLevelName = "Lead" });
//            repo.Setup(r => r.AddEmploymentTypeIfNotExistsAsync("Full-time"))
//                .ReturnsAsync(new EmploymentType { EmploymentTypeId = 70, EmploymentTypeName = "Full-time" });

//            JobPost? captured = null;
//            repo.Setup(r => r.AddJobPostAsync(It.IsAny<JobPost>()))
//                .Callback<JobPost>(jp => captured = jp)
//                .ReturnsAsync((JobPost jp) => { jp.JobPostId = 999; return jp; });

//            repo.Setup(r => r.GetJobPostByIdAsync(999))
//                .ReturnsAsync(() => captured!);

//            var result = await service.AddJobPostAsync(dto, employerId: 1);

//            Assert.NotNull(result);
//            Assert.Equal(999, result.JobPostId);
//            Assert.Equal(10, captured!.IndustryId);
//            Assert.Equal(20, captured.JobPositionId);
//            Assert.Equal(30, captured.SalaryRangeId);
//            Assert.Equal(40, captured.ProvinceId);
//            Assert.Equal(50, captured.ExperienceLevelId);
//            Assert.Equal(60, captured.JobLevelId);
//            Assert.Equal(70, captured.EmploymentTypeId);

//            repo.Verify(r => r.AddIndustryIfNotExistsAsync("Tech"), Times.Once);
//            repo.Verify(r => r.AddJobPositionIfNotExistsAsync("Data Engineer", 10), Times.Once);
//            repo.Verify(r => r.AddSalaryRangeIfNotExistsAsync(1000, 2000, "USD"), Times.Once);
//            repo.Verify(r => r.AddProvinceIfNotExistsAsync("HCM"), Times.Once);
//            repo.Verify(r => r.AddExperienceLevelIfNotExistsAsync("Senior"), Times.Once);
//            repo.Verify(r => r.AddJobLevelIfNotExistsAsync("Lead"), Times.Once);
//            repo.Verify(r => r.AddEmploymentTypeIfNotExistsAsync("Full-time"), Times.Once);
//        }

//        [Fact]
//        public async Task AddJobPostAsync_WithCvTemplate_ShouldCreateLink_AndDetailHasTemplate()
//        {
//            using var ctx = NewInMemoryContext();
//            // seed sẵn bản ghi CvTemplate để GetJobPostDetail lấy được thông tin:contentReference[oaicite:3]{index=3}
//            ctx.CvTemplateOfEmployers.Add(new CvTemplateOfEmployer
//            {
//                CvtemplateOfEmployerId = 77,
//                CvTemplateName = "Standard Template",
//                DocFileUrl = "doc.url",
//                PdfFileUrl = "pdf.url",
//                IsDelete = false
//            });
//            await ctx.SaveChangesAsync();

//            var service = BuildService(ctx, out var repo, out _);

//            var dto = new AddJobPostDTO
//            {
//                Title = "QA Engineer",
//                EndDate = DateTime.UtcNow.AddDays(8),
//                Description = "Test",
//                CandidaterRequirements = "Automation",
//                Interest = "Bonus",
//                WorkLocation = "Da Nang",
//                Status = "OPEN",
//                IndustryId = 1,
//                JobPositionId = 2,
//                SalaryRangeId = 3,
//                ProvinceId = 4,
//                ExperienceLevelId = 5,
//                JobLevelId = 6,
//                EmploymentTypeId = 7,
//                CvtemplateOfEmployerId = 77
//            };

//            JobPost? captured = null;
//            repo.Setup(r => r.AddJobPostAsync(It.IsAny<JobPost>()))
//                .Callback<JobPost>(jp => captured = jp)
//                .ReturnsAsync((JobPost jp) => { jp.JobPostId = 501; return jp; });

//            repo.Setup(r => r.GetJobPostByIdAsync(501))
//                .ReturnsAsync(() => captured!);

//            var result = await service.AddJobPostAsync(dto, employerId: 9);

//            // Đã có 1 link trong bảng CvTemplateForJobposts
//            var link = ctx.CvTemplateForJobposts.FirstOrDefault(x => x.JobPostId == 501);
//            Assert.NotNull(link);
//            Assert.Equal(77, link.CvtemplateOfEmployerId);
//            Assert.True(link.IsDisplay);

//            // Detail trả về có thông tin template
//            Assert.Equal(77, result.CvTemplateId);
//            Assert.Equal("Standard Template", result.CvTemplateName);
//            Assert.Equal("doc.url", result.DocFileUrl);
//            Assert.Equal("pdf.url", result.PdfFileUrl);
//        }

//        [Fact]
//        public async Task AddJobPostAsync_WithInvalidNewSalaryRange_ShouldNotCreateSalaryRange()
//        {
//            using var ctx = NewInMemoryContext();
//            var service = BuildService(ctx, out var repo, out _);

//            var dto = new AddJobPostDTO
//            {
//                Title = "Support Engineer",
//                EndDate = DateTime.UtcNow.AddDays(9),
//                Description = "Help customers",
//                CandidaterRequirements = "Communication",
//                Interest = "Allowance",
//                WorkLocation = "HN",
//                Status = "OPEN",
//                // Chuỗi sai định dạng → service KHÔNG gọi AddSalaryRangeIfNotExistsAsync:contentReference[oaicite:4]{index=4}
//                NewSalaryRange = "abc-def-VND",
//                IndustryId = 1,
//                JobPositionId = 2,
//                ProvinceId = 3,
//                ExperienceLevelId = 4,
//                JobLevelId = 5,
//                EmploymentTypeId = 6
//            };

//            JobPost? captured = null;
//            repo.Setup(r => r.AddJobPostAsync(It.IsAny<JobPost>()))
//                .Callback<JobPost>(jp => captured = jp)
//                .ReturnsAsync((JobPost jp) => { jp.JobPostId = 321; return jp; });

//            repo.Setup(r => r.GetJobPostByIdAsync(321))
//                .ReturnsAsync(() => captured!);

//            var result = await service.AddJobPostAsync(dto, employerId: 42);

//            Assert.NotNull(result);
//            // vì NewSalaryRange không hợp lệ → SalaryRangeId vẫn null
//            Assert.Null(captured!.SalaryRangeId);

//            repo.Verify(r => r.AddSalaryRangeIfNotExistsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
//        }
//    }
//}
