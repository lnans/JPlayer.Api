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

            ApiError error = new()
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace,
                CorrelationId = httpContext.TraceIdentifier
            };

            httpContext.Response.ContentType = "application/json";

            JsonSerializerOptions jsonOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await JsonSerializer.SerializeAsync(httpContext.Response.Body, error, jsonOptions);
            await httpContext.Response.Body.FlushAsync().ConfigureAwait(false);
        }
    }
}