using System.Collections.Generic;

namespace ElmaWebApi.App.Core.Configurations
{
    /// <summary>
    /// Configuration type
    /// </summary>
    internal sealed class AppConfiguration
    {
        /// <summary>
        /// Path to change listen
        /// </summary>
        public IEnumerable<string> FileWatchPathes { get; set; }

        /// <summary>
        /// External api main settings
        /// </summary>
        public IEnumerable<ExternalApi> ExternalApi { get; set; }
    }
}
