using System;
using System.Net.Http;

using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Podclave.Core.Exceptions;
using Podclave.Core.Models;
using Podclave.Core.Models.Deserialization;
namespace Podclave.Core;

public interface IFeedFetcher
{
    Task<Feed> Fetch(string url);
}

public class FeedFetcher: IFeedFetcher
{
    private readonly IFeedDownloader _downloader;
    private readonly ILogger<FeedFetcher> _logger;

    public FeedFetcher(
        IFeedDownloader downloader,
        ILogger<FeedFetcher> logger)
    {
        _downloader = downloader;
        _logger = logger;
    }

    public async Task<Feed> Fetch(string url)
    {
        _logger.LogDebug("Attempting to download feed at url {url}", url);
        var downloadResult = await _downloader.Download(url);
        _logger.LogDebug("Successfully downloaded feed. Attempting to parse...");
        var feed = ParseFeed(downloadResult);
        _logger.LogDebug("Successfully parsed feed for podcast {name}", feed.Name);

        return feed;
    }

    private Feed ParseFeed(string feed)
    {
        // Deserializer doesn't like whitespace at the start 
        feed = feed.TrimStart();
        
        XmlSerializer serializer = new XmlSerializer(typeof(StandardPodcastFeed));
        
        using (StringReader sr = new StringReader(feed))
        {
            StandardPodcastFeed? deserializedFeed;
            try {
                deserializedFeed = (StandardPodcastFeed?)serializer.Deserialize(sr);
            }
            catch (InvalidOperationException ex)
            {
                throw new FeedParseException("Unable to parse feed as StandardPodcastFeed", ex);
            }
            
            return new Feed
            {
                Name = deserializedFeed.Channel.Title,
                PublishedAt = deserializedFeed.Channel.PublishedAt,
                Episodes = deserializedFeed.Channel.Items.Select((i) => new Episode
                {
                    Title = i.Title,
                    PublishedAt = i.PublishedAt,
                    MediaLink = i.Enclosure.Url,
                }).ToList()
            };
        }
    }
}