using JPlayer.Data.Dto.SystemInfo;

namespace JPlayer.Business.SystemInfo
{
    /// <summary>
    ///     Provide system information
    /// </summary>
    internal interface IProvider
    {
        /// <summary>
        ///     Get memory information
        /// </summary>
        /// <returns></returns>
        SystemInfoCollectionItem GetSystemInfo();
    }
}