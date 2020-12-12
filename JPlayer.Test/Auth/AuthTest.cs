using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business;
using JPlayer.Business.Services;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Credentials;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Crypto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Auth
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
            this._authService = new AuthService(this._loggerAuthService, this._dbContext);
            this._authController = this.CreateTestController(new AuthController(this._loggerAuthController, this._authService));
        }

        [TearDown]
        public void CleanUp()
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
            CredentialsForm credentialsForm = new CredentialsForm {Login = "UserAdmin", Password = "UserAdmin"};
            IActionResult actionResult = await this._authController.SignIn(credentialsForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<CredentialsInfo> userInfo = result.Value as ApiResult<CredentialsInfo>;
            Assert.IsNotNull(userInfo);
            Assert.AreEqual("UserAdmin", userInfo.Data.Login);
        }

        [Test]
        public async Task SignIn_WithBadCred_ShouldReturn_ApiError_WithStatus401()
        {
            CredentialsForm credentialsForm = new CredentialsForm {Login = "UserAdmin", Password = "WrongPassword"};
            IActionResult actionResult = await this._authController.SignIn(credentialsForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, result.StatusCode);

            ApiError apiError = result.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(GlobalLabelCodes.AuthAuthenticationFailed, apiError.Error);
        }

        [Test]
        public void GetIdentity_ShouldReturn_CredentialsInfo_WithStatus200()
        {
            // Create fake authentication
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, "UserAdmin"));
            this._authController.HttpContext.User = new ClaimsPrincipal(identity);

            IActionResult actionResult = this._authController.GetIdentity();
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<CredentialsInfo> userInfo = result.Value as ApiResult<CredentialsInfo>;
            Assert.IsNotNull(userInfo);
            Assert.AreEqual("UserAdmin", userInfo.Data.Login);
        }

        [Test]
        public async Task SignOut_ShouldReturn_Status200()
        {
            IActionResult actionResult = await this._authController.SignOut();
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task UpdateCredentials_WithGoodCred_ShouldReturn_Status200()
        {
            // Create fake authentication
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, "UserAdmin"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            this._authController.HttpContext.User = new ClaimsPrincipal(identity);

            CredentialsUpdateForm credentialsUpdateForm = new CredentialsUpdateForm {CurrentPassword = "UserAdmin", NewPassword = "NewPassword"};
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            UsrUserDao user = await this._dbContext.Users.FindAsync(1);
            Assert.IsTrue(PasswordHelper.Check(user.Login, "NewPassword", user.Password));
        }

        [Test]
        public async Task UpdateCredentials_WithBadCred_ShouldReturn_Status401()
        {
            // Create fake authentication
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, "UserAdmin"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            this._authController.HttpContext.User = new ClaimsPrincipal(identity);

            CredentialsUpdateForm credentialsUpdateForm = new CredentialsUpdateForm {CurrentPassword = "WrongPassword", NewPassword = "NewPassword"};
            IActionResult actionResult = await this._authController.UpdateCredentials(credentialsUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}