using JPlayer.Lib.Contract;

namespace JPlayer.Data.Dto.Profile
{
    /// <summary>
    ///     Search filter for profiles
    /// </summary>
    public class ProfileCriteria : ApiCriteria
    {
        public ProfileCriteria() : base(int.MaxValue - 1, 0, SortDir.Asc, nameof(Name))
        {
        }

        /// <summary>
        ///     Profile name
        /// </summary>
        public string Name { get; set; }
    }
}