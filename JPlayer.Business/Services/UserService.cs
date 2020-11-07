using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model.User;
using JPlayer.Data.Dto.User;
using JPlayer.Lib;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Business.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        ///     Return user in database
        /// </summary>
        /// <param name="userCriteria">Search filter</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserCollectionItem>> GetUsers(UserCriteria userCriteria)
        {
            userCriteria ??= new UserCriteria();
            List<UserDao> result = await this.UserFilterd(userCriteria)
                .Skip(userCriteria.Skip)
                .Take(userCriteria.Limit)
                .ToListAsync();

            return result.Select(u => new UserCollectionItem
            {
                Id = u.Id,
                Login = u.Login,
                CreationDate = u.CreationDate
            });
        }

        /// <summary>
        ///     Return number of user in database
        /// </summary>
        /// <param name="userCriteria">Search filter</param>
        /// <returns></returns>
        public async Task<int> GetUsersCount(UserCriteria userCriteria)
        {
            userCriteria ??= new UserCriteria();
            return await this.UserFilterd(userCriteria).CountAsync();
        }

        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <param name="userForm"></param>
        /// <returns></returns>
        public async Task<UserEntity> CreateUser(UserForm userForm)
        {
            UserDao newUser = new UserDao
            {
                CreationDate = DateTime.Now,
                Login = userForm.Login
            };

            await this._dbContext.Users.AddAsync(newUser);
            await this._dbContext.SaveChangesAsync();

            return new UserEntity
            {
                Login = newUser.Login,
                CreationDate = newUser.CreationDate
            };
        }

        private IQueryable<UserDao> UserFilterd(UserCriteria userCriteria)
        {
            IQueryable<UserDao> filtered = this._dbContext.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(userCriteria.Login))
                filtered = filtered.Where(u => u.Login.ToUpper().Contains(userCriteria.Login.ToUpper()));

            filtered = userCriteria.SortField.ToLower() switch
            {
                "login" => userCriteria.SortDir == SortDir.Asc ? filtered.OrderBy(o => o.Login) : filtered.OrderByDescending(o => o.Login),
                "creationdate" => userCriteria.SortDir == SortDir.Asc ? filtered.OrderBy(o => o.CreationDate) : filtered.OrderByDescending(o => o.CreationDate),
                _ => filtered
            };

            return filtered;
        }
    }
}