using System;

namespace JPlayer.Data.Dao.Model
{
    public class SysInfoHistory
    {
        public long Id { get; set; }

        public long UnixTimeSeconds { get; set; }

        public long Total { get; set; }

        public long Used { get; set; }

        public long Free { get; set; }
    }
}