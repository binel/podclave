
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Podclave.Core.Configuration;

namespace Podclave.Core.Extensions;

public static class Extensions
{
    public static void AddPodclaveCoreServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDownloader, Downloader>();
        builder.Services.AddSingleton<IFeedFetcher, FeedFetcher>();
        builder.Services.AddSingleton<IConfigLoader, ConfigLoader>();
    }
}