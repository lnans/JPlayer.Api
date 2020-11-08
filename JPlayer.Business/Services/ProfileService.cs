using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Function;
using JPlayer.Data.Dto.Profile;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Exception;
using JPlayer.Lib.Mapper;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Business.Services
{
    public class ProfileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ObjectMapper _mapper;

        public ProfileService(ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        /// <summary>
        ///     Get profiles in database
        ///     Can be filtered with a criteria object
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProfileCollectionItem>> GetMany(ProfileCriteria criteria)
        {
            criteria ??= new ProfileCriteria();
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
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<int> GetCount(ProfileCriteria criteria)
        {
            criteria ??= new ProfileCriteria();
            return await this.ProfileFiltered(criteria).CountAsync();
        }

        /// <summary>
        ///     Get a specific profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProfileEntity> GetOne(int id)
        {
            UsrProfileDao profile = await this._dbContext.Profiles
                .Include(p => p.ProfileFunctions)
                .ThenInclude(pf => pf.Function)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);

            ProfileEntity result = this._mapper.Map<ProfileEntity, UsrProfileDao>(profile);
            result.Functions = profile.ProfileFunctions.Select(pf => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Create a profile
        /// </summary>
        /// <param name="createForm"></param>
        /// <returns></returns>
        public async Task<ProfileEntity> CreateOne(ProfileCreateForm createForm)
        {
            // Check if profile already exist
            if (await this._dbContext.Profiles.AnyAsync(p => p.Name.ToLower() == createForm.Name.ToLower()))
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileAlreadyExist);

            UsrProfileDao profile = new UsrProfileDao
            {
                Name = createForm.Name
            };

            List<UsrProfileFunctionDao> profileFunctions = new List<UsrProfileFunctionDao>();
            foreach (int functionId in createForm.FunctionIds)
            {
                UsrFunctionDao function = await this._dbContext.Functions.FirstOrDefaultAsync(f => f.Id == functionId);
                if (function == null)
                    throw new ApiNotFoundException(GlobalLabelCodes.FunctionNotFound);
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
            result.Functions = profileFunctions.Select(pf => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Update a specific profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateForm"></param>
        /// <returns></returns>
        public async Task<ProfileEntity> UpdateOne(int id, ProfileUpdateForm updateForm)
        {
            UsrProfileDao profile = await this._dbContext.Profiles.FindAsync(id);
            if (profile == null)
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);

            if (profile.ReadOnly)
                throw new ApiException(GlobalLabelCodes.ProfileReadOnly);

            // Get current functions of the profile
            IQueryable<UsrProfileFunctionDao> profileFunctions = this._dbContext.ProfileFUnctions.Where(pf => pf.ProfileId == id);
            foreach (int functionId in updateForm.FunctionIds)
            {
                UsrFunctionDao function = await this._dbContext.Functions.FirstOrDefaultAsync(f => f.Id == functionId);
                if (function == null)
                    throw new ApiNotFoundException(GlobalLabelCodes.FunctionNotFound);

                // Add function to the profile if not exist
                if (!profileFunctions.Any(pf => pf.FunctionId == functionId))
                    await this._dbContext.ProfileFUnctions.AddAsync(new UsrProfileFunctionDao {ProfileId = id, FunctionId = functionId});
            }

            // Remove a function from the profile if not given
            foreach (UsrProfileFunctionDao profileFunction in profileFunctions)
            {
                if (updateForm.FunctionIds.All(r => r != profileFunction.FunctionId))
                    this._dbContext.ProfileFUnctions.Remove(profileFunction);
            }

            await this._dbContext.SaveChangesAsync();

            ProfileEntity result = this._mapper.Map<ProfileEntity, UsrProfileDao>(profile);
            result.Functions = profileFunctions.Select(pf => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(pf.Function));
            return result;
        }

        /// <summary>
        ///     Delete a specific profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteOne(int id)
        {
            UsrProfileDao profile = await this._dbContext.Profiles.FindAsync(id);
            if (profile == null)
                throw new ApiNotFoundException(GlobalLabelCodes.ProfileNotFound);

            if (profile.ReadOnly)
                throw new ApiException(GlobalLabelCodes.ProfileReadOnly);

            this._dbContext.Profiles.Remove(profile);
            await this._dbContext.SaveChangesAsync();
        }

        private IQueryable<UsrProfileDao> ProfileFiltered(ProfileCriteria criteria)
        {
            IQueryable<UsrProfileDao> filtered = this._dbContext.Profiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                filtered = filtered.Where(u => u.Name.ToUpper().Contains(criteria.Name.ToUpper()));

            filtered = criteria.SortField.ToLower() switch
            {
                "name" => criteria.SortDir == SortDir.Asc ? filtered.OrderBy(o => o.Name) : filtered.OrderByDescending(o => o.Name),
                _ => filtered
            };
            return filtered;
        }
    }
}