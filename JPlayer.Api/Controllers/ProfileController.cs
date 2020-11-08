using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Profile;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            this._profileService = profileService;
        }

        /// <summary>
        ///     Get profile list
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns></returns>
        /// <response code="200">Profile list returned</response>
        [HttpGet("")]
        [ProducesResponseType(typeof(ApiResult<Page<ProfileCollectionItem>>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] ProfileCriteria criteria)
        {
            Page<ProfileCollectionItem> result = new Page<ProfileCollectionItem>
            {
                TotalCount = await this._profileService.GetCount(criteria),
                List = await this._profileService.GetMany(criteria),
                Skipped = criteria.Skip,
                Taked = criteria.Limit
            };

            return this.Ok(result.AsApiResult("profileCollection"));
        }

        /// <summary>
        ///     Get a specific profile
        /// </summary>
        /// <param name="id">Profile id</param>
        /// <returns></returns>
        /// <response code="200">Profile returned</response>
        /// <response code="404">Profile not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProfileEntity>), 200)]
        [ProducesResponseType(typeof(ApiResult<string>), 404)]
        public async Task<IActionResult> GetOne(int id)
        {
            try
            {
                ProfileEntity result = await this._profileService.GetOne(id);
                return this.Ok(result.AsApiResult("profileEntity"));
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.Message.AsApiError());
            }
        }
    }
}