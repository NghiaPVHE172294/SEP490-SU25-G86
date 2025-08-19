using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace vn.edu.fpt.Services.CvTemplateUpload
{
    public interface ICvTemplateUploadService
    {
        Task<(string pdfUrl, string docUrl, string imgUrl)> UploadCvTemplateAsync(IFormFile pdfFile, IFormFile docFile, IFormFile previewImage);
    }

    public class CvTemplateUploadService : ICvTemplateUploadService
    {
        public async Task<(string pdfUrl, string docUrl, string imgUrl)> UploadCvTemplateAsync(IFormFile pdfFile, IFormFile docFile, IFormFile previewImage)
        {
            // 1. Lưu file tạm
            var tempPdfPath = Path.GetTempFileName() + Path.GetExtension(pdfFile.FileName);
            string tempDocPath = null;
if (docFile != null)
    tempDocPath = Path.GetTempFileName() + Path.GetExtension(docFile.FileName);
            var tempImgPath = Path.GetTempFileName() + Path.GetExtension(previewImage.FileName);
            using (var imgStream = new FileStream(tempImgPath, FileMode.Create))
                await previewImage.CopyToAsync(imgStream);
            using (var pdfStream = new FileStream(tempPdfPath, FileMode.Create))
                await pdfFile.CopyToAsync(pdfStream);
            if (docFile != null)
{
    using (var docStream = new FileStream(tempDocPath, FileMode.Create))
        await docFile.CopyToAsync(docStream);
}

         

            // 3. Upload lên Firebase Storage
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS") ?? "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";
            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET") ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";
            string folderName = "CV storage/AdminUploadCVTemplate";
            // Khởi tạo FirebaseApp nếu chưa có
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
                {
                    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath),
                });
            }
            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseCredentialsPath);
            var storage = Google.Cloud.Storage.V1.StorageClient.Create(credential);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            // PDF
            string pdfObjectName = $"{folderName}/{timestamp}_{pdfFile.FileName}";
            string pdfUrl;
            using (var pdfUp = new FileStream(tempPdfPath, FileMode.Open, FileAccess.Read))
            {
                var pdfObj = await storage.UploadObjectAsync(
                    bucket: bucketName,
                    objectName: pdfObjectName,
                    contentType: pdfFile.ContentType,
                    source: pdfUp
                );
                pdfUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(pdfObjectName)}?alt=media";
            }
            // DOCX
            string docUrl = null;
            if (docFile != null)
            {
                string docObjectName = $"{folderName}/{timestamp}_{docFile.FileName}";
                using (var docUp = new FileStream(tempDocPath, FileMode.Open, FileAccess.Read))
                {
                    var docObj = await storage.UploadObjectAsync(
                        bucket: bucketName,
                        objectName: docObjectName,
                        contentType: docFile.ContentType,
                        source: docUp
                    );
                    docUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(docObjectName)}?alt=media";
                }
            }
            // IMG
            string imgObjectName = $"{folderName}/{timestamp}_preview{Path.GetExtension(previewImage.FileName)}";
            string imgUrl;
            using (var imgUp = new FileStream(tempImgPath, FileMode.Open, FileAccess.Read))
            {
                var imgObj = await storage.UploadObjectAsync(
                    bucket: bucketName,
                    objectName: imgObjectName,
                    contentType: previewImage.ContentType,
                    source: imgUp
                );
                imgUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(imgObjectName)}?alt=media";
            }

            // 4. Xóa file tạm
            File.Delete(tempPdfPath);
            if (tempDocPath != null)
    File.Delete(tempDocPath);
            File.Delete(tempImgPath);

            // 5. Trả về URL
            return (pdfUrl, docUrl, imgUrl);
        }
    }
}
