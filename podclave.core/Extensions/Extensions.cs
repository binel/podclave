
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Podclave.Core.Configuration;
using Podclave.Core.Handlers;
using Podclave.Core;
using Podclave.Core.Tasks;

namespace Podclave.Core.Extensions;

public static class Extensions
{
    public static void AddPodclaveCoreServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IFeedDownloader, FeedDownloader>();
        builder.Services.AddSingleton<IFeedFetcher, FeedFetcher>();
        builder.Services.AddSingleton<IConfigLoader, ConfigLoader>();
        builder.Services.AddSingleton<ITaskRespository, TaskRespository>();
        builder.Services.AddSingleton<FetchFeedHandler>();
        builder.Services.AddSingleton<InitializationHandler>();
        builder.Services.AddSingleton<EpisodeDownloadHandler>();
    }
}
