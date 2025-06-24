using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace SEP490_SU25_G86_Client.Pages.Admin
{
    public class ManagePermissionsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ManagePermissionsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<RoleDto> Roles { get; set; } = new();
        public List<PermissionDto> Permissions { get; set; } = new();
        public List<int> RolePermissionIds { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int SelectedRoleId { get; set; }
        [BindProperty]
        public List<int> SelectedPermissionIds { get; set; } = new();
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            await LoadData(client);

            var role = HttpContext.Session.GetString("user_role");
            if (role != "ADMIN")
            {
                return RedirectToPage("/NotFound");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();
            await LoadData(client);

            // Lấy các quyền hiện tại của role
            var currentPermIds = new HashSet<int>(RolePermissionIds);
            var newPermIds = new HashSet<int>(SelectedPermissionIds ?? new List<int>());

            // Thêm quyền mới
            foreach (var id in newPermIds.Except(currentPermIds))
            {
                var payload = new { RoleId = SelectedRoleId, PermissionId = id, IsAuthorized = true };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("https://localhost:7004/api/RolePermission", content);
                if (!resp.IsSuccessStatusCode) ErrorMessage += $"Thêm quyền {id} thất bại. ";
            }
            // Xóa quyền bị bỏ chọn
            foreach (var id in currentPermIds.Except(newPermIds))
            {
                var resp = await client.DeleteAsync($"https://localhost:7004/api/RolePermission/{SelectedRoleId}/{id}");
                if (!resp.IsSuccessStatusCode) ErrorMessage += $"Xóa quyền {id} thất bại. ";
            }
            if (string.IsNullOrEmpty(ErrorMessage))
                Message = "Cập nhật phân quyền thành công!";
            await LoadData(client); // reload lại danh sách
            return Page();
        }

        private async Task LoadData(HttpClient client)
        {
            // Lấy token từ session và truyền vào header
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Lấy roles
            var rolesResp = await client.GetAsync("https://localhost:7004/api/RolePermission/roles");
            if (rolesResp.IsSuccessStatusCode)
            {
                var json = await rolesResp.Content.ReadAsStringAsync();
                Roles = JsonSerializer.Deserialize<List<RoleDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // Lấy permissions
            var permsResp = await client.GetAsync("https://localhost:7004/api/RolePermission/permissions");
            if (permsResp.IsSuccessStatusCode)
            {
                var json = await permsResp.Content.ReadAsStringAsync();
                Permissions = JsonSerializer.Deserialize<List<PermissionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // Lấy quyền của role đang chọn
            if (SelectedRoleId == 0 && Roles.Any()) SelectedRoleId = Roles.First().RoleId;
            var rolePermResp = await client.GetAsync($"https://localhost:7004/api/RolePermission/role-permissions/{SelectedRoleId}");
            if (rolePermResp.IsSuccessStatusCode)
            {
                var json = await rolePermResp.Content.ReadAsStringAsync();
                RolePermissionIds = JsonSerializer.Deserialize<List<int>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        public class RoleDto { public int RoleId { get; set; } public string RoleName { get; set; } }
        public class PermissionDto { public int Id { get; set; } public string Name { get; set; } public string Method { get; set; } public string Endpoint { get; set; } }
    }
} 