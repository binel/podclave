
using Microsoft.Extensions.Logging;
using Podclave.Cli.Tasks;
using Podclave.Core;
using Podclave.Core.Models;
using Podclave.Core.Configuration;
using Podclave.Core.Exceptions;

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
            var podcastConfig = config.Podcasts
                .Where(p => p.Name == fetchFeedTask.Podcast.Name)
                .FirstOrDefault();

            // Check that we don't already have a task queued to download this episode 
            var tasks = _taskRepository.GetAllTasks();
            var found = tasks.Where(t => t is EpisodeDownloadTask 
                && ((EpisodeDownloadTask)t).Episode.PublishedAt == episode.PublishedAt).Any();
            if (found)
            {
                continue;
            }

            // Check that the episode isn't already downloaded 
            var path = $"{config.BaseDirectory}/{podcastConfig.DirectoryName}/{episode.Filename}";
            if (File.Exists(path))
            {
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
        Feed? feed;
        try {
            feed = await _feedFetcher.Fetch(podcast.FeedUrl);
        }
        catch (FeedParseException ex)
        {
            _logger.LogWarning($"Unable to parse feed for {podcast.Name}! Skipping. Error: {ex.Message}");
            return downloadListForFeed;
        }

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