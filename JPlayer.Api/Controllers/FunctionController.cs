using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data;
using JPlayer.Data.Dto.Function;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JPlayer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FunctionController : ControllerBase
    {
        private readonly FunctionService _functionService;
        private readonly ILogger<FunctionController> _logger;

        public FunctionController(ILogger<FunctionController> logger, FunctionService functionService)
        {
            this._logger = logger;
            this._functionService = functionService;
        }

        /// <summary>
        ///     Get functions list
        /// </summary>
        /// <param name="functionCriteria">search filter</param>
        /// <returns></returns>
        /// <response code="200">Functions list returned</response>
        [HttpGet("")]
        [Authorize(Roles = JPlayerRoles.ProfileRead)]
        [ProducesResponseType(typeof(ApiResult<Page<FunctionCollectionItem>>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] FunctionCriteria functionCriteria)
        {
            this._logger.LogInformation("Retrieve function list");
            Page<FunctionCollectionItem> result = new Page<FunctionCollectionItem>
            {
                TotalCount = await this._functionService.GetCount(functionCriteria),
                List = await this._functionService.GetMany(functionCriteria),
                Skipped = functionCriteria.Skip,
                Taked = functionCriteria.Limit
            };

            return this.Ok(result.AsApiResult("functionCollection"));
        }
    }
}