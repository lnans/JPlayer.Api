using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JPlayer.Api.Middleware;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JPlayer.Test
{
    [TestFixture]
    public class MiddlewareTest
    {
        [Test]
        public async Task ExceptionMiddleware_ShouldReturn_ApiError_WhenExceptionThrown()
        {
            // Prepare
            ExceptionMiddleware exceptionMiddleware = new ExceptionMiddleware();

            IExceptionHandlerFeature exceptionHandlerFeature = new ExceptionHandlerFeature {Error = new Exception("Test"), Path = "/"};
            DefaultHttpContext context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Features.Set(exceptionHandlerFeature);

            // Act
            await exceptionMiddleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(context.Response.Body);
            string streamText = await reader.ReadToEndAsync();
            ApiError objResponse = JsonConvert.DeserializeObject<ApiError>(streamText);

            // Assert
            Assert.IsNotNull(objResponse);
            Assert.AreEqual("Test", objResponse.Error);
        }

        [Test]
        public async Task ExceptionMiddleware_ShouldReturn_Null_WhenNothingAppend()
        {
            // Prepare
            ExceptionMiddleware exceptionMiddleware = new ExceptionMiddleware();
            JsonSerializerOptions JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            DefaultHttpContext context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, "OK", JsonOptions);
            await context.Response.Body.FlushAsync().ConfigureAwait(false);

            // Act
            await exceptionMiddleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(context.Response.Body);
            string streamText = await reader.ReadToEndAsync();
            string objResponse = JsonConvert.DeserializeObject<string>(streamText);

            // Assert
            Assert.IsNotNull(objResponse);
            Assert.AreEqual("OK", objResponse);
        }
    }
}