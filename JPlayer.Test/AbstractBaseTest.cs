using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Data;
using JPlayer.Data.Dao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JPlayer.Test
{
    public abstract class AbstractBaseTest
    {
        private readonly IServiceProvider _controllerServiceProvider;

        protected ApplicationDbContext DbContext;

        protected AbstractBaseTest()
        {
            // Create Mocks for httpContext SignIn method on controllers
            Mock<IAuthenticationService> authServiceMock = new();
            authServiceMock
                .Setup(_ =>
                    _.SignInAsync(It.IsAny<HttpContext>(),
                        It.IsAny<string>(),
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<AuthenticationProperties>()
                    )
                )
                .Returns(Task.FromResult((object) null));

            Mock<IServiceProvider> serviceProviderMock = new();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            this._controllerServiceProvider = serviceProviderMock.Object;
        }

        protected void InitDbContext()
        {
            DbContextOptionsBuilder builder = new();
            builder.UseSqlite("Data Source=jplayer.test.db");
            this.DbContext = new ApplicationDbContext(builder.Options);
            this.DbContext.Database.EnsureCreated();
        }

        protected void CleanDbContext()
        {
            this.DbContext.Database.EnsureDeleted();
        }

        protected static ClaimsPrincipal CreateUser(string login, string id)
        {
            string[] roles =
            {
                JPlayerRoles.UserRead,
                JPlayerRoles.ProfileRead,
                JPlayerRoles.UserWrite,
                JPlayerRoles.ProfileWrite
            };
            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, login));
            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            if (id != null) identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));

            return new ClaimsPrincipal(identity);
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