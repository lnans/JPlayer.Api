using System.Net;
using JPlayer.Api.Controllers;
using JPlayer.Data.Dto.Version;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class VersionTest
    {
        [Test]
        public void GetVersion_ShouldReturn_ApplicationVersion_WithStatus200()
        {
            // Arrange
            VersionController versionController = new();

            // Act
            IActionResult actionResult = versionController.GetVersion();
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<ApplicationVersion> applicationVersion = result.Value as ApiResult<ApplicationVersion>;

            Assert.IsNotNull(applicationVersion);
            Assert.IsNotNull(applicationVersion.Data.Name);
        }
    }
}