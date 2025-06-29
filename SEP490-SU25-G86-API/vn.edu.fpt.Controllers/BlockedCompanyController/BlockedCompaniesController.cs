using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BlockedCompanyService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.BlockedCompanyController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlockedCompaniesController : ControllerBase
    {
        private readonly IBlockedCompanyService _service;
        public BlockedCompaniesController(IBlockedCompanyService service)
        {
            _service = service;
        }

        [HttpGet("user/{candidateId}")]
        public async Task<ActionResult<IEnumerable<BlockedCompanyDTO>>> GetByCandidateId(int candidateId)
        {
            var list = await _service.GetBlockedCompaniesByCandidateIdAsync(candidateId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BlockedCompany entity)
        {
            var result = await _service.AddAsync(entity);
            return Ok(result);
        }

        [HttpDelete("{blockedCompanyId}")]
        public async Task<IActionResult> Delete(int blockedCompanyId)
        {
            var success = await _service.DeleteAsync(blockedCompanyId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
} 