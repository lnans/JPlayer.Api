using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace JPlayer.Api.Middleware
{
    public class ExceptionMiddleware
    {
        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            Exception ex = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (ex == null)
                return;

            ApiError error = new ApiError
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message
            };

            httpContext.Response.ContentType = "application/json";
            await using Utf8JsonWriter writer = new Utf8JsonWriter(httpContext.Response.Body);
            JsonSerializer.Serialize(writer, error);
            await writer.FlushAsync().ConfigureAwait(false);
        }
    }
}