using System.Collections.Generic;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.SystemInfo;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace JPlayer.Api.Controllers
{
    /// <summary>
    ///     Get Hardware information
    /// </summary>
    [ApiController]
    [Route("system")]
    [Produces("application/json")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        private readonly SystemService _systemService;

        public SystemController(ILogger<SystemController> logger, SystemService systemService)
        {
            this._logger = logger;
            this._systemService = systemService;
        }

        /// <summary>
        ///     Get system information historic (one tick is 5sec)
        /// </summary>
        /// <param name="ticks">Ticks to get</param>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<SystemInfoCollectionItem>>), 200)]
        [SwaggerOperation(Tags = new[] {SwaggerTags.InformationSection})]
        public IActionResult Get([FromQuery] int ticks) =>
            this.Ok(this._systemService.GetSystemInfoHistory(ticks).AsApiResult("systemInfo"));
    }
}