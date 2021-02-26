using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business;
using JPlayer.Business.Services;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.User;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Crypto;
using JPlayer.Lib.Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class UserTest : AbstractBaseTest
    {
        [SetUp]
        public void Setup()
        {
            this.InitDbContext();
            this._loggerUserService = new NLogLoggerFactory().CreateLogger<UserService>();
            this._loggerUserController = new NLogLoggerFactory().CreateLogger<UserController>();
            this._userService = new UserService(this._loggerUserService, this.DbContext, new ObjectMapper());
            this._userController =
                this.CreateTestController(new UserController(this._loggerUserController, this._userService));
        }

        [TearDown]
        public void TearDown()
        {
            this.CleanDbContext();
        }

        private ILogger<UserService> _loggerUserService;
        private ILogger<UserController> _loggerUserController;
        private UserService _userService;
        private UserController _userController;

        [Test]
        public async Task GetMany_ShouldReturn_UserList_WithStatus200()
        {
            IActionResult actionResult = await this._userController.GetMany(new UserCriteria());
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<Page<UserCollectionItem>> users = result.Value as ApiResult<Page<UserCollectionItem>>;

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Data.List.Count());
            Assert.AreEqual(1, users.Data.TotalCount);
            Assert.AreEqual("UserAdmin", users.Data.List.First().Login);
        }

        [Test]
        public async Task GetOne_KnownUser_ShouldReturn_User_WithStatus200()
        {
            IActionResult actionResult = await this._userController.GetOne(1);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<UserEntity> user = result.Value as ApiResult<UserEntity>;

            Assert.IsNotNull(user);
            Assert.AreEqual("UserAdmin", user.Data.Login);
            Assert.AreEqual(1, user.Data.Profiles.Count());
        }

        [Test]
        public async Task GetOne_UnknownUser_ShouldReturn_ApiError_WithStatus404()
        {
            IActionResult actionResult = await this._userController.GetOne(99);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserNotFound, error.Error);
        }

        [Test]
        public async Task PostOne_NewUser_ShouldReturn_Status200()
        {
            UserCreateForm userCreateForm = new() {Login = "NewUser", Password = "NewUser", Profiles = new[] {1}};
            IActionResult actionResult = await this._userController.Create(userCreateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<UserEntity> user = result.Value as ApiResult<UserEntity>;

            Assert.IsNotNull(user);
            Assert.AreEqual("NewUser", user.Data.Login);
            Assert.AreEqual(1, user.Data.Profiles.Count());

            UsrUserDao userDb = await this.DbContext.Users.FindAsync(user.Data.Id);

            Assert.IsNotNull(userDb);
            Assert.AreEqual("NewUser", userDb.Login);
            Assert.AreEqual(1, userDb.UserProfiles.Count);
        }

        [Test]
        public async Task PostOne_NewUser_WithUnknownProfile_ShouldReturn_ApiError_WithStatus404()
        {
            UserCreateForm userCreateForm = new() {Login = "NewUser", Password = "NewUser", Profiles = new[] {99}};
            IActionResult actionResult = await this._userController.Create(userCreateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.ProfileNotFound, error.Error);
        }

        [Test]
        public async Task PostOne_KnownUser_ShouldReturn_ApiError_WithStatus400()
        {
            UserCreateForm userCreateForm = new() {Login = "UserAdmin", Password = "NewUser", Profiles = new[] {1}};
            IActionResult actionResult = await this._userController.Create(userCreateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.BadRequest, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserAlreadyExist, error.Error);
        }

        [Test]
        public async Task PutOne_KnownUser_ShouldReturn_Status200()
        {
            UsrUserDao fakeUser = new() {Login = "FakeUser", Password = PasswordHelper.Crypt("FakeUser", "FakeUser")};
            await this.DbContext.Users.AddAsync(fakeUser);
            await this.DbContext.SaveChangesAsync();

            UserUpdateForm userUpdateForm = new() {Profiles = new[] {1}};
            IActionResult actionResult = await this._userController.Update(fakeUser.Id, userUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<UserEntity> user = result.Value as ApiResult<UserEntity>;

            Assert.IsNotNull(user);
            Assert.AreEqual("FakeUser", user.Data.Login);
            Assert.AreEqual(1, user.Data.Profiles.Count());
        }

        [Test]
        public async Task PutOne_ReadOnlyUser_ShoudReturn_Status409()
        {
            UserUpdateForm userUpdateForm = new() {Profiles = new[] {1}};
            IActionResult actionResult = await this._userController.Update(1, userUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Conflict, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserReadOnly, error.Error);
        }

        [Test]
        public async Task PutOne_UnkownUser_ShouldReturn_Status404()
        {
            UserUpdateForm userUpdateForm = new() {Profiles = new[] {1}};
            IActionResult actionResult = await this._userController.Update(99, userUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserNotFound, error.Error);
        }

        [Test]
        public async Task DeleteOne_KnownUser_ShouldReturn_Status200()
        {
            UsrUserDao fakeUser = new() {Login = "FakeUser", Password = PasswordHelper.Crypt("FakeUser", "FakeUser")};
            await this.DbContext.Users.AddAsync(fakeUser);
            await this.DbContext.SaveChangesAsync();

            IActionResult actionResult = await this._userController.Delete(fakeUser.Id);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, this.DbContext.Users.Count());
        }

        [Test]
        public async Task DeleteOne_ReadOnlyUser_ShouldReturn_Status409()
        {
            IActionResult actionResult = await this._userController.Delete(1);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Conflict, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserReadOnly, error.Error);
        }

        [Test]
        public async Task DeleteOne_UnkownUser_ShouldReturn_Status404()
        {
            IActionResult actionResult = await this._userController.Delete(99);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.UserNotFound, error.Error);
        }
    }
}