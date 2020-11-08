using System.Collections.Generic;
using JPlayer.Data.Dto.Function;

namespace JPlayer.Data.Dto.Profile
{
    /// <summary>
    ///     Profile entity details
    /// </summary>
    public class ProfileEntity
    {
        /// <summary>
        ///     Profile id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Profile name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Functions assigned to this profile
        /// </summary>
        public IEnumerable<FunctionCollectionItem> Functions { get; set; }

        /// <summary>
        ///     Determinate if the profile is in read only mode
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}