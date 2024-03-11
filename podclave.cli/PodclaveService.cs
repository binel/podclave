
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Podclave.Core;
using Podclave.Core.Configuration;

namespace Podclave.Cli;

public class PodclaveService : BackgroundService
{
    private readonly IFeedFetcher _feedFetcher;
    private readonly IConfigLoader _configLoader;
    private readonly ILogger<PodclaveService> _logger;

    public PodclaveService(
        IFeedFetcher feedFetcher,
        IConfigLoader configLoader,
        ILogger<PodclaveService> logger)
    {
        _feedFetcher = feedFetcher;
        _configLoader = configLoader;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = _configLoader.Load();

        foreach(var podcast in config.Podcasts)
        {
            var feed = await _feedFetcher.Fetch(podcast.FeedUrl);

            if (podcast.DryRun)
            {
                _logger.LogInformation("Dry run set for {podcast}", feed.Name);
            }
        }
    }
}

