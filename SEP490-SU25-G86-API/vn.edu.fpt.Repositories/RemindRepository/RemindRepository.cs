using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RemindRepository
{
    public class RemindRepository : IRemindRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public RemindRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task SaveRemindAsync(Remind remind)
        {
            await _context.Reminds.AddAsync(remind);
            await _context.SaveChangesAsync();
        }
    }
}
