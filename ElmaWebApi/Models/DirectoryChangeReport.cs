using System;
using System.IO;

namespace ElmaWebApi.App.Models
{
    internal class DirectoryChangeReport
    {
        public WatcherChangeTypes ChangeType { get; set; }

        public string FullFilePath { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
