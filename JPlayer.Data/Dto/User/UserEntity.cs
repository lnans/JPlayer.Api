using System;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User entity
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        ///     User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     User creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Last connection of the user
        /// </summary>
        public DateTime LastConnectionDate { get; set; }
    }
}