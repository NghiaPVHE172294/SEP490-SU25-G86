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
            var result = new List<CvDTO>();
            foreach (var c in cvs)
            {
                bool isUsed = await _repo.HasBeenUsedInSubmissionAsync(c.CvId);
                result.Add(new CvDTO
                {
                    CvId = c.CvId,
                    FileName = Path.GetFileName(c.FileUrl),
                    FileUrl = c.FileUrl,
                    Notes = c.Notes,
                    UploadDate = c.UploadDate,
                    UpdatedDate = c.UploadDate,
                    CVName = c.Cvname,
                    IsUsed = isUsed
                });
            }
            return result;
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

        public async Task<int> AddAsync(int userId, string roleName, AddCvDTO dto, string fileUrl)
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
            int maxCv = 20;
            if (roleName == "EMPLOYER" || roleName == "RECRUITER")
            {
                maxCv = 100;
            }
            int currentCount = await _repo.CountByUserAsync(userId);
            if (currentCount >= maxCv)
                throw new Exception($"[BR-10] Bạn đã đạt đến số lượng CV tối đa cho phép ({maxCv}).");

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
            return await _repo.AddAsync(cv); // trả về CvId
        }

        public async Task DeleteAsync(int userId, int cvId)
        {
            var cv = await _repo.GetByIdAsync(cvId);
            if (cv == null || cv.UploadByUserId != userId) throw new Exception("Not found or not allowed");
            // Prevent deletion if CV has been used for job applications
            if (await _repo.HasBeenUsedInSubmissionAsync(cvId))
                throw new Exception("CV này đang được dùng để ứng tuyển công việc (chưa rút đơn) và không thể xóa. Nếu bạn đã rút đơn ở tất cả các vị trí, bạn có thể xóa CV này.");
            await _repo.DeleteAsync(cv);
        }

        public async Task UpdateCvNameAsync(int cvId, string newName)
        {
            var cv = await _repo.GetByIdAsync(cvId);
            if (cv == null) throw new Exception("CV không tồn tại");
            cv.Cvname = newName;
            await _repo.UpdateAsync(cv);
        }

        public async Task<string> UploadFileToFirebaseStorage(IFormFile file, int candidateId)
        {
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS") ?? "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";
            //string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS") ?? "C:\\Users\\ADMIN\\Downloads\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";
            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET") ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";
            string folderName = "CV storage";
            // Khởi tạo FirebaseApp nếu chưa có
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
                {
                    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath),
                });
            }
            // Tạo credential cho StorageClient
            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath);
            var storage = Google.Cloud.Storage.V1.StorageClient.Create(credential);
            using var stream = file.OpenReadStream();
            // Tạo objectName duy nhất: "CV storage/{candidateId}_{yyyyMMddHHmmss}_{fileName}"
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string objectName = $"{folderName}/{candidateId}_{timestamp}_{file.FileName}";
            // Always set Content-Type to application/pdf for PDF files
            var contentType = file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : file.ContentType;
            var obj = await storage.UploadObjectAsync(
                bucket: bucketName,
                objectName: objectName,
                contentType: contentType,
                source: stream
            );
            // Trả về public URL đúng chuẩn Firebase
            var fileUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
            return fileUrl;
        }
        // Overload cũ cho các controller chưa truyền candidateId
        public async Task<string> UploadFileToFirebaseStorage(IFormFile file)
        {
            // Có thể throw exception hoặc dùng candidateId = 0 (tùy logic của bạn)
            return await UploadFileToFirebaseStorage(file, 0);
        }
    }
}