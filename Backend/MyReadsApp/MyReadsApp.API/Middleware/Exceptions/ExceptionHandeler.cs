using Microsoft.AspNetCore.Http;
using MyReadsApp.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace MyReadsApp.API.Middleware.Exceptions
{
    public class ExceptionHandeler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NoFoundException ex)
            {
                await HandleException(context, ex, HttpStatusCode.NotFound);
            }
            catch (ConfilectException ex)
            {
                await HandleException(context, ex, HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
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
