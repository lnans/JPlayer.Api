using System.Net;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business;
using JPlayer.Business.Services;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Credentials;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Crypto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class AuthTest : AbstractBaseTest
    {
        [SetUp]
        public void Setup()
        {
            this.InitDbContext();
            this._loggerAuthService = new NLogLoggerFactory().CreateLogger<AuthService>();
            this._loggerAuthController = new NLogLoggerFactory().CreateLogger<AuthController>();
            this._authService = new AuthService(this._loggerAuthService, this.DbContext);
            this._authController =
                this.CreateTestController(new AuthController(this._loggerAuthController, this._authService));
        }

        [TearDown]
        public void TearDown()
        {
            this.CleanDbContext();
        }

        private ILogger<AuthService> _loggerAuthService;
        private ILogger<AuthController> _loggerAuthController;
        private AuthService _authService;
        private AuthController _authController;

        [Test]
        public async Task SignIn_WithGoodCred_ShouldReturn_CredentialsInfo_WithStatus200()
        {
            // Arrange credentials form
            CredentialsForm credentialsForm = new() {Login = "UserAdmin", Password = "UserAdmin"};

            // Act
            IActionResult actionResult = await this._authController.SignIn(credentialsForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<CredentialsInfo> userInfo = result.Value as ApiResult<CredentialsInfo>;
            Assert.IsNotNull(userInfo);
            Assert.AreEqual("UserAdmin", userInfo.Data.Login);
        }

        [Test]
        public async Task SignIn_WithWrongPassword_ShouldReturn_ApiError_WithStatus401()
        {
            // Arrange credentials form
            CredentialsForm credentialsForm = new() {Login = "UserAdmin", Password = "WrongPassword"};

            // Act
            IActionResult actionResult = await this._authController.SignIn(credentialsForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, result.StatusCode);

            ApiError apiError = result.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(GlobalLabelCodes.AuthAuthenticationFailed, apiError.Error);
        }

        [Test]
        public async Task SignIn_WithWrongLogin_ShouldReturn_ApiError_WithStatus401()
        {
            // Arrange credentials form
            CredentialsForm credentialsForm = new() {Login = "WrongLogin", Password = "UserAdmin"};

            // Act
            IActionResult actionResult = await this._authController.SignIn(credentialsForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, result.StatusCode);

            ApiError apiError = result.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(GlobalLabelCodes.AuthAuthenticationFailed, apiError.Error);
        }

        [Test]
        public void GetIdentity_ShouldReturn_CredentialsInfo_WithStatus200()
        {
            // Arrange authentication
            this._authController.HttpContext.User = CreateUser("UserAdmin", "1");

            // Act
            IActionResult actionResult = this._authController.GetIdentity();
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<CredentialsInfo> userInfo = result.Value as ApiResult<CredentialsInfo>;
            Assert.IsNotNull(userInfo);
            Assert.AreEqual("UserAdmin", userInfo.Data.Login);
        }

        [Test]
        public async Task SignOut_ShouldReturn_Status200()
        {
            // Act
            IActionResult actionResult = await this._authController.SignOut();
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task UpdateCredentials_WithGoodCred_ShouldReturn_Status200()
        {
            // Arrange
            this._authController.HttpContext.User = CreateUser("UserAdmin", "1");
            CredentialsUpdateForm credentialsUpdateForm =
                new() {CurrentPassword = "UserAdmin", NewPassword = "NewPassword"};

            // Act
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            UsrUserDao user = await this.DbContext.Users.FindAsync(1);
            Assert.IsTrue(PasswordHelper.Check(user.Login, "NewPassword", user.Password));
        }

        [Test]
        public async Task UpdateCredentials_WithBadCred_ShouldReturn_Status401()
        {
            // Arrange
            this._authController.HttpContext.User = CreateUser("UserAdmin", "1");
            CredentialsUpdateForm credentialsUpdateForm =
                new() {CurrentPassword = "WrongPassword", NewPassword = "NewPassword"};

            // Act
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public async Task UpdateCredentials_WithWrongIdentifier_ShouldReturn_Status401()
        {
            // Arrange
            this._authController.HttpContext.User = CreateUser("Fake", "99");
            CredentialsUpdateForm credentialsUpdateForm =
                new() {CurrentPassword = "Password", NewPassword = "Password"};

            // Act
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public async Task UpdateCredentials_WithNullIdentifier_ShouldReturn_Status500()
        {
            // Arrange
            this._authController.HttpContext.User = CreateUser("Fake", null);
            CredentialsUpdateForm credentialsUpdateForm =
                new() {CurrentPassword = "Password", NewPassword = "Password"};

            // Act
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            StatusCodeResult result = actionResult as StatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}