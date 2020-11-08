using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Function;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FunctionController : ControllerBase
    {
        private readonly FunctionService _functionService;

        public FunctionController(FunctionService functionService)
        {
            this._functionService = functionService;
        }

        /// <summary>
        ///     Get functions list
        /// </summary>
        /// <param name="functionCriteria">search filter</param>
        /// <returns></returns>
        /// <response code="200">Functions list returned</response>
        [HttpGet("")]
        [ProducesResponseType(typeof(ApiResult<Page<FunctionCollectionItem>>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] FunctionCriteria functionCriteria)
        {
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