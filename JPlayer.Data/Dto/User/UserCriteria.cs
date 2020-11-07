using JPlayer.Lib.Contract;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User search filter
    /// </summary>
    public class UserCriteria : ApiCriteria
    {
        public UserCriteria() : base(int.MaxValue - 1, 0, SortDir.Asc, nameof(Login))
        {
            this.Login = string.Empty;
        }

        /// <summary>
        ///     Search by login
        /// </summary>
        public string Login { get; set; }
    }
}