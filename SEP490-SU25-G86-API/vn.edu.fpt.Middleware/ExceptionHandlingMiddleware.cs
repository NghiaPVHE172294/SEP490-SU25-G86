using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    try
        //    {
        //        await _next(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unhandled exception occurred.");
        //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        context.Response.ContentType = "application/json";
        //        await context.Response.WriteAsync($"{{\"error\":\"{ex.Message}\"}}");
        //    }
        //}
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("rate limit"))
            {
                // Bắt riêng rate-limit từ OpenAI
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { error = ex.Message });
                await context.Response.WriteAsync(payload);
            }
            catch (Exception)
            {
                // Bắt mọi lỗi khác
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { error = "Internal server error" });
                await context.Response.WriteAsync(payload);
            }
        }
    }
} 