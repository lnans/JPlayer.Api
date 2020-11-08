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