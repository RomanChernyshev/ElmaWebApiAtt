using System;
using System.IO;
using ElmaWebApi.App.Core.Configurations;
using ElmaWebApi.App.Models;

namespace ElmaWebApi.App.Core
{
    /// <summary>
    /// Initiates listening for changes
    /// </summary>
    internal sealed class BigWatcher
    {
        #region private members

        private readonly AppConfiguration configuration;

        #endregion

        public BigWatcher(AppConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Start listening
        /// </summary>
        public void Watch()
        {
            foreach (var path in configuration.FileWatchPathes)
            {
                WatchersStorage.AddWatcher(path, AddReport, WatcherChangeTypes.All);
            }
        }

        private void AddReport(object sender, FileSystemEventArgs args)
        {
            ChangesQueue.AddReport(new DirectoryChangeReport
            {
                ChangeTime = DateTime.UtcNow,
                ChangeType = args.ChangeType,
                FullFilePath = args.FullPath
            });
        }
    }
}
