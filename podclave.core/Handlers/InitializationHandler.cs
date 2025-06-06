
using Microsoft.Extensions.Logging;
using Podclave.Core.Tasks;
using Podclave.Core.Configuration;

namespace Podclave.Core.Handlers;

public class InitializationHandler : IHandler
{
    private readonly ILogger<InitializationHandler> _logger;
    private readonly IConfigLoader _configLoader;

    private readonly ITaskRespository _taskRepository;

    public InitializationHandler(ILogger<InitializationHandler> logger,
        IConfigLoader configLoader,
        ITaskRespository taskRespository)
    {
        _logger = logger;
        _configLoader = configLoader;
        _taskRepository = taskRespository;
    }

    public Task Handle(WorkTask t)
    {
        PodclaveConfig? config = _configLoader.Load();
        if (config == null)
        {
            _logger.LogError("Config could not be found. Aborting initalization.");
            return Task.CompletedTask;
        }

        foreach(var podcast in config.Podcasts)
        {
            var priorTask = _taskRepository.GetLastTask<FetchFeedTask>();

            var fetchTask = new FetchFeedTask
            {
                Podcast = podcast,
                DoNotWorkBefore = priorTask != null ? priorTask.DoNotWorkBefore.AddSeconds(config.FeedFetchCooldownSeconds) : DateTime.UtcNow,
            };
            _logger.LogInformation("Created task to fetch feed {name} not before {time}", podcast.Name, fetchTask.DoNotWorkBefore);
            _taskRepository.AddTask(fetchTask);
        }

        return Task.CompletedTask;
    }
}
