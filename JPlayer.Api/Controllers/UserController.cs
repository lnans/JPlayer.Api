using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data;
using JPlayer.Data.Dto.User;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers
{
    /// <summary>
    ///     User management controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        ///     Get user list
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <response code="200">User list</response>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize(Roles = JPlayerRoles.UserRead)]
        [ProducesResponseType(typeof(ApiResult<Page<UserCollectionItem>>), 200)]
        public async Task<IActionResult> GetMany([FromQuery] UserCriteria criteria)
        {
            Page<UserCollectionItem> result = new Page<UserCollectionItem>
            {
                TotalCount = await this._userService.GetUsersCount(criteria),
                List = await this._userService.GetUsers(criteria),
                Skipped = criteria.Skip,
                Taked = criteria.Limit
            };
            return this.Ok(result.AsApiResult("userCollection"));
        }

        /// <summary>
        ///     Get a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">User entity</response>
        /// <response code="404">User not found</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = JPlayerRoles.UserRead)]
        [ProducesResponseType(typeof(ApiResult<UserEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<IActionResult> GetOne(int id)
        {
            try
            {
                UserEntity result = await this._userService.GetUser(id);
                return this.Ok(result.AsApiResult("userEntity"));
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
        }

        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">User created</response>
        /// <response code="400">An user already exist with this login</response>
        /// <response code="404">A given profile doesnt exist</response>
        [HttpPost("")]
        [Authorize(Roles = JPlayerRoles.UserWrite)]
        [ProducesResponseType(typeof(ApiResult<UserEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<IActionResult> Create([FromBody] UserCreateForm userCreateForm)
        {
            try
            {
                UserEntity result = await this._userService.CreateUser(userCreateForm);
                return this.Ok(result.AsApiResult("userEntity"));
            }
            catch (ApiAlreadyExistException e)
            {
                return this.BadRequest(e.AsApiError());
            }
            catch (ApiNotFoundException e)
            {
                return this.NotFound(e.AsApiError());
            }
        }

        /// <summary>
        ///     Update an user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userUpdateForm"></param>
        /// <returns></returns>
        /// <response code="200">User updated</response>
        /// <response code="404">User not found, or given profiles doesnt exist</response>
        /// <response code="409">User is in read only mode</response>
        [HttpPut("{id}")]
        [Authorize(Roles = JPlayerRoles.UserWrite)]
        [ProducesResponseType(typeof(ApiResult<UserEntity>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        [ProducesResponseType(typeof(ApiError), 409)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateForm userUpdateForm)
        {
            try
            {
                UserEntity result = await this._userService.UpdateUser(id, userUpdateForm);
                return this.Ok(result.AsApiResult("userEntity"));
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
        ///     Delete a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">User deleted</response>
        /// <response code="404">User not found</response>
        /// <response code="409">User is in read only mode</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = JPlayerRoles.UserWrite)]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        [ProducesResponseType(typeof(ApiError), 409)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await this._userService.DeleteUser(id);
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