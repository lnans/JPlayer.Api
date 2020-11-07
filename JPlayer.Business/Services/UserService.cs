using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model.User;
using JPlayer.Data.Dto.User;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
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
                Deactivated = u.Deactivated,
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
        ///     Return a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserEntity> GetUser(int id)
        {
            UserDao result = await this._dbContext.Users.FindAsync(id);
            if (result == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);
            return new UserEntity
            {
                Id = result.Id,
                Login = result.Login,
                Deactivated = result.Deactivated,
                CreationDate = result.CreationDate,
                LastConnectionDate = result.LastConnectionDate
            };
        }

        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <param name="userCreateForm"></param>
        /// <returns></returns>
        public async Task<UserEntity> CreateUser(UserCreateForm userCreateForm)
        {
            if (await this._dbContext.Users.AnyAsync(u => u.Login == userCreateForm.Login))
                throw new ApiAlreadyExistException(GlobalLabelCodes.UserAlreadyExist);

            UserDao newUser = new UserDao
            {
                CreationDate = DateTime.Now,
                Login = userCreateForm.Login
            };

            await this._dbContext.Users.AddAsync(newUser);
            await this._dbContext.SaveChangesAsync();

            return new UserEntity
            {
                Id = newUser.Id,
                Login = newUser.Login,
                Deactivated = newUser.Deactivated,
                CreationDate = newUser.CreationDate
            };
        }

        /// <summary>
        ///     Update an user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userCreateForm"></param>
        /// <returns></returns>
        public async Task<UserEntity> UpdateUser(int id, UserUpdateForm userCreateForm)
        {
            UserDao user = await this._dbContext.Users.FindAsync(id);
            if (user == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            user.Deactivated = userCreateForm.Deactivated;
            await this._dbContext.SaveChangesAsync();
            return new UserEntity
            {
                Id = user.Id,
                Login = user.Login,
                Deactivated = user.Deactivated,
                CreationDate = user.CreationDate,
                LastConnectionDate = user.LastConnectionDate
            };
        }

        /// <summary>
        ///     Delete an user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUser(int id)
        {
            UserDao user = await this._dbContext.Users.FindAsync(id);
            if (user == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            this._dbContext.Remove(user);
            await this._dbContext.SaveChangesAsync();
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