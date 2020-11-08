using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User update form
    /// </summary>
    public class UserUpdateForm
    {
        /// <summary>
        ///     Profiles of the user
        /// </summary>
        [Required]
        public IEnumerable<int> Profiles { get; set; }
    }
}