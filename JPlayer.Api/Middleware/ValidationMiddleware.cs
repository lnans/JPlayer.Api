using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JPlayer.Business;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace JPlayer.Api.Middleware
{
    public class ValidationMiddleware : IActionResult
    {
        private readonly ILogger<ValidationMiddleware> _logger =
            new Logger<ValidationMiddleware>(new NLogLoggerFactory());

        public async Task ExecuteResultAsync(ActionContext context)
        {
            KeyValuePair<string, ModelStateEntry>[] modelStateEntries =
                context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();

            IEnumerable<ModelError> errors = modelStateEntries.SelectMany(entries => entries.Value.Errors);
            ApiError error = new()
            {
                Error = GlobalLabelCodes.RequestValidationError,
                Details = errors.Select(err => err.ErrorMessage)
            };

            this._logger.LogInformation(GlobalLabelCodes.RequestValidationError);
            foreach (string errorDetail in error.Details)
                this._logger.LogInformation(errorDetail);

            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.HttpContext.Response.Body, error);
            await context.HttpContext.Response.Body.FlushAsync();
        }
    }
}