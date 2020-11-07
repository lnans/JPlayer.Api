using System.Threading.Tasks;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.User;
using JPlayer.Lib;
using Microsoft.AspNetCore.Mvc;

namespace JPlayer.Api.Controllers.User
{
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
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(ApiResult<Page<UserCollectionItem>>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] UserCriteria criteria)
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
        ///     Create a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(ApiResult<UserEntity>), 200)]
        public async Task<IActionResult> CreateUser([FromBody] UserForm userForm) => this.Ok(await this._userService.CreateUser(userForm));
    }
}