using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers.Version
{
    [Route("[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        /// <summary>
        ///     Get the version of the current application
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult GetVersion()
        {
            System.Version assemblyVer = typeof(Program).Assembly.GetName().Version;
            if (assemblyVer != null)
            {
                string version = $"{assemblyVer.Major}.{assemblyVer.Minor}.{assemblyVer.Build}";
                return this.Ok(version);
            }

            return this.StatusCode(500);
        }
    }
}