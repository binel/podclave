
using Microsoft.Extensions.Logging;
using Podclave.Core.Models;

namespace Podclave.Core;

public interface IEpisodeDownloader
{
    Task<Episode> Download(Episode episode);
}

public class EpisodeDownloader : IEpisodeDownloader
{

    private readonly ILogger<EpisodeDownloader> _logger;

    public EpisodeDownloader(
        ILogger<EpisodeDownloader> logger
    )
    {
        _logger = logger;
    }

    public async Task<Episode> Download(Episode episode)
    {
        _logger.LogInformation("Attempting to download episode {name}", episode.Title);

        
        return episode;
    }
}