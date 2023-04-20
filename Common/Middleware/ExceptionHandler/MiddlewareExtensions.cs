using Microsoft.AspNetCore.Builder;

namespace Common.Middleware.ExceptionHandler;

public static class MiddlewareExtensions
{
    public static void UseExceptionHandlerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsService>();
    }
}