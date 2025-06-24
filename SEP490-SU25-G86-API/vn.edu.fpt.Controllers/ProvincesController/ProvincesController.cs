using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ProvinceDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ProvinceServices;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.ProvincesController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var provinces = await _provinceService.GetAllAsync();
            return Ok(provinces);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddProvinceDTO dto)
        {
            var id = await _provinceService.AddAsync(dto);
            return Ok(id);
        }
    }
}
