using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPlayer.Data.Dto.Profile
{
    /// <summary>
    ///     Form for create profile
    /// </summary>
    public class ProfileCreateForm
    {
        /// <summary>
        ///     Profile name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        ///     Functions list
        /// </summary>
        [Required]
        public IEnumerable<int> FunctionIds { get; set; }
    }
}