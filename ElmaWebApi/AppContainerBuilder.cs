using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using ElmaWebApi.App.Core;
using ElmaWebApi.App.Core.Configurations;
using ElmaWebApi.App.ExtentionAPI;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ElmaWebApi.App
{
    internal sealed class AppContainerBuilder
    {
        private static ContainerBuilder containerBuilder = new ContainerBuilder();
        private static IContainer container;

        public static IContainer Container { get => container; }

        public static void Build()
        {
            var allAssemblies = Directory.EnumerateFiles($"{Directory.GetCurrentDirectory()}\\Extensions", "*.dll", SearchOption.TopDirectoryOnly)
                         .Select(Assembly.LoadFrom).ToArray();

            var appConfig = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json", optional: true, reloadOnChange: true)
                .Build()
                .Get<AppConfiguration>();
            
            containerBuilder.RegisterInstance(appConfig)
                .As<AppConfiguration>()
                .SingleInstance();

            containerBuilder.RegisterInstance(appConfig.ExternalApi)
                .As<IEnumerable<IExternalApiDescriptor>>()
                .SingleInstance();

            var services = new ServiceCollection();
            services.AddHttpClient();

            var manager = allAssemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => !t.IsInterface
                    && t.GetInterfaces()
                        .Any(i => i == typeof(ICustomHttpClient))
                );

            containerBuilder.RegisterType(manager)
                .AsSelf()
                .WithParameter((p, ctx) => p.ParameterType == typeof(HttpClient),
                    (p, ctx) => ctx.Resolve<IHttpClientFactory>().CreateClient())
                .WithParameter((p, ctx) => p.ParameterType == typeof(string),
                    (p, ctx) => appConfig.ExternalApi.FirstOrDefault(url => url.ClientIdentifier == manager.Name)?.Url)
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => !t.IsInterface && t.GetInterfaces().Any(i => i == typeof(IService)))
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => !t.IsInterface && t.GetInterfaces().Any(i => i == typeof(IFileWatchHandler)))
                .AsImplementedInterfaces()
                .SingleInstance();

            containerBuilder.RegisterType<BigWatcher>()
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterType<Scheduler>()
                .AsSelf()
                .SingleInstance();

            containerBuilder.Populate(services);

            container = containerBuilder.Build();
        }
    }
}
