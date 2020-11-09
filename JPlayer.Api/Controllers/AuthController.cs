using System.Security.Authentication;
using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Credentials;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
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
        public async Task<IActionResult> SignIn([FromBody] CredentialsForm credentialsForm)
        {
            try
            {
                CredentialsInfo result = await this._authService.SignInAsync(this.HttpContext, credentialsForm);
                return this.Ok(result.AsApiResult("credentialsInfo"));
            }
            catch (AuthenticationException e)
            {
                return this.Unauthorized(e.AsApiError());
            }
        }

        /// <summary>
        ///     Sign Out a user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("SignOut")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public async Task<IActionResult> SignOut()
        {
            await this.HttpContext.SignOutAsync();
            return this.Ok(true.AsApiResult());
        }
    }
}