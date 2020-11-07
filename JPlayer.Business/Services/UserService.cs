using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
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
            List<UsrUserDao> result = await this.UserFilterd(userCriteria)
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
            UsrUserDao result = await this._dbContext.Users.FindAsync(id);
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

            UsrUserDao newUsrUser = new UsrUserDao
            {
                CreationDate = DateTime.Now,
                Login = userCreateForm.Login
            };

            await this._dbContext.Users.AddAsync(newUsrUser);
            await this._dbContext.SaveChangesAsync();

            return new UserEntity
            {
                Id = newUsrUser.Id,
                Login = newUsrUser.Login,
                Deactivated = newUsrUser.Deactivated,
                CreationDate = newUsrUser.CreationDate
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
            UsrUserDao usrUser = await this._dbContext.Users.FindAsync(id);
            if (usrUser == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            usrUser.Deactivated = userCreateForm.Deactivated;
            await this._dbContext.SaveChangesAsync();
            return new UserEntity
            {
                Id = usrUser.Id,
                Login = usrUser.Login,
                Deactivated = usrUser.Deactivated,
                CreationDate = usrUser.CreationDate,
                LastConnectionDate = usrUser.LastConnectionDate
            };
        }

        /// <summary>
        ///     Delete an user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUser(int id)
        {
            UsrUserDao usrUser = await this._dbContext.Users.FindAsync(id);
            if (usrUser == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            this._dbContext.Remove(usrUser);
            await this._dbContext.SaveChangesAsync();
        }

        private IQueryable<UsrUserDao> UserFilterd(UserCriteria userCriteria)
        {
            IQueryable<UsrUserDao> filtered = this._dbContext.Users.AsQueryable();
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