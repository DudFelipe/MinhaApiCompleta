using Elmah.Io.AspNetCore;
using System.Net;

namespace DevIO.Api.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
               try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
            }
        }

        private static void HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ex.Ship(context);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
