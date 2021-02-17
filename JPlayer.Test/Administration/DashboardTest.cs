using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business.Services;
using JPlayer.Data;
using JPlayer.Data.Dto.Dashboard;
using JPlayer.Lib.Contract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class DashboardTest : AbstractBaseTest
    {
        [SetUp]
        public void Setup()
        {
            this.InitDbContext();
            this._loggerDashboardService = new NLogLoggerFactory().CreateLogger<DashboardService>();
            this._loggerDashboardController = new NLogLoggerFactory().CreateLogger<DashboardController>();
            this._dashboardService = new DashboardService(this._loggerDashboardService, this._dbContext);
            this._dashboardController =
                this.CreateTestController(new DashboardController(this._loggerDashboardController,
                    this._dashboardService));
        }

        [TearDown]
        public void TearDown()
        {
            this.CleanDbContext();
        }

        private ILogger<DashboardService> _loggerDashboardService;
        private ILogger<DashboardController> _loggerDashboardController;
        private DashboardService _dashboardService;
        private DashboardController _dashboardController;

        [Test]
        public async Task GetAdministrationTiles_ShouldReturn_TilesInformationList()
        {
            // Create fake authentication
            string[] roles = {JPlayerRoles.UserRead, JPlayerRoles.ProfileRead};
            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, "UserAdmin"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            this._dashboardController.HttpContext.User = new ClaimsPrincipal(identity);

            IActionResult actionResult = await this._dashboardController.GetAdministrationTiles();
            ObjectResult result = actionResult as ObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<IEnumerable<TileCollectionItem>> tiles =
                result.Value as ApiResult<IEnumerable<TileCollectionItem>>;
            Assert.IsNotNull(tiles);
            Assert.Greater(tiles.Data.Count(), 0);
        }
    }
}