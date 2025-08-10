using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.Services.CvTemplateService
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
    }

    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration configuration)
        {
            // Đọc từ biến môi trường với fallback value như CvService
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS") ?? "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";
            _bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET") ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";
            
            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath);
            _storageClient = Google.Cloud.Storage.V1.StorageClient.Create(credential);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileName = $"CV storage/EmployerUploadCVTemplate/{timestamp}_{Guid.NewGuid()}_{file.FileName}";
            
            using var stream = file.OpenReadStream();
            var obj = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: fileName,
                contentType: file.ContentType,
                source: stream
            );
            
            // Set file thành public để Google Docs viewer có thể truy cập
            try
            {
                await _storageClient.UpdateObjectAsync(new Google.Cloud.Storage.V1.Object
                {
                    Bucket = _bucketName,
                    Name = fileName,
                    Acl = new List<Google.Cloud.Storage.V1.ObjectAccessControl>
                    {
                        new Google.Cloud.Storage.V1.ObjectAccessControl
                        {
                            Entity = "allUsers",
                            Role = "READER"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng vẫn trả về URL (có thể Firebase Rules đã cho phép public)
                Console.WriteLine($"Không thể set public access: {ex.Message}");
            }
            
            // Trả về public URL đúng chuẩn Firebase giống CvService
            var fileUrl = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media";
            return fileUrl;
        }
    }
}
