using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JPlayer.Api.Middleware
{
    public class LogMiddleware
    {
        private readonly ILogger<LogMiddleware> _logger;
        private readonly RequestDelegate _next;

        public LogMiddleware(ILogger<LogMiddleware> logger, RequestDelegate next)
        {
            this._logger = logger;
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["CorrelationId"] = context.TraceIdentifier;
            Stopwatch sw = Stopwatch.StartNew();

            string protocol = context.Request.IsHttps ? "https" : "http";
            string url = $"{protocol}://{context.Request.Host.Value}{context.Request.Path}";

            this._logger.LogInformation($"{context.Request.Method} {context.Request.Protocol} {url}");

            try
            {
                await this._next(context);
                sw.Stop();
                this._logger.LogInformation($"END with status code: {context.Response.StatusCode} in {sw.Elapsed.TotalMilliseconds} ms");
            }
            catch (Exception e)
            {
                sw.Stop();
                this._logger.LogError(message: $"Request has throw an exception, reason - elapsed time {sw.Elapsed.TotalMilliseconds} ms", exception: e);
                throw;
            }
        }
    }
}