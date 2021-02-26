using JPlayer.Data.Dto.Version;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class VersionController : ControllerBase
    {
        /// <summary>
        ///     Get the version of the current application
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(ApplicationVersion), 200)]
        public IActionResult GetVersion() => this.Ok(new ApplicationVersion(typeof(Program)).AsApiResult());
    }
}