using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using Microsoft.AspNetCore.Http;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService
{
    public class CvService : ICvService
    {
        private readonly ICvRepository _repo;
        public CvService(ICvRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CvDTO>> GetAllByUserAsync(int userId)
        {
            var cvs = await _repo.GetAllByUserAsync(userId);
            return cvs.Select(c => new CvDTO
            {
                CvId = c.CvId,
                FileName = Path.GetFileName(c.FileUrl),
                FileUrl = c.FileUrl,
                Notes = c.Notes,
                UploadDate = c.UploadDate,
                UpdatedDate = c.UploadDate,
                CVName = c.Cvname
            }).ToList();
        }

        public async Task<CvDTO?> GetByIdAsync(int cvId)
        {
            var c = await _repo.GetByIdAsync(cvId);
            if (c == null) return null;
            return new CvDTO
            {
                CvId = c.CvId,
                FileName = Path.GetFileName(c.FileUrl),
                FileUrl = c.FileUrl,
                Notes = c.Notes,
                UploadDate = c.UploadDate,
                UpdatedDate = c.UploadDate,
                CVName = c.Cvname
            };
        }

        public async Task AddAsync(int userId, AddCvDTO dto, string fileUrl)
        {
            if (dto.File == null)
                throw new Exception("Bạn chưa chọn file CV để upload.");
            // Validate file size (≤ 5MB)
            if (dto.File.Length > 5 * 1024 * 1024)
                throw new Exception("[BR-08] CV file size must not exceed 5MB.");
            // Validate file type (PDF)
            if (!dto.File.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && dto.File.ContentType != "application/pdf")
                throw new Exception("[BR-09] CV file must be in PDF format.");
            // Validate số lượng CV theo role
            // (Giả sử bạn lấy role từ DB hoặc truyền vào, ở đây demo lấy từ DB qua userId)
            int maxCv = 5; // default Candidate
            // TODO: Lấy role thực tế từ DB, ví dụ:
            // var user = ... lấy user từ DB theo userId
            // if (user.Role == "EMPLOYER") maxCv = 200;
            // Ở đây giả lập: nếu userId < 10000 là candidate, >= 10000 là employer
            if (userId >= 10000) maxCv = 200;
            int currentCount = await _repo.CountByUserAsync(userId);
            if (currentCount >= maxCv)
                throw new Exception($"[BR-10] You have reached the maximum number of CVs ({maxCv}).");

            var cv = new Cv
            {
                UploadByUserId = userId,
                CandidateId = userId,
                FileUrl = fileUrl,
                Notes = dto.Notes,
                UploadDate = DateTime.UtcNow,
                IsDelete = false,
                Cvname = string.IsNullOrEmpty(dto.CVName) ? Path.GetFileName(fileUrl) : dto.CVName
            };
            await _repo.AddAsync(cv);
        }

        public async Task DeleteAsync(int userId, int cvId)
        {
            var cv = await _repo.GetByIdAsync(cvId);
            if (cv == null || cv.UploadByUserId != userId) throw new Exception("Not found or not allowed");
            await _repo.DeleteAsync(cv);
        }

        public async Task UpdateCvNameAsync(int cvId, string newName)
        {
            var cv = await _repo.GetByIdAsync(cvId);
            if (cv == null) throw new Exception("CV không tồn tại");
            cv.Cvname = newName;
            await _repo.UpdateAsync(cv);
        }
    }
}