using Microsoft.Extensions.Logging;
using Podclave.Core.Tasks;
using Podclave.Core.Configuration;

namespace Podclave.Core.Handlers;

public class EpisodeDownloadHandler : IHandler
{
    private readonly IConfigLoader _configLoader;

    private readonly ILogger<EpisodeDownloadHandler> _logger;

    public EpisodeDownloadHandler(
        IConfigLoader configLoader,
        ILogger<EpisodeDownloadHandler> logger)
    {
        _configLoader = configLoader;
        _logger = logger;
    }

    public async Task Handle(WorkTask t)
    {
        var episodeDownloadTask = t as EpisodeDownloadTask;
        if (episodeDownloadTask == null)
        {
            _logger.LogError("Attempted to handle non-valid task type. Aborting");
            return;
        }

        PodclaveConfig? config = _configLoader.Load();
        if (config == null)
        {
            _logger.LogError("Config could not be found. Aborting task to download {name} from feed {feed}.",
                 episodeDownloadTask.Episode.Title,
                 episodeDownloadTask.Episode.FeedName);
            return;
        }

        var podcastConfig = config.Podcasts
            .Where(p => p.Name == episodeDownloadTask.Episode.FeedName)
            .FirstOrDefault();

        if (podcastConfig == null)
        {
            _logger.LogError("Could not find a podcast config that matches episode to download. Aborting task to download {name} from feed {feed}",
                 episodeDownloadTask.Episode.Title,
                 episodeDownloadTask.Episode.FeedName);
            return;
        }

        var path = $"{config.BaseDirectory}/{podcastConfig.DirectoryName}";

        if (!Directory.Exists(path))
        {
            _logger.LogInformation("Directory for {name} doesn't exist. Creating {path}",
                podcastConfig.Name, path);
            
            Directory.CreateDirectory(path);
        }

        _logger.LogInformation("Attempting to download episode {epName} ({filename}) for {name}.",
            episodeDownloadTask.Episode.Title,
            episodeDownloadTask.Episode.Filename,
            podcastConfig.Name);

        byte[]? responseData = null;
        using (HttpClient client = new HttpClient())
        {
            var link = episodeDownloadTask.Episode.MediaLink;
            try 
            {
                HttpResponseMessage response = await client.GetAsync(link);

                if (response.IsSuccessStatusCode)
                {
                    responseData = await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured try to download episode from {link}. Error: {e.Message}");
                throw;
            }

        }
        
        if (responseData == null)
        {
            _logger.LogError("Error in downloading epsidoe. Cannot save. Aborting.");
            return;
        }

        _logger.LogInformation("Download successful.");

        using (FileStream fs = new FileStream($"{path}/{episodeDownloadTask.Episode.Filename}", FileMode.Create))
        {
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(responseData);
            bw.Close();
        }
    }
}
