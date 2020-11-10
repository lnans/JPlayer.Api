using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JPlayer.Business;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JPlayer.Api.Middleware
{
    public class ValidationMiddleware : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            KeyValuePair<string, ModelStateEntry>[] modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();

            IEnumerable<ModelError> errors = modelStateEntries.SelectMany(entries => entries.Value.Errors);
            ApiError error = new ApiError
            {
                Error = GlobalLabelCodes.RequestValidationError,
                Details = errors.Select(err => err.ErrorMessage)
            };

            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.HttpContext.Response.Body, error);
            await context.HttpContext.Response.Body.FlushAsync();
        }
    }
}