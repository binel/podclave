
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Podclave.Core;
using Podclave.Core.Configuration;
using Podclave.Core.Models;
using Podclave.Core.Tasks;
using Podclave.Core.Handlers;

namespace Podclave.Core;

public class PodclaveService : BackgroundService
{
    private readonly ITaskRespository _taskRespository;

    private readonly FetchFeedHandler _fetchFeedHandler;

    private readonly InitializationHandler _initializationHandler;

    private readonly EpisodeDownloadHandler _episodeDownloadHandler;

    private readonly ILogger<PodclaveService> _logger;

    public PodclaveService(
        ITaskRespository taskRespository,
        FetchFeedHandler fetchFeedHandler,
        InitializationHandler initializationHandler,
        EpisodeDownloadHandler episodeDownloadHandler,
        ILogger<PodclaveService> logger)
    {
        _taskRespository = taskRespository;
        _fetchFeedHandler = fetchFeedHandler;
        _initializationHandler = initializationHandler;
        _episodeDownloadHandler = episodeDownloadHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _taskRespository.AddTask(new InitializationTask());

        _logger.LogInformation("Starting work loop...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextTask = _taskRespository.PopNextTask();

            if (nextTask == null)
            {
                Thread.Sleep(5000);
                continue;
            }

            if (nextTask is InitializationTask)
            {
                await _initializationHandler.Handle(nextTask);
            }
            else if (nextTask is FetchFeedTask)
            {
                await _fetchFeedHandler.Handle(nextTask);
            }
            else if (nextTask is EpisodeDownloadTask)
            {
                await _episodeDownloadHandler.Handle(nextTask);
                
            }
        }
        _logger.LogInformation("Shutting down...");
    }
}

