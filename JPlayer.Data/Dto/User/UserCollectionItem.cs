using System;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User item in a list
    /// </summary>
    public class UserCollectionItem
    {
        /// <summary>
        ///     User id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     User deactivation state
        /// </summary>
        public bool Deactivated { get; set; }

        /// <summary>
        ///     User creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}