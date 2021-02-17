using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data;
using JPlayer.Data.Dto.Dashboard;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JPlayer.Api.Controllers
{
    /// <summary>
    ///     Controller to get tiles information for different dashboards
    /// </summary>
    [ApiController]
    [Route("dashboard")]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger, DashboardService dashboardService)
        {
            this._dashboardService = dashboardService;
            this._logger = logger;
        }

        /// <summary>
        ///     Return available administration tiles for the current logged user
        /// </summary>
        /// <returns></returns>
        [HttpGet("administration")]
        [Authorize(Roles = JPlayerRoles.UserRead + "," + JPlayerRoles.ProfileRead)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<TileCollectionItem>>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAdministrationTiles()
        {
            this._logger.LogInformation("Retrieve administration tiles information");
            IEnumerable<string> functions = this.HttpContext.User.Claims
                .Where(cl => cl.Type == ClaimTypes.Role)
                .Select(cl => cl.Value);

            IEnumerable<TileCollectionItem> result = await this._dashboardService.GetAdminTiles(functions.ToArray());
            return this.Ok(result.AsApiResult("administrationTiles"));
        }
    }
}