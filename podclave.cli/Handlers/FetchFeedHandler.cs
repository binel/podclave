
using Microsoft.Extensions.Logging;
using Podclave.Cli.Tasks;
using Podclave.Core;
using Podclave.Core.Models;
using Podclave.Core.Configuration;

namespace Podclave.Cli.Handlers;

public class FetchFeedHandler : IHandler
{
    private readonly ILogger<FetchFeedHandler> _logger;
    private readonly ITaskRespository _taskRepository;
    private readonly IConfigLoader _configLoader;
    private readonly IFeedFetcher _feedFetcher;

    public FetchFeedHandler(
        ILogger<FetchFeedHandler> logger,
        ITaskRespository taskRespository,
        IConfigLoader configLoader,
        IFeedFetcher feedFetcher
    )
    {
        _logger = logger;
        _taskRepository = taskRespository;
        _configLoader = configLoader;
        _feedFetcher = feedFetcher;
    }

    public async Task Handle(WorkTask t)
    {
        var config = _configLoader.Load();
        var fetchFeedTask = t as FetchFeedTask;

        _logger.LogInformation("Fetching new episodes for {name}", fetchFeedTask.Podcast.Name);
        var epList = await GetDownloadableEpisodesForFeed(fetchFeedTask.Podcast);
        int addedEpCount = 0;
        foreach (var episode in epList)
        {
            var priorTask = _taskRepository.GetLastTask<EpisodeDownloadTask>();

            if (priorTask == null)
            {
                _logger.LogInformation("Prior task was null, so next ep download task will happen immediately");
            }
            else 
            {
                _logger.LogInformation("Prior task has download time of {time}", priorTask.DoNotWorkBefore);
            }

            string filename = "E_" + episode.PublishedAt.ToString("yyyy_MM_dd") + ".mp3";
            var podcastConfig = config.Podcasts
                .Where(p => p.Name == fetchFeedTask.Podcast.Name)
                .FirstOrDefault();
            var path = $"{config.BaseDirectory}/{podcastConfig.DirectoryName}/{filename}";

            if (File.Exists(path))
            {
                _logger.LogInformation($"{podcastConfig.DirectoryName}/{filename} already downloaded.");
                continue;
            }

            var epDownloadTask = new EpisodeDownloadTask
            {
                Episode = episode,
                DoNotWorkBefore = priorTask != null ? priorTask.DoNotWorkBefore.AddSeconds(config.RequestDelayBaseSeconds) : DateTime.UtcNow
            };
            _logger.LogInformation("Created task to download episode {title} not before {time}", epDownloadTask.Episode.Title, epDownloadTask.DoNotWorkBefore);
            _taskRepository.AddTask(epDownloadTask);
            addedEpCount++;
        }
        
        _logger.LogInformation("{count} episode download tasks added for {name}", addedEpCount, fetchFeedTask.Podcast.Name);
        var fetchTask = new FetchFeedTask
        {
            Podcast = fetchFeedTask.Podcast,
            DoNotWorkBefore = DateTime.UtcNow.AddHours(config.FeedFetchIntervalHours),
        };
        _logger.LogInformation("Created task to fetch feed {name} not before {time}", fetchFeedTask.Podcast.Name, fetchTask.DoNotWorkBefore);
        _taskRepository.AddTask(fetchTask);
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

            ep.FeedName = podcast.Name;
            
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