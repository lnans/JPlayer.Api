using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JPlayer.Api.Controllers;
using JPlayer.Business.Services;
using JPlayer.Data.Dto.Function;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace JPlayer.Test.Administration
{
    [TestFixture]
    public class FunctionTest : AbstractBaseTest
    {
        [SetUp]
        public void Setup()
        {
            this.InitDbContext();
            this._loggerFunctionService = new NLogLoggerFactory().CreateLogger<FunctionService>();
            this._loggerFunctionController = new NLogLoggerFactory().CreateLogger<FunctionsController>();
            this._functionService =
                new FunctionService(this._loggerFunctionService, this.DbContext, new ObjectMapper());
            this._functionController =
                this.CreateTestController(new FunctionsController(this._loggerFunctionController,
                    this._functionService));
        }

        [TearDown]
        public void TearDown()
        {
            this.CleanDbContext();
        }

        private ILogger<FunctionService> _loggerFunctionService;
        private ILogger<FunctionsController> _loggerFunctionController;
        private FunctionService _functionService;
        private FunctionsController _functionController;

        [Test]
        public async Task GetMany_ShouldReturn_FunctionList_WithStatus200()
        {
            // Act
            IActionResult actionResult = await this._functionController.GetMany(new FunctionCriteria());
            ObjectResult result = actionResult as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual((int) HttpStatusCode.OK, result.StatusCode);

            ApiResult<Page<FunctionCollectionItem>> functions = result.Value as ApiResult<Page<FunctionCollectionItem>>;

            Assert.IsNotNull(functions);
            Assert.Greater(functions.Data.List.Count(), 0);
        }
    }
}