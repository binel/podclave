using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Podclave.Core.Handlers;
using Podclave.Core.Tasks;
using Podclave.Core;
using Podclave.Core.Extensions;

namespace Podclave.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.AddPodclaveCoreServices();

            builder.Services.AddLogging(builder => 
                builder.AddSimpleConsole(o => {
                    o.SingleLine = true;
                    o.TimestampFormat = "dd/MM/yy hh:mm:ss ";
                })
            );

            builder.Services.AddHostedService<PodclaveService>();

            builder.Services.AddSingleton<ITaskRespository, TaskRespository>();
            builder.Services.AddSingleton<FetchFeedHandler>();
            builder.Services.AddSingleton<InitializationHandler>();
            builder.Services.AddSingleton<EpisodeDownloadHandler>();

            using IHost host = builder.Build();

            await host.RunAsync();
        }
    }
}
