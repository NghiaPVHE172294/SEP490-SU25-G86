using System.Threading.Tasks;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.Services.GeminiCvJobMatchingService
{
    public interface IGeminiCvJobMatchingService
    {
        Task<MatchedCvandJobPost?> CompareCvWithJobCriteriaAsync(int cvParsedDataId, int jobCriteriaId);
    }
}
