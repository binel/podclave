using System;
using System.Net.Http;

using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Podclave.Core.Models;
using Podclave.Core.Models.Deserialization;
namespace Podclave.Core;

public interface IFeedFetcher
{
    Task<Feed> Fetch(string url);
}

public class FeedFetcher: IFeedFetcher
{
    private readonly IDownloader _downloader;
    private readonly ILogger<FeedFetcher> _logger;

    public FeedFetcher(
        IDownloader downloader,
        ILogger<FeedFetcher> logger)
    {
        _downloader = downloader;
        _logger = logger;
    }

    public async Task<Feed> Fetch(string url)
    {
        _logger.LogInformation("Attempting to download feed at url {url}", url);
        var downloadResult = await _downloader.Download(url);
        _logger.LogInformation("Successfully downloaded feed. Attempting to parse...");
        var feed = ParseFeed(downloadResult);
        _logger.LogInformation("Successfully parsed feed for podcast {name}", feed.Name);

        return feed;
    }

    private Feed ParseFeed(string feed)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(StandardPodcastFeed));
        
        using (StringReader sr = new StringReader(feed))
        {
            StandardPodcastFeed? deserializedFeed = (StandardPodcastFeed?)serializer.Deserialize(sr);
            return new Feed
            {
                Name = deserializedFeed.Channel.Title,
                PublishedAt = deserializedFeed.Channel.PublishedAt,
                Episodes = deserializedFeed.Channel.Items.Select((i) => new Episode
                {
                    Title = i.Title,
                    PublishedAt = i.PublishedAt,
                    MediaLink = i.Enclosure.Url
                }).ToList()
            };
        }
    }
}