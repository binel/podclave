
using Podclave.Cli.Tasks;
using Podclave.Core;

namespace Podclave.Cli.Handlers;

public class EpisodeDownloadHandler : IHandler
{

    private readonly IEpisodeDownloader _episodeDownloader;

    public EpisodeDownloadHandler(IEpisodeDownloader episodeDownloader)
    {
        _episodeDownloader = episodeDownloader;
    }

    public async Task Handle(WorkTask t)
    {
        var episodeDownloadTask = t as EpisodeDownloadTask;

        await _episodeDownloader.Download(episodeDownloadTask.Episode);
    }
}