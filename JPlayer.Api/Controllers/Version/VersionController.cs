using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers.Version
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
        [ProducesResponseType(typeof(ApplicationVersion), 200)]
        public IActionResult GetVersion() => this.Ok(new ApplicationVersion());
    }
}