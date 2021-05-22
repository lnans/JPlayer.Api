using System;
using JPlayer.Data.Dto.SystemInfo;

namespace JPlayer.Business.SystemInfo.Provider
{
    internal class LinuxProvider : IProvider
    {
        public SystemInfoCollectionItem GetSystemInfo() => throw new NotImplementedException();
    }
}