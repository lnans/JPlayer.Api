using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.Credentials
{
    /// <summary>
    ///     Update credentials form
    /// </summary>
    public class CredentialsUpdateForm
    {
        /// <summary>
        ///     Current password
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string CurrentPassword { get; set; }

        /// <summary>
        ///     New password
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string NewPassword { get; set; }
    }
}