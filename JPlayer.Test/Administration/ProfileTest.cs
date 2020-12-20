using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Profile;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class ProfileTest : AbstractBaseTest
    {
        [SetUp]
        public void Setup()
        {
            this.InitDbContext();
            this._loggerProfileService = new NLogLoggerFactory().CreateLogger<ProfileService>();
            this._loggerProfileController = new NLogLoggerFactory().CreateLogger<ProfileController>();
            this._profileService = new ProfileService(this._loggerProfileService, this._dbContext, new ObjectMapper());
            this._profileController = this.CreateTestController(new ProfileController(this._loggerProfileController, this._profileService));
        }

        [TearDown]
        public void TearDown()
        {
            this.CleanDbContext();
        }

        private ILogger<ProfileService> _loggerProfileService;
        private ILogger<ProfileController> _loggerProfileController;
        private ProfileService _profileService;
        private ProfileController _profileController;

        [Test]
        public async Task GetMany_ShouldReturn_ProfileList_WithStatus200()
        {
            IActionResult actionResult = await this._profileController.GetMany(new ProfileCriteria());
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<Page<ProfileCollectionItem>> profiles = result.Value as ApiResult<Page<ProfileCollectionItem>>;

            Assert.IsNotNull(profiles);
            Assert.AreEqual(1, profiles.Data.List.Count());
            Assert.AreEqual(1, profiles.Data.TotalCount);
            Assert.AreEqual(1, profiles.Data.List.First().Id);
        }

        [Test]
        public async Task GetOne_KnownProfile_ShouldReturn_Profile_WithStatus200()
        {
            IActionResult actionResult = await this._profileController.GetOne(1);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<ProfileEntity> profile = result.Value as ApiResult<ProfileEntity>;

            Assert.IsNotNull(profile);
            Assert.AreEqual(1, profile.Data.Id);
        }

        [Test]
        public async Task GetOne_UnknownProfile_ShouldReturn_ApiError_WithStatus404()
        {
            IActionResult actionResult = await this._profileController.GetOne(99);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.NotFound, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.ProfileNotFound, error.Error);
        }

        [Test]
        public async Task CreateOne_NewProfile_ShouldReturn_NewProfile_WithStatus200()
        {
            ProfileCreateForm profileCreateForm = new ProfileCreateForm
            {
                Name = "FakeProfile",
                FunctionIds = new[] {1}
            };
            IActionResult actionResult = await this._profileController.CreateOne(profileCreateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<ProfileEntity> profile = result.Value as ApiResult<ProfileEntity>;

            Assert.IsNotNull(profile);
            Assert.AreEqual("FakeProfile", profile.Data.Name);
        }

        [Test]
        public async Task CreateOne_AlreadyExistProfile_ShouldReturn_ApiError_WithStatus400()
        {
            ProfileCreateForm profileCreateForm = new ProfileCreateForm
            {
                Name = "Administrator",
                FunctionIds = new[] {1}
            };
            IActionResult actionResult = await this._profileController.CreateOne(profileCreateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.BadRequest, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(error.Error, GlobalLabelCodes.ProfileAlreadyExist);
        }

        [Test]
        public async Task UpdateOne_ReadOnlyProfile_ShouldReturn_ApiError_WithStatus409()
        {
            ProfileUpdateForm profileUpdateForm = new ProfileUpdateForm
            {
                FunctionIds = new[] {1}
            };
            IActionResult actionResult = await this._profileController.UpdateOne(1, profileUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.Conflict, result.StatusCode);

            ApiError error = result.Value as ApiError;

            Assert.IsNotNull(error);
            Assert.AreEqual(GlobalLabelCodes.ProfileReadOnly, error.Error);
        }

        public async Task UpdateOne_KnownProfile_ShouldReturn_ApiError_WithStatus200()
        {
            ProfileUpdateForm profileUpdateForm = new ProfileUpdateForm
            {
                FunctionIds = new[] {1}
            };
            IActionResult actionResult = await this._profileController.UpdateOne(1, profileUpdateForm);
            ObjectResult result = actionResult as ObjectResult;

            Assert.IsNotNull(result);
        }
    }
}