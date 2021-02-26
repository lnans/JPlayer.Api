using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Credentials;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace JPlayer.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
        {
            this._logger = logger;
            this._authService = authService;
        }

        /// <summary>
        ///     Sign In a user
        /// </summary>
        /// <param name="credentialsForm"></param>
        /// <returns></returns>
        /// <response code="200">Authentication success</response>
        /// <response code="401">Authentication failed</response>
        [HttpPost("SignIn")]
        [ProducesResponseType(typeof(ApiResult<CredentialsInfo>), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        [SwaggerOperation(Tags = new[] {SwaggerTags.AuthenticationSection})]
        public async Task<IActionResult> SignIn([FromBody] CredentialsForm credentialsForm)
        {
            try
            {
                this._logger.LogInformation("Login attempt as {Login}", credentialsForm.Login);
                CredentialsInfo result = await this._authService.SignInAsync(this.HttpContext, credentialsForm);
                return this.Ok(result.AsApiResult("credentialsInfo"));
            }
            catch (AuthenticationException e)
            {
                this._logger.LogInformation("Login failed");
                return this.Unauthorized(e.AsApiError());
            }
        }

        /// <summary>
        ///     Get current logged user information
        /// </summary>
        /// <returns></returns>
        /// <response code="200">User information</response>
        /// <response code="401">Authentication failed</response>
        [Authorize]
        [HttpGet("")]
        [ProducesResponseType(typeof(ApiResult<CredentialsInfo>), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        [SwaggerOperation(Tags = new[] {SwaggerTags.AuthenticationSection})]
        public IActionResult GetIdentity()
        {
            this._logger.LogInformation("Getting current logged user");
            string login = this.HttpContext.User.Claims
                .Where(cl => cl.Type == ClaimTypes.Name)
                .Select(cl => cl.Value).FirstOrDefault();

            IEnumerable<string> functions = this.HttpContext.User.Claims
                .Where(cl => cl.Type == ClaimTypes.Role)
                .Select(cl => cl.Value);

            CredentialsInfo result = new()
            {
                Login = login,
                Functions = functions.ToList()
            };

            return this.Ok(result.AsApiResult("credentialsInfo"));
        }

        /// <summary>
        ///     Sign Out a user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Authentication success</response>
        /// <response code="401">Authentication failed</response>
        [Authorize]
        [HttpDelete("SignOut")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        [SwaggerOperation(Tags = new[] {SwaggerTags.AuthenticationSection})]
        public new async Task<IActionResult> SignOut()
        {
            this._logger.LogInformation("Logout");
            await this.HttpContext.SignOutAsync();
            return this.Ok(true.AsApiResult());
        }

        /// <summary>
        ///     Update current user credentials
        /// </summary>
        /// <param name="credentialsUpdateForm"></param>
        /// <returns></returns>
        /// <response code="200">Credentials updated</response>
        /// <response code="401">Wrong password</response>
        /// <response code="404">Current connected user not found</response>
        /// <response code="500">Internal authentication error</response>
        [Authorize]
        [HttpPut("UpdateCredentials")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        [ProducesResponseType(typeof(ApiError), 404)]
        [ProducesResponseType(typeof(ApiError), 500)]
        [SwaggerOperation(Tags = new[] {SwaggerTags.AuthenticationSection})]
        public async Task<IActionResult> UpdateCredentials([FromBody] CredentialsUpdateForm credentialsUpdateForm)
        {
            this._logger.LogInformation("Checking current logged user");
            string userId = this.HttpContext.User.Claims
                .Where(cl => cl.Type == ClaimTypes.NameIdentifier)
                .Select(cl => cl.Value).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(userId))
                return this.StatusCode(500);

            try
            {
                this._logger.LogInformation("Update credentials attempt");
                await this._authService.UpdateCredentials(int.Parse(userId), credentialsUpdateForm);
                return this.Ok(true.AsApiResult());
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
            catch (AuthenticationException e)
            {
                return this.Unauthorized(e.AsApiError());
            }
        }
    }
}