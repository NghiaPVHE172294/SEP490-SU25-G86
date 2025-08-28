using SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;

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
            Console.WriteLine("========= [DEBUG] JWT CLAIMS =========");
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("❌ User.Identity is NOT authenticated");
            }
            else
            {
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"✔️ Claim Type: {claim.Type} | Value: {claim.Value}");
                }
            }

            // BỎ QUA SignalR hubs: để Hub tự authorize bằng JWT/Role
            var path = context.Request.Path;
            if (path.StartsWithSegments("/hubs"))
            {
                await _next(context);
                return;
            }

            // AllowAnonymous thì bỏ qua
            var endpointMeta = context.GetEndpoint();
            if (endpointMeta?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }

            // Lấy AccountId từ JWT token
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int accountId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing user ID.");
                return;
            }

            // ✅ Lấy route template thay vì path thật
            var routePattern = (endpointMeta as RouteEndpoint)?.RoutePattern?.RawText?.ToLower() ?? "";
            var method = context.Request.Method;

            Console.WriteLine($"🔍 Checking access for AccountId: {accountId}, Endpoint: {routePattern}, Method: {method}");

            // Kiểm tra quyền
            var hasPermission = await permissionService.CheckAccessAsync(accountId, routePattern, method);
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
