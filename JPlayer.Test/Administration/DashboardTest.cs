using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Dashboard;
using JPlayer.Lib.Contract;
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
            this._dashboardService = new DashboardService(this._loggerDashboardService, this.DbContext);
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
            // Arrange authentication
            this._dashboardController.HttpContext.User = CreateUser("UserAdmin", "1");

            // Act
            IActionResult actionResult = await this._dashboardController.GetAdministrationTiles();
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<IEnumerable<TileCollectionItem>> tiles =
                result.Value as ApiResult<IEnumerable<TileCollectionItem>>;
            Assert.IsNotNull(tiles);
            Assert.Greater(tiles.Data.Count(), 0);
        }

        [Test]
        public async Task GetMenuList_ShouldReturn_MenuItemsList()
        {
            // Arrange authentication
            this._dashboardController.HttpContext.User = CreateUser("UserAdmin", "1");

            // Act
            IActionResult actionResult = await this._dashboardController.GetMenuList();
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<IEnumerable<string>> menuItems =
                result.Value as ApiResult<IEnumerable<string>>;
            Assert.IsNotNull(menuItems);
            Assert.Greater(menuItems.Data.Count(), 0);
            
        }
    }
}