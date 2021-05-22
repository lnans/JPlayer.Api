using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management;
using JPlayer.Data.Dto.SystemInfo;

namespace JPlayer.Business.SystemInfo.Provider
{
    internal class WindowProvider : IProvider
    {
        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Handle by the interface")]
        public SystemInfoCollectionItem GetSystemInfo()
        {
            // OS and Memory info
            ManagementObjectSearcher osQuery = new("SELECT FreePhysicalMemory,TotalVisibleMemorySize,Caption FROM Win32_OperatingSystem");
            ManagementObject osInfo = osQuery.Get().OfType<ManagementObject>().FirstOrDefault();

            string freeMemory = osInfo?["FreePhysicalMemory"].ToString();
            string totalMemory = osInfo?["TotalVisibleMemorySize"].ToString();
            string osName = osInfo?["Caption"].ToString();

            // CPU info
            ManagementObjectSearcher processorQuery = new("SELECT Name,NumberOfCores,NumberOfLogicalProcessors,LoadPercentage FROM Win32_Processor");
            ManagementObject processorInfo = processorQuery.Get().OfType<ManagementObject>().FirstOrDefault();

            string processorName = processorInfo?["Name"].ToString();
            string processorCores = processorInfo?["NumberOfCores"].ToString();
            string processorThreads = processorInfo?["NumberOfLogicalProcessors"].ToString();
            string processorLoad = processorInfo?["LoadPercentage"].ToString();

            // Disk info
            ManagementObjectSearcher diskQuery = new("SELECT DeviceID,FreeSpace,Size FROM Win32_LogicalDisk WHERE Size IS NOT NULL");
            ManagementObjectCollection diskInfosList = diskQuery.Get();

            List<DiskInfo> diskInfos = new();
            foreach (ManagementBaseObject disk in diskInfosList)
                diskInfos.Add(new DiskInfo
                {
                    MountPoint = disk["DeviceID"].ToString(),
                    Total = long.Parse(disk?["Size"].ToString() ?? "0"),
                    Free = long.Parse(disk?["FreeSpace"].ToString() ?? "0")
                });

            SystemInfoCollectionItem systemInfo = new()
            {
                MemoryInfo = new MemoryInfo
                {
                    Free = long.Parse(freeMemory ?? "0") * 1024,
                    Total = long.Parse(totalMemory ?? "0") * 1024
                },
                CpuInfo = new CpuInfo
                {
                    Name = processorName,
                    CoreCount = int.Parse(processorCores ?? "0"),
                    CpuLoad = int.Parse(processorLoad ?? "0"),
                    ThreadCount = int.Parse(processorThreads ?? "0")
                },
                DiskInfo = diskInfos,
                Name = osName
            };

            return systemInfo;
        }
    }
}