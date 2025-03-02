using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;

namespace BackendTemplate.API.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Proceed with the request pipeline
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured {ex.Message}");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var result = JsonConvert.SerializeObject(new { error = ex.Message});

                await httpContext.Response.WriteAsync(result);

            }
        }
    }
}
