using System;
using System.Collections.Generic;
using JPlayer.Data.Dto.Profile;

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

        /// <summary>
        ///     Profiles of the user
        /// </summary>
        public IEnumerable<ProfileCollectionItem> Profiles { get; set; }

        /// <summary>
        ///     Determinate if the user is in read only mode
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}