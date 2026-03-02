using Microsoft.AspNetCore.Http;
using MyReadsApp.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace MyReadsApp.API.Middleware.Exceptions
{
    public class ExceptionHandeler : IMiddleware
    {
        private readonly ILogger<ExceptionHandeler> _logger;

        public ExceptionHandeler(ILogger<ExceptionHandeler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleException(context, ex, HttpStatusCode.NotFound);
            }
            catch (ConfilectException ex)
            {
                _logger.LogWarning(ex, "Conflict exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleException(context, ex, HttpStatusCode.Conflict);
            }
            catch (NotAuthorizeException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleException(context, ex, HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleException(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex, HttpStatusCode statusCodes)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCodes;

            var response = new
            {
                Succeeded = false,
                StatusCode = (int)statusCodes,
                Message = ex.Message
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
