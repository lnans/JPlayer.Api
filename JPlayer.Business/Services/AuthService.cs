using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Credentials;
using JPlayer.Lib.Crypto;
using JPlayer.Lib.Exception;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JPlayer.Business.Services
{
    /// <summary>
    ///     Service use to authenticate users
    ///     Can be use to change credentials
    /// </summary>
    public class AuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ILogger<AuthService> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        /// <summary>
        ///     Sign In a known user into the system
        /// </summary>
        /// <param name="httpContext">Current controller context</param>
        /// <param name="credentialsForm">Credentials of the user</param>
        /// <exception cref="AuthenticationException"></exception>
        /// <returns>User information</returns>
        public async Task<CredentialsInfo> SignInAsync(HttpContext httpContext, CredentialsForm credentialsForm)
        {
            // Check user in database
            this._logger.LogInformation("Get user information");
            UsrUserDao user = await this._dbContext.Users
                .Include(u => u.UserProfiles)
                .ThenInclude(up => up.Profile)
                .ThenInclude(p => p.ProfileFunctions)
                .ThenInclude(pf => pf.Function)
                .FirstOrDefaultAsync(u => u.Login == credentialsForm.Login);

            if (user == null)
            {
                this._logger.LogInformation("User not found");
                throw new AuthenticationException(GlobalLabelCodes.AuthAuthenticationFailed);
            }

            if (!PasswordHelper.Check(user.Login, credentialsForm.Password, user.Password))
            {
                this._logger.LogInformation("Wrong password");
                throw new AuthenticationException(GlobalLabelCodes.AuthAuthenticationFailed);
            }

            // Get user roles
            IEnumerable<string> roles = user.UserProfiles
                .SelectMany(up => up.Profile.ProfileFunctions)
                .Select(pf => pf.Function.FunctionCode)
                .ToList();

            // Create Identity
            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            ClaimsPrincipal principal = new(identity);
            AuthenticationProperties authProperties = new()
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.Now
            };

            // Sign in
            user.LastConnectionDate = DateTime.Now;
            await this._dbContext.SaveChangesAsync();
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return new CredentialsInfo
            {
                Login = user.Login,
                Functions = roles
            };
        }

        /// <summary>
        ///     Update credentials for the current logged user
        /// </summary>
        /// <param name="userId">NameIdentifier of the user</param>
        /// <param name="credentialsUpdateForm">User credentials</param>
        /// <exception cref="ApiNotFoundException"></exception>
        /// <exception cref="AuthenticationException"></exception>
        /// <returns></returns>
        public async Task UpdateCredentials(int userId, CredentialsUpdateForm credentialsUpdateForm)
        {
            UsrUserDao user = await this._dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                this._logger.LogInformation("User not found");
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);
            }

            if (!credentialsUpdateForm.NewPassword.Equals(credentialsUpdateForm.RetypePassword))
            {
                this._logger.LogInformation("New password and retype password are not the same");
                throw new AuthenticationException(GlobalLabelCodes.AuthPasswordNotSame);
            }
            if (!PasswordHelper.Check(user.Login, credentialsUpdateForm.CurrentPassword, user.Password))
            {
                this._logger.LogInformation("Wrong password");
                throw new AuthenticationException(GlobalLabelCodes.AuthWrongPassword);
            }

            user.Password = PasswordHelper.Crypt(user.Login, credentialsUpdateForm.NewPassword);
            await this._dbContext.SaveChangesAsync();
        }
    }
}