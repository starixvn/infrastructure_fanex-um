using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace ONEbook.UM.Api.Extensions;

public static class ExceptionMiddlewareExtensions
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
                if (contextFeature != null)
                {
                    Console.WriteLine(contextFeature.Error.Message);
                    var error = new
                    {
                        context.Response.StatusCode,
                        Message = "Internal Server Error."
                    };
                    var message = JsonConvert.SerializeObject(error);
                    await context.Response.WriteAsync(message);
                }
            });
        });
    }
}