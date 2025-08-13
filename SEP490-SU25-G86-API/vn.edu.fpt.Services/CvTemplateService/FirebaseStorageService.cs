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
            
            // Note: File sẽ được set public thông qua Firebase Storage Rules
            // Không cần set ACL programmatically nếu Firebase Rules đã cho phép public read

            var fileUrl = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media";
            return fileUrl;
        }
    }
}
