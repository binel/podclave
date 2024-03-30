using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
            
            if (deserializedFeed == null)
            {
                throw new FeedParseException("Unable to parse feed - deserialized feed was null");
            }

            Channel channel = new Channel();
            if (deserializedFeed.Channel == null)
            {
                throw new FeedParseException("Unable to parse feed - Could not determine channel");
            }
            else
            {
                channel = deserializedFeed.Channel;
            }



            if (string.IsNullOrEmpty(channel.Title))
            {
                throw new FeedParseException("Unable to parse feed - channel title was not valid");
            }

            if (channel.PublishedAt == DateTime.MinValue)
            {
                _logger.LogWarning("Feed {title} does not have a valid published at value", channel.Title);
            }

            if (channel.Items.Count == 0)
            {
                throw new FeedParseException("Unable to parse feed - no items found");
            }

            return new Feed
            {
                Name = channel.Title,
                PublishedAt = channel.PublishedAt,
                Episodes = channel.Items.Select((i) => new Episode
                {
                    Title = i.Title,
                    PublishedAt = i.PublishedAt,
                    MediaLink = i.Enclosure.Url,
                }).ToList()
            };
        }
    }
}