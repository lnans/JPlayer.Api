﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Profile;
using JPlayer.Data.Dto.User;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Crypto;
using JPlayer.Lib.Exception;
using JPlayer.Lib.Mapper;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Business.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ObjectMapper _mapper;

        public UserService(ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
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

            return result.Select(user => this._mapper.Map<UserCollectionItem, UsrUserDao>(user));
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
            UsrUserDao user = await this._dbContext.Users
                .Include(u => u.UserProfiles)
                .ThenInclude(up => up.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new ApiNotFoundException(GlobalLabelCodes.UserNotFound);

            UserEntity result = this._mapper.Map<UserEntity, UsrUserDao>(user);
            result.Profiles = user.UserProfiles.Select(up => this._mapper.Map<ProfileCollectionItem, UsrProfileDao>(up.Profile));
            return result;
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
                Login = userCreateForm.Login,
                Password = PasswordHelper.Crypt(userCreateForm.Login, userCreateForm.Password)
            };

            List<UsrUserProfileDao> userProfiles = new List<UsrUserProfileDao>();
            foreach (int profileId in userCreateForm.Profiles)
            {
                UsrProfileDao profile = await this._dbContext.Profiles.FindAsync(profileId);
                if (profile == null)
                    throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);

                userProfiles.Add(new UsrUserProfileDao {User = newUsrUser, Profile = profile});
            }

            newUsrUser.UserProfiles = userProfiles;
            await this._dbContext.Users.AddAsync(newUsrUser);
            await this._dbContext.SaveChangesAsync();

            UserEntity result = this._mapper.Map<UserEntity, UsrUserDao>(newUsrUser);
            result.Profiles = newUsrUser.UserProfiles.Select(up => this._mapper.Map<ProfileCollectionItem, UsrProfileDao>(up.Profile));
            return result;
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

            if(usrUser.ReadOnly)
                throw new ApiException(GlobalLabelCodes.UserReadOnly);

            // Get current profiles of the user
            IQueryable<UsrUserProfileDao> userProfiles = this._dbContext.UserProfiles.Where(pf => pf.UserId == id);
            foreach (int profileId in userCreateForm.Profiles)
            {
                UsrProfileDao profile = await this._dbContext.Profiles.FirstOrDefaultAsync(f => f.Id == profileId);
                if (profile == null)
                    throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);

                // Add profiles to the user if not exist
                if (!userProfiles.Any(up => up.ProfileId == profileId))
                    await this._dbContext.UserProfiles.AddAsync(new UsrUserProfileDao {UserId = id, ProfileId = profileId});
            }

            // Remove a function from the profile if not given
            foreach (UsrUserProfileDao userProfile in userProfiles)
            {
                if (userCreateForm.Profiles.All(p => p != userProfile.ProfileId))
                    this._dbContext.UserProfiles.Remove(userProfile);
            }

            await this._dbContext.SaveChangesAsync();

            UserEntity result = this._mapper.Map<UserEntity, UsrUserDao>(usrUser);
            result.Profiles = usrUser.UserProfiles.Select(up => this._mapper.Map<ProfileCollectionItem, UsrProfileDao>(up.Profile));
            return result;
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

            if (usrUser.ReadOnly)
                throw new ApiException(GlobalLabelCodes.UserReadOnly);

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