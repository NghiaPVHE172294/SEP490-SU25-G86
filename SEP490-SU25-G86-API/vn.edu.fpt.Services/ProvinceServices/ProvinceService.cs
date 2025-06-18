using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.ProvinceDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepositories;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ProvinceServices
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _repository;

        public ProvinceService(IProvinceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProvinceGetDTO>> GetAllAsync()
        {
            var provinces = await _repository.GetAllAsync();

            return provinces.Select(p => new ProvinceGetDTO
            {
                ProvinceId = p.ProvinceId,
                ProvinceName = p.ProvinceName,
                Region = p.Region
            });
        }
    }
}
