using Microsoft.AspNetCore.Builder;

namespace API.Middleware {
    public static class ExceptionMiddlewareExtensions {
        public static void ConfigureCustomExceptionMiddleware (this IApplicationBuilder app) 
        {
            app.UseMiddleware<ErrorHandlingMiddleware> ();
        }
    }
}