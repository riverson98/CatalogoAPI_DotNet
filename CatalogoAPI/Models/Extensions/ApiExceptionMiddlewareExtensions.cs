using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CatalogoAPI.Models.Extensions
{
    public static class ApiExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature is not null)
                    {
                        await context.Response.WriteAsync(new DetalhesDeErro()
                        {
                            StatusCode = context.Response.StatusCode,
                            Mensagem = contextFeature.Error.Message,
                            Rastro = contextFeature.Error.StackTrace
                        }.ToString());
                    }
                });
            });
        }
    }
}
