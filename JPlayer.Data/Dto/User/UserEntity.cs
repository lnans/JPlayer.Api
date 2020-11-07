using System;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User entity
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        ///     User id
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        ///     Last connection of the user
        /// </summary>
        public DateTime? LastConnectionDate { get; set; }
    }
}