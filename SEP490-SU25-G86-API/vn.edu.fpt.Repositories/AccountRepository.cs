using SEP490_SU25_G86_API.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories
{
    public class AccountRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public AccountRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public Account? GetByEmail(string email)
        {
            return _context.Accounts.Include(a => a.Role).FirstOrDefault(a => a.Email == email);
        }
    }
} 