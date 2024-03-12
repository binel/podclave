
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Podclave.Core;
using Podclave.Core.Configuration;
using Podclave.Core.Models;

namespace Podclave.Cli;

public class PodclaveService : BackgroundService
{
    private readonly IFeedFetcher _feedFetcher;
    private readonly IConfigLoader _configLoader;
    private readonly ILogger<PodclaveService> _logger;

    private List<WorkTask> _tasks = new List<WorkTask>();

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

        _logger.LogInformation("Start up. Creating work tasks to fetch all configured feeds.");

        foreach(var podcast in config.Podcasts)
        {
            var priorTask = _tasks.Where(t => t is FetchFeedTask)
                                  .OrderBy(t => t.DoNotWorkBefore)
                                  .FirstOrDefault();

            var fetchTask = new FetchFeedTask
            {
                Podcast = podcast,
                DoNotWorkBefore = priorTask != null ? priorTask.DoNotWorkBefore.AddSeconds(config.FeedFetchCooldownSeconds) : DateTime.UtcNow,
            };
            _logger.LogInformation("Created task to fetch feed {name} not before {time}", podcast.Name, fetchTask.DoNotWorkBefore);
            _tasks.Add(fetchTask);
        }

        _logger.LogInformation("Starting work loop...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextTask = _tasks.Where(t => t.DoNotWorkBefore < DateTime.UtcNow)
                                 .OrderBy(t => t.DoNotWorkBefore)
                                 .FirstOrDefault();

            if (nextTask == null)
            {
                _logger.LogInformation("No available work, sleeping for 60 secs");
                Thread.Sleep(60000);
                continue;
            }

            _tasks.Remove(nextTask);

            if (nextTask is FetchFeedTask fetchFeedTask)
            {
                _logger.LogInformation("Fetching new episodes for {name}", fetchFeedTask.Podcast.Name);
                var epList = await GetDownloadableEpisodesForFeed(fetchFeedTask.Podcast);
                _logger.LogInformation("{count} episodes added to download list for {name}", epList.Count, fetchFeedTask.Podcast.Name);
                var fetchTask = new FetchFeedTask
                {
                    Podcast = fetchFeedTask.Podcast,
                    DoNotWorkBefore = DateTime.UtcNow.AddHours(config.FeedFetchIntervalHours),
                };
                _logger.LogInformation("Created task to fetch feed {name} not before {time}", fetchFeedTask.Podcast.Name, fetchTask.DoNotWorkBefore);
                _tasks.Add(fetchTask);                
            }
        }
    }

    private async Task<List<Episode>> GetDownloadableEpisodesForFeed(PodcastConfig podcast)
    {
        var downloadListForFeed = new List<Episode>();        
        if (podcast.Ignore)
        {
            _logger.LogInformation("{name} is set to ignore. Skipping...", podcast.Name);
            return downloadListForFeed;
        }

        var feed = await _feedFetcher.Fetch(podcast.FeedUrl);

        foreach (var ep in feed.Episodes)
        {
            if (ep.PublishedAt < podcast.IgnoreEpisodesBefore)
            {
                continue;
            }
            
            downloadListForFeed.Add(ep);
        }

        if (podcast.DryRun)
        {
            _logger.LogInformation("Dry run set for {podcast}. {num} episodes were discovered but will not be downloaded", feed.Name, downloadListForFeed.Count);
            return new List<Episode>();
        }

        return downloadListForFeed;
    }
}

