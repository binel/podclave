
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Podclave.Core;
using Podclave.Core.Configuration;
using Podclave.Core.Models;
using Podclave.Cli.Tasks;
using Podclave.Cli.Handlers;

namespace Podclave.Cli;

public class PodclaveService : BackgroundService
{
    private readonly IConfigLoader _configLoader;

    private readonly IEpisodeDownloader _episodeDownloader;

    private readonly ITaskRespository _taskRespository;

    private readonly FetchFeedHandler _fetchFeedHandler;

    private readonly ILogger<PodclaveService> _logger;

    public PodclaveService(
        IConfigLoader configLoader,
        IEpisodeDownloader episodeDownloader,
        ITaskRespository taskRespository,
        FetchFeedHandler fetchFeedHandler,
        ILogger<PodclaveService> logger)
    {
        _configLoader = configLoader;
        _episodeDownloader = episodeDownloader;
        _taskRespository = taskRespository;
        _fetchFeedHandler = fetchFeedHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = _configLoader.Load();

        _logger.LogInformation("Start up. Creating work tasks to fetch all configured feeds.");

        foreach(var podcast in config.Podcasts)
        {
            var priorTask = _taskRespository.GetLastTask<FetchFeedTask>();

            var fetchTask = new FetchFeedTask
            {
                Podcast = podcast,
                DoNotWorkBefore = priorTask != null ? priorTask.DoNotWorkBefore.AddSeconds(config.FeedFetchCooldownSeconds) : DateTime.UtcNow,
            };
            _logger.LogInformation("Created task to fetch feed {name} not before {time}", podcast.Name, fetchTask.DoNotWorkBefore);
            _taskRespository.AddTask(fetchTask);
        }

        _logger.LogInformation("Starting work loop...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextTask = _taskRespository.PopNextTask();

            if (nextTask == null)
            {
                _logger.LogInformation("no task, sleeping");
                Thread.Sleep(5000);
                continue;
            }

            if (nextTask is FetchFeedTask fetchFeedTask)
            {
                await _fetchFeedHandler.Handle(nextTask);
            }
            if (nextTask is EpisodeDownloadTask episodeDownloadTask)
            {
                await _episodeDownloader.Download(episodeDownloadTask.Episode);
            }
        }
    }


}

