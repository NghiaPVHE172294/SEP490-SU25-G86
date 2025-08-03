using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ParseCvRepository
{
    public class ParseCvRepository : IParseCvRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public ParseCvRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CvparsedDatum parsed)
        {
            _context.CvparsedData.Add(parsed);
            await _context.SaveChangesAsync();
        }
    }
}
