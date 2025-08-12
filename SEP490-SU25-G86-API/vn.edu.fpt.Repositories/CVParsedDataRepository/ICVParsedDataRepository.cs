using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVParsedDataRepository
{
    public interface ICVParsedDataRepository
    {
        Task<CvparsedDatum> AddAsync(CvparsedDatum entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
