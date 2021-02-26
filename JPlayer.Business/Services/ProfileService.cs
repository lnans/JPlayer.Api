using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Function;
using JPlayer.Data.Dto.Profile;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using JPlayer.Lib.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JPlayer.Business.Services
{
    /// <summary>
    ///     Profile service for GET, CREATE, UPDATE and DELETE profiles
    ///     Functions can be assigned on profile creation.
    /// </summary>
    public class ProfileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProfileService> _logger;
        private readonly ObjectMapper _mapper;

        public ProfileService(ILogger<ProfileService> logger, ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        /// <summary>
        ///     Get profiles in database
        ///     Can be filtered with a criteria object
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Paginated profile list</returns>
        public async Task<IEnumerable<ProfileCollectionItem>> GetMany(ProfileCriteria criteria)
        {
            criteria ??= new ProfileCriteria();
            this._logger.LogInformation("Get filtered profile list with search criteria: {Criteria}",
                criteria.ToJson());
            List<UsrProfileDao> result = await this.ProfileFiltered(criteria)
                .Skip(criteria.Skip)
                .Take(criteria.Limit)
                .ToListAsync();

            return result.Select(pf => this._mapper.Map<ProfileCollectionItem, UsrProfileDao>(pf));
        }

        /// <summary>
        ///     Get profiles count in database
        ///     Can be filtered with a criteria object
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Profiles count</returns>
        public async Task<int> GetCount(ProfileCriteria criteria)
        {
            criteria ??= new ProfileCriteria();
            return await this.ProfileFiltered(criteria).CountAsync();
        }

        /// <summary>
        ///     Get a specific profile
        /// </summary>
        /// <param name="id">Profile id</param>
        /// <exception cref="ApiNotFoundException"></exception>
        /// <returns></returns>
        public async Task<ProfileEntity> GetOne(int id)
        {
            UsrProfileDao profile = await this._dbContext.Profiles
                .Include(p => p.ProfileFunctions)
                .ThenInclude(pf => pf.Function)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                this._logger.LogInformation("profile not found");
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);
            }

            ProfileEntity result = this._mapper.Map<ProfileEntity, UsrProfileDao>(profile);
            result.Functions = profile.ProfileFunctions.Select(pf =>
                this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Create a profile
        /// </summary>
        /// <param name="createForm">New profile information</param>
        /// <exception cref="ApiNotFoundException"></exception>
        /// <returns>Profile created</returns>
        public async Task<ProfileEntity> CreateOne(ProfileCreateForm createForm)
        {
            // Check if profile already exist
            if (await this._dbContext.Profiles.AnyAsync(p => p.Name.ToLower() == createForm.Name.ToLower()))
            {
                this._logger.LogInformation("Profile already exist");
                throw new ApiAlreadyExistException(GlobalLabelCodes.ProfileAlreadyExist);
            }

            UsrProfileDao profile = new()
            {
                Name = createForm.Name
            };

            List<UsrProfileFunctionDao> profileFunctions = new();
            foreach (int functionId in createForm.FunctionIds)
            {
                UsrFunctionDao function = await this._dbContext.Functions.FirstOrDefaultAsync(f => f.Id == functionId);
                if (function == null)
                {
                    this._logger.LogInformation("Try to associate unknown function with {Function}", functionId);
                    throw new ApiNotFoundException(GlobalLabelCodes.FunctionNotFound);
                }

                profileFunctions.Add(new UsrProfileFunctionDao
                {
                    Function = function,
                    Profile = profile
                });
            }

            profile.ProfileFunctions = profileFunctions;
            await this._dbContext.Profiles.AddAsync(profile);
            await this._dbContext.SaveChangesAsync();

            ProfileEntity result = this._mapper.Map<ProfileEntity, UsrProfileDao>(profile);
            result.Functions =
                profileFunctions.Select(pf => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Update a specific profile
        /// </summary>
        /// <param name="id">Profile id</param>
        /// <param name="updateForm">Profile information updated</param>
        /// <exception cref="ApiNotFoundException"></exception>
        /// <exception cref="ApiException"></exception>
        /// <returns>Profile updated</returns>
        public async Task<ProfileEntity> UpdateOne(int id, ProfileUpdateForm updateForm)
        {
            UsrProfileDao profile = await this._dbContext.Profiles.FindAsync(id);
            if (profile == null)
            {
                this._logger.LogInformation("Profile not found");
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);
            }

            if (profile.ReadOnly)
            {
                this._logger.LogInformation("Profile is in read-only mode");
                throw new ApiException(GlobalLabelCodes.ProfileReadOnly);
            }

            // Get current functions of the profile
            IQueryable<UsrProfileFunctionDao> profileFunctions =
                this._dbContext.ProfileFUnctions.Where(pf => pf.ProfileId == id);
            foreach (int functionId in updateForm.FunctionIds)
            {
                UsrFunctionDao function = await this._dbContext.Functions.FirstOrDefaultAsync(f => f.Id == functionId);
                if (function == null)
                {
                    this._logger.LogInformation("Try to associate unknown function with {Function}", functionId);
                    throw new ApiNotFoundException(GlobalLabelCodes.FunctionNotFound);
                }

                // Add function to the profile if not exist
                if (!profileFunctions.Any(pf => pf.FunctionId == functionId))
                    await this._dbContext.ProfileFUnctions.AddAsync(new UsrProfileFunctionDao
                        {ProfileId = id, FunctionId = functionId});
            }

            // Remove a function from the profile if not given
            foreach (UsrProfileFunctionDao profileFunction in profileFunctions)
                if (updateForm.FunctionIds.All(r => r != profileFunction.FunctionId))
                    this._dbContext.ProfileFUnctions.Remove(profileFunction);

            await this._dbContext.SaveChangesAsync();

            ProfileEntity result = this._mapper.Map<ProfileEntity, UsrProfileDao>(profile);
            result.Functions =
                profileFunctions.Select(pf => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Delete a specific profile
        /// </summary>
        /// <param name="id">Profile id</param>
        /// <exception cref="ApiNotFoundException"></exception>
        /// <exception cref="ApiException"></exception>
        /// <returns></returns>
        public async Task DeleteOne(int id)
        {
            UsrProfileDao profile = await this._dbContext.Profiles.FindAsync(id);
            if (profile == null)
            {
                this._logger.LogInformation("Profile not found");
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);
            }

            if (profile.ReadOnly)
            {
                this._logger.LogInformation("Profile is in read-only mode");
                throw new ApiException(GlobalLabelCodes.ProfileReadOnly);
            }

            this._dbContext.Profiles.Remove(profile);
            await this._dbContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Apply search filter to the profile DbSet
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Filtered DbSet</returns>
        private IQueryable<UsrProfileDao> ProfileFiltered(ProfileCriteria criteria)
        {
            IQueryable<UsrProfileDao> filtered = this._dbContext.Profiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                filtered = filtered.Where(u => u.Name.ToUpper().Contains(criteria.Name.ToUpper()));

            filtered = criteria.SortField.ToLower() switch
            {
                "name" => criteria.SortDir == SortDir.Asc
                    ? filtered.OrderBy(o => o.Name)
                    : filtered.OrderByDescending(o => o.Name),
                _ => filtered
            };
            return filtered;
        }
    }
}