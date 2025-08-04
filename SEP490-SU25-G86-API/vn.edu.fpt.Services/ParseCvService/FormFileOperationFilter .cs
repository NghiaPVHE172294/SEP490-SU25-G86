using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ParseCvService
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Tìm các parameter kiểu IFormFile
            var fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            if (!fileParams.Any())
                return;

            // Tạo schema multipart/form-data chỉ có 1 field "file"
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type       = "object",
                        Properties = {
                            ["file"] = new OpenApiSchema
                            {
                                Type   = "string",
                                Format = "binary"
                            }
                        },
                        Required = new HashSet<string> { "file" }
                    }
                }
            }
            };
        }
    }
}
