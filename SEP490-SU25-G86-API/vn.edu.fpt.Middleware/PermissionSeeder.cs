using Microsoft.AspNetCore.Mvc.Controllers;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Middleware
{
    public class PermissionSeeder
    {
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly IEnumerable<EndpointDataSource> _endpointSources;

        public PermissionSeeder(SEP490_G86_CvMatchContext context, IEnumerable<EndpointDataSource> endpointSources)
        {
            _context = context;
            _endpointSources = endpointSources;
        }

        public async Task SeedPermissionsAsync()
        {
            var existingPermissions = _context.Permissions.ToList();
            var newPermissions = new List<Permission>();

            foreach (var endpointSource in _endpointSources)
            {
                foreach (var endpoint in endpointSource.Endpoints)
                {
                    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                    var httpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;

                    if (actionDescriptor == null || httpMethods == null) continue;

                    var pattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;
                    if (string.IsNullOrEmpty(pattern)) continue;

                    foreach (var method in httpMethods)
                    {
                        var normalizedEndpoint = "/" + pattern.ToLower();
                        bool exists = existingPermissions.Any(p =>
                            p.Method.ToUpper() == method.ToUpper() &&
                            p.Endpoint.ToLower() == normalizedEndpoint.ToLower()
                        );

                        if (!exists)
                        {
                            // Tạo tên hiển thị: Controller - Action (viết gọn)
                            var controllerName = actionDescriptor.ControllerName;
                            var actionName = actionDescriptor.ActionName;

                            string name = $"{controllerName} - {actionName}";

                            newPermissions.Add(new Permission
                            {
                                Name = name,
                                Method = method.ToUpper(),
                                Endpoint = normalizedEndpoint
                            });
                        }
                    }
                }
            }

            if (newPermissions.Any())
            {
                _context.Permissions.AddRange(newPermissions);
                await _context.SaveChangesAsync();
            }
        }

    }
}
