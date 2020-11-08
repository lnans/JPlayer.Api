using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.Function;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Mapper;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Business.Services
{
    public class FunctionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ObjectMapper _mapper;

        public FunctionService(ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        /// <summary>
        ///     Return functions in database
        ///     can be filtred with a criteria object
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FunctionCollectionItem>> GetMany(FunctionCriteria criteria)
        {
            criteria ??= new FunctionCriteria();
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
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<int> GetCount(FunctionCriteria criteria)
        {
            criteria ??= new FunctionCriteria();
            return await this.FunctionsFiltered(criteria).CountAsync();
        }

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