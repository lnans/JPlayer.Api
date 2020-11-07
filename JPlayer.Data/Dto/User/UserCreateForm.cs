﻿using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     Create user form
    /// </summary>
    public class UserCreateForm
    {
        /// <summary>
        ///     User login
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Login { get; set; }
    }
}