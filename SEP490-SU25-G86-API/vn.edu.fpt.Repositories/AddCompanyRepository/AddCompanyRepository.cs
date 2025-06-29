using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository
{
    public class AddCompanyRepository : IAddCompanyRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public AddCompanyRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }
    }
}
