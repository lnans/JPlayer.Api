using System.Collections.Generic;

namespace JPlayer.Data.Dto.SystemInfo
{
    /// <summary>
    ///     System information for a specific time span
    /// </summary>
    public class SystemInfoCollectionItem
    {
        /// <summary>
        ///     Memory information
        /// </summary>
        public MemoryInfo MemoryInfo { get; set; }

        /// <summary>
        ///     Processor information
        /// </summary>
        public CpuInfo CpuInfo { get; set; }

        /// <summary>
        ///     Disk list
        /// </summary>
        public IEnumerable<DiskInfo> DiskInfo { get; set; }

        /// <summary>
        ///     Current time span
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        ///     Operating system name
        /// </summary>
        public string Name { get; set; }
    }

    public class MemoryInfo
    {
        /// <summary>
        ///     Total memory available
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        ///     Free memory available
        /// </summary>
        public long Free { get; set; }

        /// <summary>
        ///     Used memory
        /// </summary>
        public long Used => this.Total - this.Free;
    }

    public class CpuInfo
    {
        /// <summary>
        ///     CPU average load
        /// </summary>
        public int CpuLoad { get; set; }

        /// <summary>
        ///     CPU Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Number of core
        /// </summary>
        public int CoreCount { get; set; }

        /// <summary>
        ///     Number of thread
        /// </summary>
        public int ThreadCount { get; set; }
    }

    public class DiskInfo
    {
        /// <summary>
        ///     Disk mount point
        /// </summary>
        public string MountPoint { get; set; }

        /// <summary>
        ///     Disk size
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        ///     Disk free space
        /// </summary>
        public long Free { get; set; }

        /// <summary>
        ///     Disk used space
        /// </summary>
        public long Used => this.Total - this.Free;
    }
}