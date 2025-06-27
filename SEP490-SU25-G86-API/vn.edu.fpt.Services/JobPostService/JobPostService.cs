using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;


namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jobPostRepo;

        public JobPostService(IJobPostRepository jobPostRepo)
        {
            _jobPostRepo = jobPostRepo;
        }
        
        public async Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetPagedJobPostsAsync(page, pageSize, region);

            var result = posts.Select(j => new JobPostHomeDto
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company?.CompanyName ?? "Không rõ",
                Salary = j.SalaryRange != null
                         ? $"{j.SalaryRange.MinSalary:N0} - {j.SalaryRange.MaxSalary:N0} {j.SalaryRange.Currency}"
                         : "Thỏa thuận",
                Location = j.Province?.ProvinceName ?? "Không xác định"
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

        public async Task<IEnumerable<JobPostDTO>> GetByEmployerIdAsync(int employerId)
        {
            var posts = await _jobPostRepo.GetByEmployerIdAsync(employerId);
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
                CompanyName = jobPost.Employer?.Company?.CompanyName
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
            string? keyword = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetFilteredJobPostsAsync(
                page, pageSize, provinceId, industryId, employmentTypeIds, experienceLevelIds, jobLevelId, minSalary, maxSalary, datePostedRanges,keyword
            );

            var result = posts.Select(j => new JobPostListDTO
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.Company.CompanyName ?? "Không rõ",
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

        public async Task<IEnumerable<JobPostListDTO>> GetJobPostsByCompanyIdAsync(int companyId)
        {
            var jobs = await _jobPostRepo.GetJobPostsByCompanyIdAsync(companyId);
            return jobs.Select(j => new JobPostListDTO
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
        }
    }

}
