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
            var endpointMeta = context.GetEndpoint();
            if (endpointMeta?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }
            foreach (var claim in context.User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type} | Value: {claim.Value}");
            }

            // Lấy AccountId từ JWT token
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int accountId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing user ID.");
                return;
            }

            var endpoint = context.Request.Path.Value ?? "";
            var method = context.Request.Method;

            // Kiểm tra quyền
            var hasPermission = await permissionService.CheckAccessAsync(accountId, endpoint, method);
            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access Denied: You do not have permission.");
                return;
            }

            await _next(context);
        }

    }
}
