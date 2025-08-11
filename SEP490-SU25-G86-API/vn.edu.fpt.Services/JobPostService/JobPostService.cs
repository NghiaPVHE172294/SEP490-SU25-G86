using SEP490_SU25_G86_API.Models;
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

        public JobPostService(IJobPostRepository jobPostRepo, IBlockedCompanyRepository blockedCompanyRepo, SEP490_G86_CvMatchContext context)
        {
            _jobPostRepo = jobPostRepo;
            _blockedCompanyRepo = blockedCompanyRepo;
            _context = context;
        }
        
        public async Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null, int? salaryRangeId = null, int? experienceLevelId = null, int? candidateId = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetPagedJobPostsAsync(page, pageSize, region, salaryRangeId, experienceLevelId, candidateId);

            // Lọc bỏ job posts từ blocked companies nếu có candidateId
            if (candidateId.HasValue)
            {
                var blockedCompanies = await _blockedCompanyRepo.GetBlockedCompaniesByCandidateIdAsync(candidateId.Value);
                var blockedCompanyIds = blockedCompanies.Select(bc => bc.CompanyId).ToHashSet();
                
                posts = posts.Where(j => j.Employer?.CompanyId == null || !blockedCompanyIds.Contains(j.Employer.CompanyId.Value));
                totalItems = posts.Count(); // Cập nhật lại totalItems sau khi lọc
            }

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
            return posts.Select(j => new JobPostListDTO
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company?.CompanyName ?? j.Employer?.FullName ?? "Không rõ",
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
                WorkLocation = j.WorkLocation
            });
        }


        public async Task<ViewDetailJobPostDTO?> GetJobPostDetailByIdAsync(int jobPostId)
        {
            var jobPost = await _jobPostRepo.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return null;
            return new ViewDetailJobPostDTO
            {
                JobPostId = jobPost.JobPostId,
                IndustryId = jobPost.IndustryId,
                JobPositionId = jobPost.JobPositionId,
                Title = jobPost.Title,
                SalaryRangeId = jobPost.SalaryRangeId,
                ProvinceId = jobPost.ProvinceId,
                ExperienceLevelId = jobPost.ExperienceLevelId,
                JobLevelId = jobPost.JobLevelId,
                EmploymentTypeId = jobPost.EmploymentTypeId,
                EndDate = jobPost.EndDate,
                Description = jobPost.Description,
                CandidaterRequirements = jobPost.CandidaterRequirements,
                Interest = jobPost.Interest,
                WorkLocation = jobPost.WorkLocation,
                IsAienabled = jobPost.IsAienabled,
                Status = jobPost.Status,
                CreatedDate = jobPost.CreatedDate,
                UpdatedDate = jobPost.UpdatedDate,
                EmployerId = jobPost.EmployerId,
                EmployerName = jobPost.Employer?.FullName,
                IndustryName = jobPost.Industry?.IndustryName,
                JobPositionName = jobPost.JobPosition?.PostitionName,
                SalaryRangeName = jobPost.SalaryRange != null ? $"{jobPost.SalaryRange.MinSalary:N0} - {jobPost.SalaryRange.MaxSalary:N0} {jobPost.SalaryRange.Currency}" : null,
                ProvinceName = jobPost.Province?.ProvinceName,
                ExperienceLevelName = jobPost.ExperienceLevel?.ExperienceLevelName,
                JobLevelName = jobPost.JobLevel?.JobLevelName,
                EmploymentTypeName = jobPost.EmploymentType?.EmploymentTypeName,
                CompanyName = jobPost.Employer?.Company?.CompanyName ?? jobPost.Employer?.FullName ?? "Không rõ"
            };
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
                CandidateName = s.SubmittedByUser != null ? s.SubmittedByUser.FullName : string.Empty,
                CvFileUrl = s.Cv != null ? s.Cv.FileUrl : string.Empty,
                Status = s.Status, // lấy Status mới
                TotalScore = s.MatchedCvandJobPost != null ? s.MatchedCvandJobPost.TotalScore : null, // lấy TotalScore từ bảng liên kết
                RecruiterNote = s.RecruiterNote,
                MatchedCvandJobPostId = s.MatchedCvandJobPostId
            }).ToList();
        }
    }

}
