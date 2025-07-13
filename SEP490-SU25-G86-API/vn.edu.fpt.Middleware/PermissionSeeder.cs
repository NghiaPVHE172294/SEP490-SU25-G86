using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
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
            var validPermissions = new List<(string Method, string Endpoint)>();

            foreach (var endpointSource in _endpointSources)
            {
                foreach (var endpoint in endpointSource.Endpoints)
                {
                    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                    var httpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;

                    if (actionDescriptor == null || httpMethods == null)
                        continue;

                    var pattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;
                    if (string.IsNullOrEmpty(pattern))
                        continue;

                    foreach (var method in httpMethods)
                    {
                        var normalizedEndpoint = "/" + pattern.ToLower();
                        var upperMethod = method.ToUpper();

                        validPermissions.Add((upperMethod, normalizedEndpoint));

                        bool exists = existingPermissions.Any(p =>
                            p.Method.ToUpper() == upperMethod &&
                            p.Endpoint.ToLower() == normalizedEndpoint.ToLower()
                        );

                        if (!exists)
                        {
                            var controllerName = actionDescriptor.ControllerName;
                            var actionName = actionDescriptor.ActionName;
                            var name = $"{controllerName} - {actionName}";

                            newPermissions.Add(new Permission
                            {
                                Name = name,
                                Method = upperMethod,
                                Endpoint = normalizedEndpoint
                            });
                        }
                    }
                }
            }

            // Xóa các permission không còn tồn tại trong route thật
            var toRemove = existingPermissions
                .Where(p => !validPermissions.Any(v =>
                    v.Method == p.Method.ToUpper() &&
                    v.Endpoint.ToLower() == p.Endpoint.ToLower()
                ))
                .Where(p => !_context.RolePermissions.Any(rp => rp.PermissionId == p.Id)) // đảm bảo không bị FK
                .ToList();


            if (toRemove.Any())
            {
                _context.Permissions.RemoveRange(toRemove);
            }

            if (newPermissions.Any())
            {
                _context.Permissions.AddRange(newPermissions);
            }

            if (newPermissions.Any() || toRemove.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
