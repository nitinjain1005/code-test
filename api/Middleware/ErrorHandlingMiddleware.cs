using api.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid Arguments error");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Incorrect Input",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (ExtServiceException ex)
            {
                _logger.LogWarning(ex, "External service error");
                context.Response.StatusCode = ex.StatusCode > 0 ?(int) ex.StatusCode :(int) StatusCodes.Status503ServiceUnavailable; ;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "External service unavailable",
                    Detail = ex.Message,
                    Status = context.Response.StatusCode
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "External service error");
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "No Data returned from external service.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
