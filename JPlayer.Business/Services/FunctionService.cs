using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Function;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JPlayer.Business.Services
{
    /// <summary>
    ///     Service for GET functions
    ///     Theses objects are only readable
    /// </summary>
    public class FunctionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<FunctionService> _logger;
        private readonly ObjectMapper _mapper;

        public FunctionService(ILogger<FunctionService> logger, ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        /// <summary>
        ///     Return functions in database
        ///     can be filtred with a criteria object
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Function list</returns>
        public async Task<IEnumerable<FunctionCollectionItem>> GetMany(FunctionCriteria criteria)
        {
            criteria ??= new FunctionCriteria();
            this._logger.LogInformation($"Get filtered function list with search criteria: {criteria.ToJson()}");
            List<UsrFunctionDao> result = await this.FunctionsFiltered(criteria)
                .Skip(criteria.Skip)
                .Take(criteria.Limit)
                .ToListAsync();

            return result.Select(fc => this._mapper.Map<FunctionCollectionItem, UsrFunctionDao>(fc));
        }

        /// <summary>
        ///     Count functions in database
        ///     Can be filered with a criteria object
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Functions count</returns>
        public async Task<int> GetCount(FunctionCriteria criteria)
        {
            criteria ??= new FunctionCriteria();
            return await this.FunctionsFiltered(criteria).CountAsync();
        }

        /// <summary>
        ///     Apply search filter to the Functions DbSet
        /// </summary>
        /// <param name="criteria">Search filter</param>
        /// <returns>Filtered DbSet</returns>
        private IQueryable<UsrFunctionDao> FunctionsFiltered(FunctionCriteria criteria)
        {
            IQueryable<UsrFunctionDao> filtered = this._dbContext.Functions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.FunctionCode))
                filtered = filtered.Where(u => u.FunctionCode.ToUpper().Contains(criteria.FunctionCode.ToUpper()));

            filtered = criteria.SortField.ToLower() switch
            {
                "functioncode" => criteria.SortDir == SortDir.Asc ? filtered.OrderBy(o => o.FunctionCode) : filtered.OrderByDescending(o => o.FunctionCode),
                _ => filtered
            };
            return filtered;
        }
    }
}