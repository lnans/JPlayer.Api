﻿using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.Credentials
{
    /// <summary>
    ///     Credentials form
    /// </summary>
    public class CredentialsForm
    {
        /// <summary>
        ///     User login
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Login { get; set; }

        /// <summary>
        ///     User password
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
    }
}