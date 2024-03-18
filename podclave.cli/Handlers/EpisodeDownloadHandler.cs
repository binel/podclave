
using System.IO.Enumeration;
using Microsoft.Extensions.Logging;
using Podclave.Cli.Tasks;
using Podclave.Core;
using Podclave.Core.Configuration;
using Podclave.Core.Models;

namespace Podclave.Cli.Handlers;

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

        var config = _configLoader.Load();
        var podcastConfig = config.Podcasts
            .Where(p => p.Name == episodeDownloadTask.Episode.FeedName)
            .FirstOrDefault();

        var path = $"{config.BaseDirectory}/{podcastConfig.DirectoryName}";

        if (!Directory.Exists(path))
        {
            _logger.LogInformation("Directory for {name} doesn't exist. Creating {path}",
                podcastConfig.Name, path);
            
            Directory.CreateDirectory(path);
        }

        byte[]? responseData = null;
        using (HttpClient client = new HttpClient())
        {
            var link = episodeDownloadTask.Episode.MediaLink;
            _logger.LogInformation("Attempting to download episode for {name} at {link}", podcastConfig.Name, link);
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

        string filename = episodeDownloadTask.Episode.PublishedAt.ToString("yyyy_MM_dd") + ".mp3";

        using (FileStream fs = new FileStream($"{path}/{filename}", FileMode.Create))
        {
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(responseData);
            bw.Close();
        }
    }
}