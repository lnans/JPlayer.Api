using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.Profile
{
    /// <summary>
    ///     Form for modify a profile
    /// </summary>
    public class ProfileUpdateForm
    {
        /// <summary>
        ///     Functions assigned to this profile
        /// </summary>
        [Required]
        public IEnumerable<int> FunctionIds { get; set; }
    }
}