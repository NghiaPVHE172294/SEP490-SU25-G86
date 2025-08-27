using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository;
using System.Globalization;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int accountId)
        {
            var user = await _userRepo.GetUserByAccountIdAsync(accountId);
            if (user == null) return null;

            return new UserProfileDTO
            {
                Id = user.UserId,
                Avatar = user.Avatar,
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Account.Email,
                Phone = user.Phone,
                Dob = user.Dob?.ToString("yyyy-MM-dd"),
                LinkedIn = user.LinkedIn,
                Facebook = user.Facebook,
                AboutMe = user.AboutMe
            };
        }

        public async Task<bool> UpdateUserProfileAsync(int accountId, UpdateUserProfileDTO dto)
        {
            var user = await _userRepo.GetUserByAccountIdAsync(accountId);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                var fullName = dto.FullName.Trim();
                if (fullName.Length > 30)
                {
                    throw new ArgumentException("Tên không được vượt quá 30 ký tự.");
                }
                user.FullName = fullName;
            }

            if (dto.Address != null)
                user.Address = dto.Address;

            if (dto.Phone != null)
                user.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Dob))
            {
                if (DateTime.TryParseExact(dto.Dob, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDob))
                {
                    user.Dob = parsedDob;
                }
            }

            if (dto.LinkedIn != null)
                user.LinkedIn = dto.LinkedIn;

            if (dto.Facebook != null)
                user.Facebook = dto.Facebook;

            if (dto.AboutMe != null)
                user.AboutMe = dto.AboutMe;

            if (dto.AvatarFile != null)
            {

                if (!string.IsNullOrWhiteSpace(user.Avatar))
                {
                    await DeleteAvatarFromFirebaseStorageAsync(user.Avatar);
                }

                string firebaseUrl = await UploadAvatarToFirebaseStorage(dto.AvatarFile, user.UserId);
                user.Avatar = firebaseUrl;
            }

            await _userRepo.UpdateUserAsync(user);
            return true;
        }
        public async Task<bool> FollowCompanyAsync(int userId, int companyId)
        {
            return await _userRepo.FollowCompanyAsync(userId, companyId);
        }

        public async Task<bool> BlockCompanyAsync(int userId, int companyId, string? reason)
        {
            return await _userRepo.BlockCompanyAsync(userId, companyId, reason);
        }

        public async Task<FollowBlockStatusDTO> GetFollowBlockStatusAsync(int accountId, int companyId)
        {
            var isFollowing = await _userRepo.IsCompanyFollowedAsync(accountId, companyId);
            var isBlocked = await _userRepo.IsCompanyBlockedAsync(accountId, companyId);

            return new FollowBlockStatusDTO
            {
                IsFollowing = isFollowing,
                IsBlocked = isBlocked
            };
        }

        public async Task<string> UploadAvatarToFirebaseStorage(IFormFile file, int userId)
        {
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS")
                ?? "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";

            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET")
                ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";

            string folderName = "Image_storage/UserAvatar";

            // Khởi tạo FirebaseApp nếu cần
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
                {
                    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath),
                });
            }

            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath);
            var storage = Google.Cloud.Storage.V1.StorageClient.Create(credential);
            using var stream = file.OpenReadStream();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string objectName = $"{folderName}/{userId}_{timestamp}_{file.FileName}";

            var obj = await storage.UploadObjectAsync(
                bucket: bucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream
            );

            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }

        public async Task DeleteAvatarFromFirebaseStorageAsync(string avatarUrl)
        {
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS")
                ?? "D:\\Hoc_Tap\\SU25\\SEP490\\Project\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";

            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET")
                ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";

            // Giải mã objectName từ URL
            var uri = new Uri(avatarUrl);
            var objectNameEncoded = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("alt") == "media"
                ? uri.AbsolutePath.Replace("/v0/b/" + bucketName + "/o/", "")
                : null;
            if (string.IsNullOrEmpty(objectNameEncoded)) return;

            string objectName = Uri.UnescapeDataString(objectNameEncoded);

            var credential = GoogleCredential.FromFile(firebaseCredentialsPath);
            var storage = StorageClient.Create(credential);

            try
            {
                await storage.DeleteObjectAsync(bucketName, objectName);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Không sao nếu file không tồn tại
            }
        }
    }
}
