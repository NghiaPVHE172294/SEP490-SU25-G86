using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobCriterionRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobCriterionService
{
    public class JobCriterionService : IJobCriterionService
    {
        private readonly IJobCriterionRepository _repository;
        private readonly SEP490_G86_CvMatchContext _context;
        public JobCriterionService(IJobCriterionRepository repository, SEP490_G86_CvMatchContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<JobCriterionDTO>> GetJobCriteriaByUserIdAsync(int userId)
        {
            var list = await _repository.GetJobCriteriaByUserIdAsync(userId);
            return list.Select(jc => new JobCriterionDTO
            {
                JobCriteriaId = jc.JobCriteriaId,
                JobPostId = jc.JobPostId,
                RequiredExperience = jc.RequiredExperience,
                RequiredSkills = jc.RequiredSkills,
                EducationLevel = jc.EducationLevel,
                RequiredJobTitles = jc.RequiredJobTitles,
                PreferredLanguages = jc.PreferredLanguages,
                PreferredCertifications = jc.PreferredCertifications,
                Address = jc.Address,
                Summary = jc.Summary,
                WorkHistory = jc.WorkHistory,
                Projects = jc.Projects,
                
                
                CreatedAt = jc.CreatedAt
            }).ToList();
        }

        public async Task<JobCriterionDTO> AddJobCriterionAsync(AddJobCriterionDTO dto, int userId)
        {
            // Kiểm tra quyền sở hữu JobPost
            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(jp => jp.JobPostId == dto.JobPostId && !jp.IsDelete);
            if (jobPost == null)
                throw new Exception("JobPost không tồn tại.");
            if (jobPost.EmployerId != userId)
                throw new UnauthorizedAccessException($"Access Denied: You do not have permission. (jobPost.EmployerId={jobPost.EmployerId}, userId={userId})");
            var entity = new JobCriterion
            {
                JobPostId = dto.JobPostId,
                RequiredExperience = dto.RequiredExperience,
                RequiredSkills = dto.RequiredSkills,
                EducationLevel = dto.EducationLevel,
                RequiredJobTitles = dto.RequiredJobTitles,
                PreferredLanguages = dto.PreferredLanguages,
                PreferredCertifications = dto.PreferredCertifications,
                Address = dto.Address,
                Summary = dto.Summary,
                WorkHistory = dto.WorkHistory,
                Projects = dto.Projects,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                IsDelete = false
            };
            var result = await _repository.AddJobCriterionAsync(entity);
            return new JobCriterionDTO
            {
                JobCriteriaId = result.JobCriteriaId,
                JobPostId = result.JobPostId,
                RequiredExperience = result.RequiredExperience,
                RequiredSkills = result.RequiredSkills,
                EducationLevel = result.EducationLevel,
                RequiredJobTitles = result.RequiredJobTitles,
                PreferredLanguages = result.PreferredLanguages,
                PreferredCertifications = result.PreferredCertifications,
                Address = result.Address,
                Summary = result.Summary,
                WorkHistory = result.WorkHistory,
                Projects = result.Projects,
                
                
                CreatedAt = result.CreatedAt
            };
        }
// ... existing code ...
        public async Task<JobCriterionDTO> UpdateJobCriterionAsync(UpdateJobCriterionDTO dto, int userId)
        {
            var jobCriterion = await _context.JobCriteria.FirstOrDefaultAsync(jc => jc.JobCriteriaId == dto.JobCriteriaId && !jc.IsDelete);
            if (jobCriterion == null)
                throw new Exception("JobCriterion không tồn tại.");
            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(jp => jp.JobPostId == dto.JobPostId && !jp.IsDelete);
            if (jobPost == null)
                throw new Exception("JobPost không tồn tại.");
            if (jobPost.EmployerId != userId)
                throw new UnauthorizedAccessException($"Access Denied: You do not have permission. (jobPost.EmployerId={jobPost.EmployerId}, userId={userId})");
            // Cập nhật các trường
            jobCriterion.JobPostId = dto.JobPostId;
            jobCriterion.RequiredExperience = dto.RequiredExperience;
            jobCriterion.RequiredSkills = dto.RequiredSkills;
            jobCriterion.EducationLevel = dto.EducationLevel;
            jobCriterion.RequiredJobTitles = dto.RequiredJobTitles;
            jobCriterion.PreferredLanguages = dto.PreferredLanguages;
            jobCriterion.PreferredCertifications = dto.PreferredCertifications;
            jobCriterion.Address = dto.Address;
            jobCriterion.Summary = dto.Summary;
            jobCriterion.WorkHistory = dto.WorkHistory;
            jobCriterion.Projects = dto.Projects;
            var result = await _repository.UpdateJobCriterionAsync(jobCriterion);
            return new JobCriterionDTO
            {
                JobCriteriaId = result.JobCriteriaId,
                JobPostId = result.JobPostId,
                RequiredExperience = result.RequiredExperience,
                RequiredSkills = result.RequiredSkills,
                EducationLevel = result.EducationLevel,
                RequiredJobTitles = result.RequiredJobTitles,
                PreferredLanguages = result.PreferredLanguages,
                PreferredCertifications = result.PreferredCertifications,
                Address = result.Address,
                Summary = result.Summary,
                WorkHistory = result.WorkHistory,
                Projects = result.Projects,
                
                
                CreatedAt = result.CreatedAt
            };
        }
        // ... existing code ...
        public async Task<bool> SoftDeleteJobCriterionAsync(int id, int userId)
        {
            var jobCriterion = await _context.JobCriteria
                .FirstOrDefaultAsync(jc => jc.JobCriteriaId == id && !jc.IsDelete);

            if (jobCriterion == null)
            {
                // Nếu không tìm thấy JobCriterion
                return false;
            }

            // Cập nhật xóa mềm
            jobCriterion.IsDelete = true;
            _context.JobCriteria.Update(jobCriterion);
            await _context.SaveChangesAsync();  // Lưu thay đổi

            return true;
        }



        // Restore JobCriterion
        public async Task<bool> RestoreJobCriterionAsync(int id, int userId)
        {
            // Kiểm tra nếu JobCriterion đã bị xóa mềm (IsDelete = true)
            var jobCriterion = await _context.JobCriteria
                .FirstOrDefaultAsync(jc => jc.JobCriteriaId == id && jc.IsDelete); // Chỉ tìm JobCriterion đã bị xóa mềm

            if (jobCriterion == null)
            {
                // Nếu không tìm thấy JobCriterion đã bị xóa mềm
                return false; // Return false nếu không tìm thấy JobCriterion hoặc đã bị xóa hoàn toàn
            }

            // Khôi phục JobCriterion
            jobCriterion.IsDelete = false;
            _context.JobCriteria.Update(jobCriterion);
            await _context.SaveChangesAsync();  // Lưu thay đổi

            return true;
        }


    }
} 