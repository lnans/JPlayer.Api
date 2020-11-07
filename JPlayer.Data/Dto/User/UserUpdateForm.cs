using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.User
{
    /// <summary>
    ///     User update form
    /// </summary>
    public class UserUpdateForm
    {
        /// <summary>
        ///     User deactivation state
        /// </summary>
        [Required]
        public bool Deactivated { get; set; }
    }
}