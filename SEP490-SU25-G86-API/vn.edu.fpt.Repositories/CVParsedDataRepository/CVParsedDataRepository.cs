using SEP490_SU25_G86_API.Models;
using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVParsedDataRepository
{
    public class CVParsedDataRepository : ICVParsedDataRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public CVParsedDataRepository(SEP490_G86_CvMatchContext context) => _context = context;


        public async Task<CvparsedDatum> AddAsync(CvparsedDatum e, CancellationToken ct = default)
        {
            _context.CvparsedData.Add(e);
            await _context.SaveChangesAsync(ct);
            return e;
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }
}
