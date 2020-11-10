using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data;
using JPlayer.Data.Dto.Profile;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JPlayer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ProfileService _profileService;

        public ProfileController(ILogger<ProfileController> logger, ProfileService profileService)
        {
            this._logger = logger;
            this._profileService = profileService;
        }

        /// <summary>
        ///     Get profile list
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns></returns>
        /// <response code="200">Profile list returned</response>
        [HttpGet("")]
        [Authorize(Roles = JPlayerRoles.ProfileRead)]
        [ProducesResponseType(typeof(ApiResult<Page<ProfileCollectionItem>>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] ProfileCriteria criteria)
        {
            this._logger.LogInformation("Retrieve profile list");
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
        [Authorize(Roles = JPlayerRoles.ProfileRead)]
        [ProducesResponseType(typeof(ApiResult<ProfileEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<IActionResult> GetOne(int id)
        {
            try
            {
                this._logger.LogInformation($"Retrieve profile with id {id}");
                ProfileEntity result = await this._profileService.GetOne(id);
                return this.Ok(result.AsApiResult("profileEntity"));
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
        }

        /// <summary>
        ///     Create a new profile
        /// </summary>
        /// <param name="createForm"></param>
        /// <returns></returns>
        /// <response code="200">Profile created</response>
        /// <response code="400">Profile name already exist</response>
        /// <response code="404">One or more given functions not exist</response>
        [HttpPost("")]
        [Authorize(Roles = JPlayerRoles.ProfileWrite)]
        [ProducesResponseType(typeof(ApiResult<ProfileEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<IActionResult> CreateOne([FromBody] ProfileCreateForm createForm)
        {
            try
            {
                this._logger.LogInformation("Create profile attempt");
                ProfileEntity result = await this._profileService.CreateOne(createForm);
                return this.Ok(result.AsApiResult("profileEntity"));
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
            catch (ApiAlreadyExistException e)
            {
                return this.BadRequest(e.AsApiError());
            }
        }

        /// <summary>
        ///     Update a specific profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateForm"></param>
        /// <returns></returns>
        /// <response code="200">Profile updated</response>
        /// <response code="404">One or more given functions not exist</response>
        /// <response code="409">Profile is in read only mode</response>
        [HttpPut("{id}")]
        [Authorize(Roles = JPlayerRoles.ProfileWrite)]
        [ProducesResponseType(typeof(ApiResult<ProfileEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        [ProducesResponseType(typeof(ApiError), 409)]
        public async Task<IActionResult> UpdateOne(int id, [FromBody] ProfileUpdateForm updateForm)
        {
            try
            {
                this._logger.LogInformation($"Update profile attempt with id {id}");
                ProfileEntity result = await this._profileService.UpdateOne(id, updateForm);
                return this.Ok(result.AsApiResult("profileEntity"));
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
            catch (ApiException e)
            {
                return this.Conflict(e.AsApiError());
            }
        }

        /// <summary>
        ///     Delete a specific profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Profile deleted</response>
        /// <response code="404">Profile not found</response>
        /// <response code="409">Profile is in read only mode</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = JPlayerRoles.ProfileWrite)]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        [ProducesResponseType(typeof(ApiError), 409)]
        public async Task<IActionResult> DeleteOne(int id)
        {
            try
            {
                this._logger.LogInformation($"Delete profile attempt with id {id}");
                await this._profileService.DeleteOne(id);
                return this.Ok(true.AsApiResult());
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
            catch (ApiException e)
            {
                return this.Conflict(e.AsApiError());
            }
        }
    }
}