using SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService;
using System.Security.Claims;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            // Lấy AccountId từ JWT token (ClaimTypes.NameIdentifier)
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int accountId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing user ID.");
                return;
            }

            var endpoint = context.Request.Path.Value ?? "";
            var method = context.Request.Method;

            // Gọi service để kiểm tra quyền
            var hasPermission = await permissionService.CheckAccessAsync(accountId, endpoint, method);

            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access Denied: You do not have permission.");
                return;
            }

            // Cho request đi tiếp nếu hợp lệ
            await _next(context);
        }
    }
}
