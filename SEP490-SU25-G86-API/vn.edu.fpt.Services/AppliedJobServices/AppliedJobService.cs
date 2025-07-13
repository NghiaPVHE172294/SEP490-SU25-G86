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
                Status = s.JobPost?.Status,
                SubmissionDate = s.SubmissionDate,
                CvId = s.CvId,
                CvName = s.Cv?.Cvname,
                CvFileUrl = s.Cv?.FileUrl,
                CvNotes = s.Cv?.Notes,
                SourceType = s.SourceType
            });
        }

        public async Task AddSubmissionAsync(Cvsubmission submission)
        {
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
            var submission = await _context.Cvsubmissions.FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.SubmittedByUserId == userId && !s.IsDelete);
            if (submission == null)
                return false;
            submission.CvId = newCvId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WithdrawApplicationAsync(int submissionId, int userId)
        {
            var submission = await _context.Cvsubmissions.FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.SubmittedByUserId == userId && !s.IsDelete);
            if (submission == null)
                return false;
            submission.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 