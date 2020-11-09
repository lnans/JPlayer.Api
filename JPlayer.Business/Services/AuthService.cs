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

namespace JPlayer.Business.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<CredentialsInfo> SignInAsync(HttpContext httpContext, CredentialsForm credentialsForm)
        {
            // Check user in database
            UsrUserDao user = await this._dbContext.Users
                .Include(u => u.UserProfiles)
                .ThenInclude(up => up.Profile)
                .ThenInclude(p => p.ProfileFunctions)
                .ThenInclude(pf => pf.Function)
                .FirstOrDefaultAsync(u => u.Login == credentialsForm.Login);

            if (user == null)
                throw new AuthenticationException(GlobalLabelCodes.AuthAuthenticationFailed);

            if (!PasswordHelper.Check(user.Login, credentialsForm.Password, user.Password))
                throw new AuthenticationException(GlobalLabelCodes.AuthAuthenticationFailed);

            // Get user roles
            IEnumerable<string> roles = user.UserProfiles
                .SelectMany(up => up.Profile.ProfileFunctions)
                .Select(pf => pf.Function.FunctionCode)
                .ToList();

            // Create Identity
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.Now
            };

            // Sign in
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
        /// <param name="userId"></param>
        /// <param name="credentialsUpdateForm"></param>
        /// <returns></returns>
        public async Task UpdateCredentials(int userId, CredentialsUpdateForm credentialsUpdateForm)
        {
            UsrUserDao user = await this._dbContext.Users.FindAsync(userId);
            if (user == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            if (!PasswordHelper.Check(user.Login, credentialsUpdateForm.CurrentPassword, user.Password))
                throw new AuthenticationException(GlobalLabelCodes.AuthWrongPassword);

            user.Password = PasswordHelper.Crypt(user.Login, credentialsUpdateForm.NewPassword);
            await this._dbContext.SaveChangesAsync();
        }
    }
}