using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.Models;
using Microsoft.AspNetCore.Http;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;
using System;
using Microsoft.EntityFrameworkCore;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices
{
    public class AppliedJobService : IAppliedJobService
    {
        private readonly IAppliedJobRepository _appliedJobRepo;
        private readonly ICvRepository _cvRepo;
        private readonly SEP490_G86_CvMatchContext _context;

        public AppliedJobService(IAppliedJobRepository appliedJobRepo, ICvRepository cvRepo, SEP490_G86_CvMatchContext context)
        {
            _appliedJobRepo = appliedJobRepo;
            _cvRepo = cvRepo;
            _context = context;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAppliedJobsByUserIdAsync(int userId)
        {
            var submissions = await _appliedJobRepo.GetByUserIdAsync(userId);
            return submissions.Select(s => new AppliedJobDTO
            {
                SubmissionId = s.SubmissionId,
                JobPostId = s.JobPostId ?? 0,
                Title = s.JobPost?.Title ?? string.Empty,
                WorkLocation = s.JobPost?.WorkLocation,
                Status = s.Status,
                SubmissionDate = s.SubmissionDate,
                CvId = s.CvId,
                CvName = s.Cv?.Cvname,
                CvFileUrl = s.Cv?.FileUrl,
                CvNotes = s.Cv?.Notes,
                SourceType = s.SourceType,
                IsDelete = s.IsDelete,
                CompanyName = s.JobPost?.Employer?.Company?.CompanyName ?? "Không rõ",
                CompanyLogoUrl = s.JobPost?.Employer?.Company?.LogoUrl,
                Salary = s.JobPost?.SalaryRange != null
                    ? ($"{s.JobPost.SalaryRange.MinSalary:N0} - {s.JobPost.SalaryRange.MaxSalary:N0} {s.JobPost.SalaryRange.Currency}")
                    : "Thỏa thuận"
            });
        }

        public async Task AddSubmissionAsync(Cvsubmission submission)
        {
            if (submission == null)
                throw new ArgumentNullException(nameof(submission), "Submission không được null");
            _context.Cvsubmissions.Add(submission);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddSubmissionAsync] Exception: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[AddSubmissionAsync] InnerException: {ex.InnerException.Message}");
                    throw new Exception($"Lỗi khi lưu submission: {ex.InnerException.Message}", ex);
                }
                throw;
            }
        }

        public async Task<string> UploadFileToGoogleDrive(IFormFile file)
        {
            // TODO: Thay thế bằng code upload thực tế của bạn
            // Ví dụ trả về link giả lập
            return "/uploads/" + file.FileName;
        }

        public async Task<int> AddCvAndGetIdAsync(Cv cv)
        {
            // Đếm số lượng CV đã upload của ứng viên này
            int maxCv = 20;
            var query = _context.Cvs.Where(x => x.CandidateId == cv.CandidateId && !x.IsDelete);
            int currentCount = await query.CountAsync();
            if (currentCount >= maxCv)
                throw new Exception($"[BR-10] Bạn đã đạt đến số lượng CV tối đa cho phép ({maxCv}).");

            _context.Cvs.Add(cv);
            await _context.SaveChangesAsync();
            return cv.CvId;
        }

        public async Task<bool> HasUserAppliedToJobAsync(int userId, int jobPostId)
        {
            return await _appliedJobRepo.HasUserAppliedToJobAsync(userId, jobPostId);
        }

        public async Task<bool> UpdateAppliedCvAsync(int submissionId, int newCvId, int userId)
        {
            Console.WriteLine($"[UpdateAppliedCvAsync] Starting update - SubmissionId: {submissionId}, NewCvId: {newCvId}, UserId: {userId}");
            
            // Validate that the CV exists and belongs to the user
            var cv = await _cvRepo.GetByIdAsync(newCvId);
            if (cv == null)
            {
                Console.WriteLine($"[UpdateAppliedCvAsync] CV with ID {newCvId} not found");
                return false;
            }
            
            if (cv.UploadByUserId != userId)
            {
                Console.WriteLine($"[UpdateAppliedCvAsync] CV {newCvId} belongs to user {cv.UploadByUserId}, not {userId}");
                return false;
            }

            // DEBUG: Log chi tiết submission theo SubmissionId
            var existingSubmission = await _context.Cvsubmissions
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
            if (existingSubmission != null)
            {
                Console.WriteLine($"[DEBUG] SubmissionId: {existingSubmission.SubmissionId}, SubmittedByUserId: {existingSubmission.SubmittedByUserId}, IsDelete: {existingSubmission.IsDelete}");
            }

            // Find the submission and validate ownership
            var submission = await _context.Cvsubmissions
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.SubmittedByUserId == userId && !s.IsDelete);
            
            if (submission == null)
            {
                Console.WriteLine($"[UpdateAppliedCvAsync] Submission {submissionId} not found for user {userId}");
                
                // Check if submission exists but belongs to different user
                if (existingSubmission != null)
                {
                    Console.WriteLine($"[UpdateAppliedCvAsync] Submission {submissionId} exists but belongs to user {existingSubmission.SubmittedByUserId}");
                }
                
                return false;
            }

            Console.WriteLine($"[UpdateAppliedCvAsync] Found submission {submissionId}, current CV: {submission.CvId}, updating to {newCvId}");
            
            // Update the CV
            submission.CvId = newCvId;
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"[UpdateAppliedCvAsync] Successfully updated submission {submissionId}");
            return true;
        }

        public async Task<bool> WithdrawApplicationAsync(int submissionId, int userId)
        {
            var query = _context.Cvsubmissions.Where(s => s.SubmissionId == submissionId && s.SubmittedByUserId == userId && !s.IsDelete);
            var submission = await query.FirstOrDefaultAsync();
            if (submission == null)
                return false;
            submission.IsDelete = true;
            submission.Status = "Hồ sơ đã rút";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}