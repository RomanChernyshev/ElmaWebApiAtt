using System;
using System.Threading.Tasks;
using Autofac;
using ElmaWebApi.App;
using ElmaWebApi.App.Core;

namespace ElmaWebApi
{
    class Program
    {
        /*
         * This application is used to listen to directory change events.
         * Event handling is defined by dynamic plugins extensions.
         * Extension interfaces are defined in the namespace ElmaWebApi.App.ExtentionAPI.
         */
        static void Main(string[] args)
        {
            AppContainerBuilder.Build();
            var scheduler = AppContainerBuilder.Container.Resolve<Scheduler>();
            Task.Factory.StartNew(scheduler.Start, TaskCreationOptions.LongRunning);
            AppContainerBuilder.Container.Resolve<BigWatcher>().Watch();
            Console.ReadLine();
        }
    }
}
