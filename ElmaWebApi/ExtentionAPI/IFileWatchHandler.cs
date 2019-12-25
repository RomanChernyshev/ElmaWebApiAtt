using System;
using System.IO;
using System.Threading.Tasks;
using ElmaWebApi.App.Models;

namespace ElmaWebApi.App.ExtentionAPI
{
    /// <summary>
    /// This interface is used to implement handler extensions directory change events
    /// </summary>
    public interface IFileWatchHandler
    {
        WatcherChangeTypes[] ChangeTypes { get; }

        Task Execute(FileDescription fileDescription);

        Task CatchException(Exception exception, FileDescription fileDescription);
    }
}
