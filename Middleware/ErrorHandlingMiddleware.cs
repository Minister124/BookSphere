using System;
using System.Net;
using System.Text.Json;

namespace BookSphere.Middleware;

public class ErrorHandlingMiddleware
{
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
                _next = next;
                _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
                try
                {
                        await _next(context);
                }
                catch (Exception ex)    
                {
                        _logger.LogError(ex, ex.Message);
                        await HandleExceptionAsync(context, ex);
                }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
                var statusCode = HttpStatusCode.InternalServerError;
                var result = string.Empty;

                switch(ex)
                {
                        case UnauthorizedAccessException:
                                statusCode = HttpStatusCode.Unauthorized;
                                break;
                        case KeyNotFoundException:
                                statusCode = HttpStatusCode.NotFound;
                                break;
                        case ArgumentException:
                                statusCode = HttpStatusCode.BadRequest;
                                break;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new 
                {
                        error = ex.Message,
                        statusCode = statusCode
                };

                result = JsonSerializer.Serialize(response);

                return context.Response.WriteAsync(result);
        }
}
