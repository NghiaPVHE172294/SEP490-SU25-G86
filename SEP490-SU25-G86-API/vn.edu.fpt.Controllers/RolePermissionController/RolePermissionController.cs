using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.RolePermissionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.RolePermissionService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.RolePermissionController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionService _service;
        private readonly SEP490_G86_CvMatchContext _context;

        public RolePermissionController(IRolePermissionService service, SEP490_G86_CvMatchContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{roleId:int}/{permissionId:int}")]
        public async Task<IActionResult> GetById(int roleId, int permissionId)
        {
            var item = await _service.GetByIdAsync(roleId, permissionId);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolePermissionDTO dto)
        {
            var entity = new RolePermission
            {
                RoleId = dto.RoleId,
                PermissionId = dto.PermissionId,
                IsAuthorized = dto.IsAuthorized
            };

            await _service.AddAsync(entity);
            return Ok("Created successfully");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RolePermissionDTO dto)
        {
            var existing = await _service.GetByIdAsync(dto.RoleId, dto.PermissionId);
            if (existing == null)
                return NotFound();

            existing.IsAuthorized = dto.IsAuthorized;
            await _service.UpdateAsync(existing);

            return Ok("Updated successfully");
        }

        [HttpDelete("{roleId:int}/{permissionId:int}")]
        public async Task<IActionResult> Delete(int roleId, int permissionId)
        {
            var item = await _service.GetByIdAsync(roleId, permissionId);
            if (item == null)
                return NotFound();

            await _service.DeleteAsync(roleId, permissionId);
            return Ok("Deleted successfully");
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();
            return Ok(roles);
        }

        [HttpGet("permissions")]
        public IActionResult GetPermissions()
        {
            var permissions = _context.Permissions
                .Select(p => new {
                    Id = p.Id,
                    Name = p.Name,
                    Method = p.Method,
                    Endpoint = p.Endpoint
                })
                .ToList();
            return Ok(permissions);
        }

        [HttpGet("role-permissions/{roleId}")]
        public IActionResult GetPermissionIdsOfRole(int roleId)
        {
            var permissionIds = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsAuthorized == true)
                .Select(rp => rp.PermissionId)
                .ToList();
            return Ok(permissionIds);
        }
    }
}
