using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JPlayer.Test
{
    public abstract class AbstractBaseTest
    {
        private readonly IServiceProvider _controllerServiceProvider;

        protected ApplicationDbContext _dbContext;

        protected AbstractBaseTest()
        {
            // Create Mocks for httpContext SignIn method on controllers
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object) null));

            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            this._controllerServiceProvider = serviceProviderMock.Object;
        }

        protected void InitDbContext()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlite("Data Source=jplayer.test.db");
            this._dbContext = new ApplicationDbContext(builder.Options);
            this._dbContext.Database.EnsureCreated();
        }

        protected void CleanDbContext()
        {
            this._dbContext.Database.EnsureDeleted();
        }

        protected T CreateTestController<T>(T controller) where T : ControllerBase
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = this._controllerServiceProvider
                }
            };

            return controller;
        }
    }
}