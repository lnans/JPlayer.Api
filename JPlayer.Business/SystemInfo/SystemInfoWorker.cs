using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JPlayer.Business.SystemInfo.Provider;
using JPlayer.Data.Dto.SystemInfo;

// ReSharper disable FunctionNeverReturns

namespace JPlayer.Business.SystemInfo
{
    public sealed class SystemInfoWorker
    {
        // Read system each 2seconds
        private const int ReadTick = 2;
        private const int MaxQueueSize = 30;

        private static readonly Lazy<SystemInfoWorker> Lazy = new(() => new SystemInfoWorker());
        private readonly IProvider _provider;

        private readonly Queue<SystemInfoCollectionItem> _systemInfoQueue;

        private SystemInfoWorker()
        {
            this._provider = OperatingSystem.IsWindows() ? new WindowProvider() : new LinuxProvider();
            this._systemInfoQueue = new Queue<SystemInfoCollectionItem>();
        }

        private bool IsRunning { get; set; }

        public static SystemInfoWorker Instance => Lazy.Value;

        public event EventHandler<SystemInfoEventArgs> SystemInfoEvent;

        /// <summary>
        ///     Start system info worker
        /// </summary>
        public void Start()
        {
            if (this.IsRunning) return;
            this.IsRunning = true;

            Thread readSystemInfo = new(this.ReadSystemInfoThread);

            readSystemInfo.Start();
        }

        /// <summary>
        ///     Get current content of system info queue
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SystemInfoCollectionItem> GetQueue() => this._systemInfoQueue.Reverse();

        private void OnNewSystemInfo(SystemInfoEventArgs systemInfoEventArgs)
        {
            this.SystemInfoEvent?.Invoke(this, systemInfoEventArgs);
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
                    SystemInfoCollectionItem systemInfo = this._provider.GetSystemInfo();
                    systemInfo.Time = unixTimeSeconds * 1000;

                    while (this._systemInfoQueue.Count >= MaxQueueSize)
                        this._systemInfoQueue.Dequeue();

                    this._systemInfoQueue.Enqueue(systemInfo);
                    this.OnNewSystemInfo(new SystemInfoEventArgs {Data = this._systemInfoQueue.Reverse()});
                    previousValidTick = unixTimeSeconds;
                }

                Thread.Sleep(250);
            }
        }
    }
}