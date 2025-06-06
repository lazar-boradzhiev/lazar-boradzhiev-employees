using Employees.Api.Models;
using System.Net;

namespace Employees.Api.ErrorsHandling
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
        {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Global exception handler error");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    new ErrorModel("Something went wrong"));
            }
        }
    }
}
