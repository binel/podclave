using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            builder.Services.AddLogging(builder => builder.AddConsole());

            builder.Services.AddHostedService<PodclaveService>();

            using IHost host = builder.Build();

            await host.RunAsync();
        }
    }
}