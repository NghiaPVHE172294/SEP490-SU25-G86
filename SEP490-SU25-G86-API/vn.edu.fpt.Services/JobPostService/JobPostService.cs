using AutoMapper;
using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;


namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jobPostRepo;
        private readonly IBlockedCompanyRepository _blockedCompanyRepo;
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public JobPostService(IJobPostRepository jobPostRepo, IBlockedCompanyRepository blockedCompanyRepo, SEP490_G86_CvMatchContext context, IMapper mapper, INotificationService notificationService)
        {
            _jobPostRepo = jobPostRepo;
            _blockedCompanyRepo = blockedCompanyRepo;
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        
        public async Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null, int? salaryRangeId = null, int? experienceLevelId = null, int? candidateId = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetPagedJobPostsAsync(page, pageSize, region, salaryRangeId, experienceLevelId, candidateId);

            List<int> appliedJobPostIds = new();
            if (candidateId.HasValue)
            {
                appliedJobPostIds = await _jobPostRepo.GetAppliedJobPostIdsAsync(candidateId.Value);
            }

            var result = posts.Select(j => new JobPostHomeDto
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company?.CompanyName ?? "Không rõ",
                CompanyId = j.Employer?.CompanyId,
                CompanyLogoUrl = j.Employer?.Company?.LogoUrl,
                Salary = j.SalaryRange != null
                         ? $"{j.SalaryRange.MinSalary:N0} - {j.SalaryRange.MaxSalary:N0} {j.SalaryRange.Currency}"
                         : "Thỏa thuận",
                Location = j.Province?.ProvinceName ?? "Không xác định",
                IsApplied = candidateId.HasValue ? appliedJobPostIds.Contains(j.JobPostId) : false
            }).ToArray();

            return (result, totalItems);
        }

        public async Task<IEnumerable<JobPostDTO>> GetAllJobPostsAsync()
        {
            var posts = await _jobPostRepo.GetAllAsync();
            return posts.Select(post => new JobPostDTO
            {
                JobPostId = post.JobPostId,
                Title = post.Title,
                WorkLocation = post.WorkLocation,
                Status = post.Status,
                CreatedDate = post.CreatedDate,
                EndDate = post.EndDate
            });
        }

        public async Task<IEnumerable<JobPostListDTO>> GetByEmployerIdAsync(int employerId)
        {
            var posts = await _jobPostRepo.GetByEmployerIdAsync(employerId);
            return _mapper.Map<IEnumerable<JobPostListDTO>>(posts);
        }


        public async Task<ViewDetailJobPostDTO?> GetJobPostDetailByIdAsync(int jobPostId)
        {
            var jobPost = await _jobPostRepo.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return null;
            // Lấy liên kết template (ưu tiên IsDisplay = true, hoặc lấy bản ghi đầu tiên nếu không có)
            var cvTemplateLink = _context.CvTemplateForJobposts
                .Where(x => x.JobPostId == jobPost.JobPostId)
                .OrderByDescending(x => x.IsDisplay)
                .FirstOrDefault();
            CvTemplateOfEmployer? template = null;
            if (cvTemplateLink != null)
            {
                template = _context.CvTemplateOfEmployers.FirstOrDefault(t => t.CvtemplateOfEmployerId == cvTemplateLink.CvtemplateOfEmployerId && !t.IsDelete);
            }

            // Dùng AutoMapper để map entity → DTO
            var dto = _mapper.Map<ViewDetailJobPostDTO>(jobPost);

            // Map thủ công các trường đặc biệt từ template
            dto.CvTemplateId = template?.CvtemplateOfEmployerId;
            dto.CvTemplateName = template?.CvTemplateName;
            dto.DocFileUrl = template?.DocFileUrl;
            dto.PdfFileUrl = template?.PdfFileUrl;

            return dto;
        }


        public async Task<(IEnumerable<JobPostListDTO> Posts, int TotalItems)> GetFilteredJobPostsAsync(
            int page, int pageSize,
            int? provinceId = null,
            int? industryId = null,
            List<int>? employmentTypeIds = null,
            List<int>? experienceLevelIds = null,
            int? jobLevelId = null,
            int? minSalary = null,
            int? maxSalary = null,
            List<int>? datePostedRanges = null,
            string? keyword = null,
            int? candidateId = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetFilteredJobPostsAsync(
                page, pageSize, provinceId, industryId, employmentTypeIds, experienceLevelIds, jobLevelId, minSalary, maxSalary, datePostedRanges, keyword, candidateId
            );

            if (minSalary.HasValue && minSalary.Value < 0)
                minSalary = null;

            if (maxSalary.HasValue && maxSalary.Value < 0)
                maxSalary = null;

            List<int> appliedJobPostIds = new();
            if (candidateId.HasValue)
            {
                appliedJobPostIds = await _jobPostRepo.GetAppliedJobPostIdsAsync(candidateId.Value);
            }

            var result = posts.Select(j => new JobPostListDTO
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company?.CompanyName ?? "Không rõ",
                CompanyId = j.Employer?.Company?.CompanyId,
                CompanyLogoUrl = j.Employer?.Company?.LogoUrl,
                Salary = (j.SalaryRange != null && j.SalaryRange.MinSalary.HasValue && j.SalaryRange.MaxSalary.HasValue)
                    ? $"{j.SalaryRange.MinSalary:N0} - {j.SalaryRange.MaxSalary:N0} {j.SalaryRange.Currency}"
                    : "Thỏa thuận",
                Location = j.Province?.ProvinceName,
                EmploymentType = j.EmploymentType?.EmploymentTypeName,
                JobLevel = j.JobLevel?.JobLevelName,
                ExperienceLevel = j.ExperienceLevel?.ExperienceLevelName,
                Industry = j.Industry?.IndustryName,
                CreatedDate = j.CreatedDate,
                UpdatedDate = j.UpdatedDate,
                EndDate = j.EndDate,
                Status = j.Status,
                WorkLocation = j.WorkLocation,
                IsApplied = candidateId.HasValue ? appliedJobPostIds.Contains(j.JobPostId) : false
            });
            return (result, totalItems);
        }

        public async Task<ViewDetailJobPostDTO> AddJobPostAsync(AddJobPostDTO dto, int employerId)
        {
            // Xử lý các trường liên kết nếu có nhập mới
            int? industryId = dto.IndustryId;
            if (!string.IsNullOrWhiteSpace(dto.NewIndustryName))
            {
                var industry = await _jobPostRepo.AddIndustryIfNotExistsAsync(dto.NewIndustryName.Trim());
                industryId = industry.IndustryId;
            }

            int? jobPositionId = dto.JobPositionId;
            if (!string.IsNullOrWhiteSpace(dto.NewJobPositionName))
            {
                var jobPosition = await _jobPostRepo.AddJobPositionIfNotExistsAsync(dto.NewJobPositionName.Trim(), industryId);
                jobPositionId = jobPosition.PositionId;
            }

            int? salaryRangeId = dto.SalaryRangeId;
            if (!string.IsNullOrWhiteSpace(dto.NewSalaryRange))
            {
                // Định dạng: "min-max-currency"
                var parts = dto.NewSalaryRange.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                {
                    var salaryRange = await _jobPostRepo.AddSalaryRangeIfNotExistsAsync(min, max, parts[2]);
                    salaryRangeId = salaryRange.SalaryRangeId;
                }
            }

            int? provinceId = dto.ProvinceId;
            if (!string.IsNullOrWhiteSpace(dto.NewProvinceName))
            {
                var province = await _jobPostRepo.AddProvinceIfNotExistsAsync(dto.NewProvinceName.Trim());
                provinceId = province.ProvinceId;
            }

            int? experienceLevelId = dto.ExperienceLevelId;
            if (!string.IsNullOrWhiteSpace(dto.NewExperienceLevelName))
            {
                var exp = await _jobPostRepo.AddExperienceLevelIfNotExistsAsync(dto.NewExperienceLevelName.Trim());
                experienceLevelId = exp.ExperienceLevelId;
            }

            int? jobLevelId = dto.JobLevelId;
            if (!string.IsNullOrWhiteSpace(dto.NewJobLevelName))
            {
                var jl = await _jobPostRepo.AddJobLevelIfNotExistsAsync(dto.NewJobLevelName.Trim());
                jobLevelId = jl.JobLevelId;
            }

            int? employmentTypeId = dto.EmploymentTypeId;
            if (!string.IsNullOrWhiteSpace(dto.NewEmploymentTypeName))
            {
                var et = await _jobPostRepo.AddEmploymentTypeIfNotExistsAsync(dto.NewEmploymentTypeName.Trim());
                employmentTypeId = et.EmploymentTypeId;
            }

            // Tạo JobPost
            var jobPost = new Models.JobPost
            {
                IndustryId = industryId,
                JobPositionId = jobPositionId,
                Title = dto.Title,
                SalaryRangeId = salaryRangeId,
                ProvinceId = provinceId,
                ExperienceLevelId = experienceLevelId,
                JobLevelId = jobLevelId,
                EmploymentTypeId = employmentTypeId,
                EndDate = dto.EndDate,
                Description = dto.Description,
                CandidaterRequirements = dto.CandidaterRequirements,
                Interest = dto.Interest,
                WorkLocation = dto.WorkLocation,
                IsAienabled = dto.IsAienabled,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                EmployerId = employerId
            };
            var created = await _jobPostRepo.AddJobPostAsync(jobPost);

            // Tạo liên kết CVTemplate nếu có
            if (dto.CvtemplateOfEmployerId.HasValue)
            {
                var cvTemplateLink = new CvTemplateForJobpost
                {
                    CvtemplateOfEmployerId = dto.CvtemplateOfEmployerId.Value,
                    JobPostId = created.JobPostId,
                    IsDisplay = true
                };
                _context.CvTemplateForJobposts.Add(cvTemplateLink);
                await _context.SaveChangesAsync();
            }
            // Gửi thông báo cho người dùng đang theo dõi công ty khi có bài đăng mới
            try
            {
                // Lấy companyId của employer
                int? companyId = await _context.Users
                    .Where(u => u.UserId == employerId)
                    .Select(u => u.CompanyId)
                    .FirstOrDefaultAsync();

                if (companyId.HasValue)
                {
                    // Lấy tên công ty để hiển thị nội dung thông báo
                    var companyName = await _context.Companies
                        .Where(c => c.CompanyId == companyId.Value)
                        .Select(c => c.CompanyName)
                        .FirstOrDefaultAsync();

                    // Lấy danh sách userId đang follow công ty
                    var followerUserIds = await _context.CompanyFollowers
                        .Where(cf => cf.CompanyId == companyId.Value && cf.IsActive == true)
                        .Join(_context.Users, cf => cf.UserId, u => u.UserId, (cf, u) => u)
                        .Join(_context.Accounts, u => u.AccountId, a => a.AccountId, (u, a) => new { u, a })
                        .Join(_context.Roles, ua => ua.a.RoleId, r => r.RoleId, (ua, r) => new { ua.u, r })
                        .Where(x => x.r.RoleName.ToLower() == "candidate")
                        .Select(x => x.u.UserId)
                        .ToListAsync();

                    if (followerUserIds.Count > 0)
                    {
                        string title = created.Title ?? "Tin tuyển dụng mới";
                        string company = string.IsNullOrWhiteSpace(companyName) ? "Công ty" : companyName!;
                        string content = $"{company} vừa đăng tin tuyển dụng: {title}";
                        string targetUrl = $"/Job/DetailJobPost/{created.JobPostId}"; // điều hướng tới chi tiết job post

                        foreach (var receiverUserId in followerUserIds)
                        {
                            await _notificationService.SendAsync(new CreateNotificationRequest(receiverUserId, content, targetUrl));
                        }
                    }
                }
            }
            catch
            {
                // Nuốt lỗi để tránh làm hỏng luồng tạo bài đăng; có thể log nếu cần
            }



            var detail = await GetJobPostDetailByIdAsync(created.JobPostId);
            return detail!;
        }

        public async Task<bool> DeleteJobPostAsync(int jobPostId, int employerUserId, bool isAdmin)
        {
            return await _jobPostRepo.SoftDeleteAsync(jobPostId, isAdmin ? null : employerUserId);
        }

        public async Task<bool> RestoreJobPostAsync(int jobPostId, int employerUserId, bool isAdmin)
        {
            return await _jobPostRepo.RestoreAsync(jobPostId, isAdmin ? null : employerUserId);
        }

        public async Task<ViewDetailJobPostDTO> UpdateJobPostAsync(UpdateJobPostDTO dto, int employerId)
        {
            var jobPost = await _jobPostRepo.GetJobPostByIdAsync(dto.JobPostId);
            if (jobPost == null)
                throw new Exception("JobPost không tồn tại.");
            if (jobPost.EmployerId != employerId)
                throw new UnauthorizedAccessException($"Access Denied: You do not have permission. (jobPost.EmployerId={jobPost.EmployerId}, employerId={employerId})");

            // Xử lý các trường liên kết nếu có nhập mới
            int? industryId = dto.IndustryId;
            if (!string.IsNullOrWhiteSpace(dto.NewIndustryName))
            {
                var industry = await _jobPostRepo.AddIndustryIfNotExistsAsync(dto.NewIndustryName.Trim());
                industryId = industry.IndustryId;
            }
            int? jobPositionId = dto.JobPositionId;
            if (!string.IsNullOrWhiteSpace(dto.NewJobPositionName))
            {
                var jobPosition = await _jobPostRepo.AddJobPositionIfNotExistsAsync(dto.NewJobPositionName.Trim(), industryId);
                jobPositionId = jobPosition.PositionId;
            }
            int? salaryRangeId = dto.SalaryRangeId;
            if (!string.IsNullOrWhiteSpace(dto.NewSalaryRange))
            {
                var parts = dto.NewSalaryRange.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                {
                    var salaryRange = await _jobPostRepo.AddSalaryRangeIfNotExistsAsync(min, max, parts[2]);
                    salaryRangeId = salaryRange.SalaryRangeId;
                }
            }
            int? provinceId = dto.ProvinceId;
            if (!string.IsNullOrWhiteSpace(dto.NewProvinceName))
            {
                var province = await _jobPostRepo.AddProvinceIfNotExistsAsync(dto.NewProvinceName.Trim());
                provinceId = province.ProvinceId;
            }
            int? experienceLevelId = dto.ExperienceLevelId;
            if (!string.IsNullOrWhiteSpace(dto.NewExperienceLevelName))
            {
                var exp = await _jobPostRepo.AddExperienceLevelIfNotExistsAsync(dto.NewExperienceLevelName.Trim());
                experienceLevelId = exp.ExperienceLevelId;
            }
            int? jobLevelId = dto.JobLevelId;
            if (!string.IsNullOrWhiteSpace(dto.NewJobLevelName))
            {
                var jl = await _jobPostRepo.AddJobLevelIfNotExistsAsync(dto.NewJobLevelName.Trim());
                jobLevelId = jl.JobLevelId;
            }
            int? employmentTypeId = dto.EmploymentTypeId;
            if (!string.IsNullOrWhiteSpace(dto.NewEmploymentTypeName))
            {
                var et = await _jobPostRepo.AddEmploymentTypeIfNotExistsAsync(dto.NewEmploymentTypeName.Trim());
                employmentTypeId = et.EmploymentTypeId;
            }

            // Cập nhật các trường
            jobPost.IndustryId = industryId;
            jobPost.JobPositionId = jobPositionId;
            jobPost.Title = dto.Title;
            jobPost.SalaryRangeId = salaryRangeId;
            jobPost.ProvinceId = provinceId;
            jobPost.ExperienceLevelId = experienceLevelId;
            jobPost.JobLevelId = jobLevelId;
            jobPost.EmploymentTypeId = employmentTypeId;
            jobPost.EndDate = dto.EndDate;
            jobPost.Description = dto.Description;
            jobPost.CandidaterRequirements = dto.CandidaterRequirements;
            jobPost.Interest = dto.Interest;
            jobPost.WorkLocation = dto.WorkLocation;
            jobPost.IsAienabled = dto.IsAienabled;
            jobPost.Status = dto.Status;
            jobPost.UpdatedDate = DateTime.UtcNow;

            var updated = await _jobPostRepo.UpdateJobPostAsync(jobPost);

            // Cập nhật liên kết CVTemplate nếu có
            if (dto.CvtemplateOfEmployerId.HasValue)
            {
                var existingLink = _context.CvTemplateForJobposts.FirstOrDefault(x => x.JobPostId == updated.JobPostId);
                if (existingLink != null)
                {
                    existingLink.CvtemplateOfEmployerId = dto.CvtemplateOfEmployerId.Value;
                    existingLink.IsDisplay = true;
                    _context.CvTemplateForJobposts.Update(existingLink);
                }
                else
                {
                    var cvTemplateLink = new CvTemplateForJobpost
                    {
                        CvtemplateOfEmployerId = dto.CvtemplateOfEmployerId.Value,
                        JobPostId = updated.JobPostId,
                        IsDisplay = true
                    };
                    _context.CvTemplateForJobposts.Add(cvTemplateLink);
                }
                await _context.SaveChangesAsync();
            }

            var detail = await GetJobPostDetailByIdAsync(updated.JobPostId);
            return detail!;
        }

        public async Task<(IEnumerable<JobPostListDTO> Posts, int TotalItems)> GetJobPostsByCompanyIdAsync(int companyId, int page, int pageSize)
        {
            var (posts, totalItems) = await _jobPostRepo.GetJobPostsByCompanyIdAsync(companyId, page, pageSize);

            var result = posts.Select(j => new JobPostListDTO
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company?.CompanyName ?? "Không rõ",
                Salary = j.SalaryRange != null
            ? $"{j.SalaryRange.MinSalary:N0} - {j.SalaryRange.MaxSalary:N0} {j.SalaryRange.Currency}"
            : "Thỏa thuận",
                Location = j.Province?.ProvinceName,
                EmploymentType = j.EmploymentType?.EmploymentTypeName,
                JobLevel = j.JobLevel?.JobLevelName,
                ExperienceLevel = j.ExperienceLevel?.ExperienceLevelName,
                Industry = j.Industry?.IndustryName,
                CreatedDate = j.CreatedDate,
                UpdatedDate = j.UpdatedDate
            });

            return (result, totalItems);
        }
        public async Task<List<CvSubmissionForJobPostDTO>> GetCvSubmissionsByJobPostIdAsync(int jobPostId)
        {
            var submissions = await _jobPostRepo.GetCvSubmissionsByJobPostIdAsync(jobPostId);
            foreach (var s in submissions)
            {
                Console.WriteLine($"[DEBUG] SubmissionId={s.SubmissionId}, CvId={s.CvId}, JobPostId={s.JobPostId}");
                var cvParsedDataQuery = _context.CvparsedData.Where(p => p.CvId == s.CvId && !p.IsDelete);
                var jobCriteriaQuery = _context.JobCriteria.Where(c => c.JobPostId == s.JobPostId && !c.IsDelete);
                Console.WriteLine($"[DEBUG] CvparsedDatum count: {cvParsedDataQuery.Count()}, JobCriteria count: {jobCriteriaQuery.Count()}");
                var cvParsedDataId = cvParsedDataQuery.OrderByDescending(p => p.ParsedAt).Select(p => (int?)p.CvparsedDataId).FirstOrDefault();
                var jobCriteriaId = jobCriteriaQuery.OrderByDescending(c => c.CreatedAt).Select(c => (int?)c.JobCriteriaId).FirstOrDefault();
                Console.WriteLine($"[DEBUG] Mapped CvParsedDataId={cvParsedDataId}, JobCriteriaId={jobCriteriaId}");
            }
            return submissions.Select(s => new CvSubmissionForJobPostDTO
            {
                CvParsedDataId = (s != null && s.CvId != null && _context?.CvparsedData != null && _context.CvparsedData.Any(p => p.CvId == s.CvId && !p.IsDelete))
    ? _context.CvparsedData
        .Where(p => p.CvId == s.CvId && !p.IsDelete)
        .OrderByDescending(p => p.ParsedAt)
        .Select(p => (int?)p.CvparsedDataId)
        .FirstOrDefault()
    : null,
                JobCriteriaId = (s != null && s.JobPostId != null && _context?.JobCriteria != null && _context.JobCriteria.Any(c => c.JobPostId == s.JobPostId && !c.IsDelete))
    ? _context.JobCriteria
        .Where(c => c.JobPostId == s.JobPostId && !c.IsDelete)
        .OrderByDescending(c => c.CreatedAt)
        .Select(c => (int?)c.JobCriteriaId)
        .FirstOrDefault()
    : null,
                SubmissionId = s.SubmissionId,
                SubmissionDate = s.SubmissionDate,
                CandidateId = s.SubmittedByUserId,
                CandidateName = s.SubmittedByUser != null ? s.SubmittedByUser.FullName : string.Empty,
                CvFileUrl = s.Cv != null ? s.Cv.FileUrl : string.Empty,
                Status = s.Status, // lấy Status mới
                TotalScore = s.MatchedCvandJobPost != null ? s.MatchedCvandJobPost.TotalScore : null, // lấy TotalScore từ bảng liên kết
                RecruiterNote = s.RecruiterNote,
                MatchedCvandJobPostId = s.MatchedCvandJobPostId
            }).ToList();
        }
        public Task<List<RelatedJobItemDTO>> GetRelatedJobsAsync(
            int industryId,
            int take = 5,
            int? excludeJobPostId = null,
            CancellationToken ct = default)
        {
            // guard nhỏ cho chắc
            if (industryId <= 0) return Task.FromResult(new List<RelatedJobItemDTO>());
            if (take <= 0) take = 5;

            // repo đã trả DTO => trả thẳng
            return _jobPostRepo.GetRelatedByIndustryAsync(industryId, take, excludeJobPostId, ct);
        }
    }
}
