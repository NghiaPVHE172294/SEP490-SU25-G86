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

        public async Task AddAsync(int userId, string roleName, AddCvDTO dto, string fileUrl)
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
            await _repo.AddAsync(cv);
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

        public async Task<string> UploadFileToGoogleDrive(IFormFile file)
        {
            var serviceAccountJson = "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-2ad992eb4897.json"; // Đảm bảo đồng bộ với controller
            var googleDriveFolderId = "1jlghm3ntLE6JDPcwJqA2tlVrmCVBVpYM";
            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(serviceAccountJson)
                .CreateScoped(Google.Apis.Drive.v3.DriveService.Scope.Drive);

            var service = new Google.Apis.Drive.v3.DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "CVMatcher"
            });

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                Parents = new List<string> { googleDriveFolderId }
            };

            using (var stream = file.OpenReadStream())
            {
                var request = service.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink, webContentLink";
                request.SupportsAllDrives = true;
                try
                {
                    var uploadResult = await request.UploadAsync();
                    Console.WriteLine($"[GoogleDrive] Upload status: {uploadResult.Status}, Exception: {uploadResult.Exception}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GoogleDrive] Exception during upload: {ex}");
                    throw new Exception($"Google Drive upload exception: {ex.Message}", ex);
                }

                var uploadedFile = request.ResponseBody;
                if (uploadedFile == null)
                {
                    Console.WriteLine("[GoogleDrive] ResponseBody is null after upload. Possible cause: service account, folderId, or permission error.");
                    throw new Exception("Không upload được file lên Google Drive. ResponseBody null.");
                }
                if (string.IsNullOrEmpty(uploadedFile.Id))
                {
                    Console.WriteLine("[GoogleDrive] Uploaded file does not have an Id.");
                    throw new Exception("File upload lên Google Drive không có Id.");
                }

                // Set quyền public cho file
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                var permissionRequest = service.Permissions.Create(permission, uploadedFile.Id);
                permissionRequest.SupportsAllDrives = true;
                await permissionRequest.ExecuteAsync();

                return uploadedFile.WebViewLink;
            }
        }
    }
}