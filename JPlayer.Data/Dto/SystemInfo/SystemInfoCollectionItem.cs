using System;

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
        public SystemInfo SystemInfo { get; set; }

        /// <summary>
        ///     Current time span
        /// </summary>
        public long Time { get; set; }
    }

    public class SystemInfo
    {
        /// <summary>
        ///     Memory info identifier
        /// </summary>
        public long Id { get; set; }

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
        public long Used { get; set; }
    }
}