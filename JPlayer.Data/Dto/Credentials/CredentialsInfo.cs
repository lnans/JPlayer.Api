using System.Collections.Generic;

namespace JPlayer.Data.Dto.Credentials
{
    /// <summary>
    ///     Entity return on authentication
    /// </summary>
    public class CredentialsInfo
    {
        /// <summary>
        ///     User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     User roles
        /// </summary>
        public IEnumerable<string> Functions { get; set; }
    }
}