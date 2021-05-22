using System;
using System.Collections.Generic;

namespace JPlayer.Data.Dto.SystemInfo
{
    public class SystemInfoEventArgs : EventArgs
    {
        public IEnumerable<SystemInfoCollectionItem> Data { get; set; }
    }
}