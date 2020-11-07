using System;

namespace JPlayer.Data.Dto.Version
{
    /// <summary>
    ///     Describe an application assembly
    /// </summary>
    public class ApplicationVersion
    {
        public ApplicationVersion(Type applicationType)
        {
            this.Name = applicationType.Assembly.GetName().Name;
            System.Version assemblyVersion = applicationType.Assembly.GetName().Version;
            if (assemblyVersion != null)
                this.Version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        }

        /// <summary>
        ///     Application Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Application Version
        /// </summary>
        public string Version { get; }
    }
}