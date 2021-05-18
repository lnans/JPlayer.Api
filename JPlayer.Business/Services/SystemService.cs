using System;
using System.Collections.Generic;
using System.Linq;
using JPlayer.Business.SystemInfo;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.SystemInfo;
using JPlayer.Lib.Object;
using Microsoft.Extensions.Logging;

namespace JPlayer.Business.Services
{
    public class SystemService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SystemService> _logger;
        private readonly ObjectMapper _mapper;

        public SystemService(ILogger<SystemService> logger, ApplicationDbContext dbContext, ObjectMapper mapper)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        /// <summary>
        ///     Get system info history
        /// </summary>
        /// <param name="ticks">number of ticks to get</param>
        /// <returns></returns>
        public IEnumerable<SystemInfoCollectionItem> GetSystemInfoHistory(int ticks)
        {
            this._logger.LogInformation($"Get {ticks} of system information");
            long secondsTimeSpan = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - ticks * 5;
            List<SystemInfoCollectionItem> result =
                SystemInfoWorker.Instance.GetUnsavedSystemInfos().ToList();

            IQueryable<SysInfoHistory> dbResult = this._dbContext.SystemInfos
                .OrderByDescending(info => info.UnixTimeSeconds)
                .Take(ticks - result.Count);

            foreach (SysInfoHistory sysMemoryInfo in dbResult)
                if (result.All(info => info.Time != sysMemoryInfo.UnixTimeSeconds))
                    result.Add(new SystemInfoCollectionItem
                    {
                        Time = sysMemoryInfo.UnixTimeSeconds,
                        SystemInfo = this._mapper.Map<Data.Dto.SystemInfo.SystemInfo, SysInfoHistory>(sysMemoryInfo)
                    });

            return result.OrderBy(info => info.Time);
        }
    }
}