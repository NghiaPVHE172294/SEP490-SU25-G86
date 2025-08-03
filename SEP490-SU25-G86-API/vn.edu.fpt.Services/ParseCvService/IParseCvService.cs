using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ParseCvService
{
    public interface IParseCvService
    {
            Task<CvparsedDatum> ParseAndSaveAsync(IFormFile file);
    }
}
