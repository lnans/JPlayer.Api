using System;
using System.Collections.Generic;
using JPlayer.Lib.Process;

namespace JPlayer.Business.SystemInfo.Provider
{
    internal class WindowProvider : IProvider
    {
        private readonly Dictionary<string, string> _commands = new()
        {
            {"memory", "wmic OS get FreePhysicalMemory,TotalVisibleMemorySize /Value"}
        };

        public Data.Dto.SystemInfo.SystemInfo GetMemoryInfo()
        {
            string cmdResult = ProcessCommand.WindowsRun(this._commands["memory"]);

            string[] outPutLines = cmdResult.Split("\r\n");

            Data.Dto.SystemInfo.SystemInfo systemInfo = new()
            {
                Free = long.Parse(outPutLines[0].Split('=', StringSplitOptions.RemoveEmptyEntries)[1]),
                Total = long.Parse(outPutLines[1].Split('=', StringSplitOptions.RemoveEmptyEntries)[1])
            };

            systemInfo.Used = systemInfo.Total - systemInfo.Free;
            return systemInfo;
        }
    }
}