using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception (in production, use proper logging)
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Set response for API calls
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"error\": \"An internal server error occurred.\"}}");
                }
                else
                {
                    // Redirect to error page for MVC requests
                    context.Response.Redirect("/Home/Error");
                }
            }
        }
    }
}