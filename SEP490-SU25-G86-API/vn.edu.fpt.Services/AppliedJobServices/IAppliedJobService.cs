using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.Models;
using Microsoft.AspNetCore.Http;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices
{
    public interface IAppliedJobService
    {
        Task<IEnumerable<AppliedJobDTO>> GetAppliedJobsByUserIdAsync(int userId);
        Task AddSubmissionAsync(Cvsubmission submission);
        Task<string> UploadFileToGoogleDrive(IFormFile file);
        Task<int> AddCvAndGetIdAsync(Cv cv);
        Task<bool> HasUserAppliedToJobAsync(int userId, int jobPostId);
        Task<bool> UpdateAppliedCvAsync(int submissionId, int newCvId, int userId);
        Task<bool> WithdrawApplicationAsync(int submissionId, int userId);
    }
} 