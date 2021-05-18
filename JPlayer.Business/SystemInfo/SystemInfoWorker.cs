using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JPlayer.Business.SystemInfo.Provider;
using JPlayer.Data.Dao;
using JPlayer.Data.Dao.Model;
using JPlayer.Data.Dto.SystemInfo;
using JPlayer.Lib.Extension;
using JPlayer.Lib.Object;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable FunctionNeverReturns

namespace JPlayer.Business.SystemInfo
{
    public sealed class SystemInfoWorker
    {
        // Read system each 5seconds
        // Save in database each minutes
        private const int ReadTick = 5;
        private const int SaveTick = 60;

        private static readonly Lazy<SystemInfoWorker> Lazy = new(() => new SystemInfoWorker());

        private readonly ConcurrentQueue<SystemInfoCollectionItem> _freshQueue;
        private readonly ObjectMapper _mapper;
        private readonly IProvider _provider;
        private IServiceScopeFactory _serviceScopeFactory;
        private ConcurrentQueue<SystemInfoCollectionItem> _toDbQueue;

        private SystemInfoWorker()
        {
            this._mapper = new ObjectMapper();
            this._provider = new WindowProvider();
            this._freshQueue = new ConcurrentQueue<SystemInfoCollectionItem>();
            this._toDbQueue = new ConcurrentQueue<SystemInfoCollectionItem>();
        }

        private bool IsRunning { get; set; }

        public static SystemInfoWorker Instance => Lazy.Value;

        /// <summary>
        ///     Start system info worker
        ///     And save info in data base
        /// </summary>
        public void Start(IServiceScopeFactory serviceScopeFactory)
        {
            if (this.IsRunning) return;
            this._serviceScopeFactory = serviceScopeFactory;
            this.IsRunning = true;

            Thread readSystemInfo = new(this.ReadSystemInfoThread);
            Thread saveSystemInfo = new(this.SaveSystemInfoThread);

            readSystemInfo.Start();
            saveSystemInfo.Start();
        }

        /// <summary>
        ///     Return System info that are not already saved in database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SystemInfoCollectionItem> GetUnsavedSystemInfos()
        {
            return this._toDbQueue.ToArray().Concat(this._freshQueue.ToArray());
        }

        /// <summary>
        ///     Read system information each `ReadTick` seconds
        /// </summary>
        private void ReadSystemInfoThread()
        {
            long previousValidTick = 0;
            while (true)
            {
                long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long unixTimeSeconds = unixTimeMilliseconds / 1000;
                if (unixTimeSeconds % ReadTick == 0 && previousValidTick != unixTimeSeconds)
                {
                    SystemInfoCollectionItem systemInfo = new()
                    {
                        Time = unixTimeSeconds,
                        SystemInfo = this._provider.GetMemoryInfo()
                    };

                    this._freshQueue.Enqueue(systemInfo);
                    previousValidTick = unixTimeSeconds;
                }

                Thread.Sleep(250);
            }
        }

        /// <summary>
        ///     Save system information in database each `SaveTick` seconds
        /// </summary>
        private void SaveSystemInfoThread()
        {
            long previousValidTick = 0;
            while (true)
            {
                long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long unixTimeSeconds = unixTimeMilliseconds / 1000;
                if (unixTimeSeconds % SaveTick == 0 && previousValidTick != unixTimeSeconds)
                {
                    this._toDbQueue = new ConcurrentQueue<SystemInfoCollectionItem>(this._freshQueue.DequeueAll());

                    List<SysInfoHistory> sysMemInfos = new();
                    foreach (SystemInfoCollectionItem systemInfoCollectionItem in this._toDbQueue)
                    {
                        SysInfoHistory dao =
                            this._mapper.Map<SysInfoHistory, Data.Dto.SystemInfo.SystemInfo>(systemInfoCollectionItem.SystemInfo);
                        dao.UnixTimeSeconds = systemInfoCollectionItem.Time;
                        sysMemInfos.Add(dao);
                    }

                    if (sysMemInfos.Any())
                    {
                        using IServiceScope scope = this._serviceScopeFactory.CreateScope();
                        using ApplicationDbContext dbContext =
                            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        dbContext.AddRange(sysMemInfos);
                        dbContext.SaveChanges();
                    }

                    this._toDbQueue.Clear();
                    previousValidTick = unixTimeSeconds;
                }

                Thread.Sleep(250);
            }
        }
    }
}