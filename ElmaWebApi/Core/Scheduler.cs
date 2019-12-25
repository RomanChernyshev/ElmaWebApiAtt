using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElmaWebApi.App.Core.Configurations;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.App.Models;

namespace ElmaWebApi.App.Core
{
    /// <summary>
    /// At the specified interval, check the queue of changes and call the handlers
    /// </summary>
    internal sealed class Scheduler
    {
        private readonly IEnumerable<IFileWatchHandler> fileWatchHandlers;
        private readonly AppConfiguration configuration;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fileWatchHandlers">All registered handlers</param>
        public Scheduler(IEnumerable<IFileWatchHandler> fileWatchHandlers, AppConfiguration configuration)
        {
            this.fileWatchHandlers = fileWatchHandlers;
            this.configuration = configuration;
        }

        public void Start()
        {
            while (true)
            {
                for (var i = 0; i < ChangesQueue.Count; i++)
                {
                    ExecuteHandlers().Wait();
                }
                Thread.Sleep(500);
            }
        }

        private async Task ExecuteHandlers()
        {
            var report = ChangesQueue.GetReport();
            if (report == null)
            {
                return;
            }

            foreach (var handler in fileWatchHandlers)
            {
                if (!handler.ChangeTypes.Contains(report.ChangeType))
                {
                    continue;
                }

                var fileDescription = new FileDescription
                {
                    FullPath = report.FullFilePath
                };

                try
                {
                    await handler.Execute(fileDescription);
                }
                catch(Exception ex)
                {
                    await handler.CatchException(ex, fileDescription);
                    continue;
                }
            }
        }
    }
}
