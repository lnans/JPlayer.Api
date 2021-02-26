using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JPlayer.Api.Middleware;
using JPlayer.Business;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JPlayer.Test
{
    [TestFixture]
    public class MiddlewareTest : AbstractBaseTest
    {
        [Test]
        public async Task ExceptionMiddleware_ShouldReturn_ApiError_WhenExceptionThrown()
        {
            // Arrange
            ExceptionMiddleware exceptionMiddleware = new();

            IExceptionHandlerFeature exceptionHandlerFeature = new ExceptionHandlerFeature
                {Error = new Exception("Test"), Path = "/"};
            DefaultHttpContext context = new();
            context.Response.Body = new MemoryStream();
            context.Features.Set(exceptionHandlerFeature);

            // Act
            await exceptionMiddleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new(context.Response.Body);
            string streamText = await reader.ReadToEndAsync();
            ApiError objResponse = JsonConvert.DeserializeObject<ApiError>(streamText);

            // Assert
            Assert.IsNotNull(objResponse);
            Assert.AreEqual("Test", objResponse.Error);
        }

        [Test]
        public async Task ExceptionMiddleware_ShouldReturn_Null_WhenNothingAppend()
        {
            // Arrange
            ExceptionMiddleware exceptionMiddleware = new();
            JsonSerializerOptions jsonOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            DefaultHttpContext context = new();
            context.Response.Body = new MemoryStream();
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, "OK", jsonOptions);
            await context.Response.Body.FlushAsync().ConfigureAwait(false);

            // Act
            await exceptionMiddleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new(context.Response.Body);
            string streamText = await reader.ReadToEndAsync();
            string objResponse = JsonConvert.DeserializeObject<string>(streamText);

            // Assert
            Assert.IsNotNull(objResponse);
            Assert.AreEqual("OK", objResponse);
        }

        [Test]
        public async Task LogMiddleware_ShouldSet_HttpContextInformation()
        {
            // Arrange
            DefaultHttpContext context = new() {User = CreateUser("UserAdmin", "1"), TraceIdentifier = "identifier"};
            ILogger<LogMiddleware> logger = new NLogLoggerFactory().CreateLogger<LogMiddleware>();

            static Task RequestDelegate(HttpContext _)
            {
                return Task.CompletedTask;
            }

            LogMiddleware logMiddleware = new(logger, RequestDelegate);

            // Act
            await logMiddleware.Invoke(context);

            // Assert
            Assert.AreEqual("identifier", context.Items["CorrelationId"]);
            Assert.AreEqual("UserAdmin", context.Items["NameIdentifier"]);
        }

        [Test]
        public void LogMiddleware_ShouldThrow_WhenDelegateException()
        {
            // Arrange
            DefaultHttpContext context = new() {User = CreateUser("UserAdmin", "1"), TraceIdentifier = "identifier"};
            ILogger<LogMiddleware> logger = new NLogLoggerFactory().CreateLogger<LogMiddleware>();

            static Task RequestDelegate(HttpContext _)
            {
                throw new Exception("Test exception");
            }

            LogMiddleware logMiddleware = new(logger, RequestDelegate);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await logMiddleware.Invoke(context));
        }

        [Test]
        public async Task ValidationMiddleware_ShouldReturn_ApiError_WithInformation()
        {
            // Arrange
            ModelStateDictionary modelStateDictionary = new();
            modelStateDictionary.AddModelError("property", "fake_error");
            ActionContext actionContext = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor(),
                modelStateDictionary);
            actionContext.HttpContext.Response.Body = new MemoryStream();
            ValidationMiddleware validationMiddleware = new();

            // Act
            await validationMiddleware.ExecuteResultAsync(actionContext);
            actionContext.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new(actionContext.HttpContext.Response.Body);
            string streamText = await reader.ReadToEndAsync();
            ApiError objResponse = JsonConvert.DeserializeObject<ApiError>(streamText);

            // Assert
            Assert.IsNotNull(objResponse);
            Assert.AreEqual(GlobalLabelCodes.RequestValidationError, objResponse.Error);
            Assert.IsTrue(objResponse.Details.Any());
            Assert.AreEqual(objResponse.Details.First(), "fake_error");
        }
    }
}