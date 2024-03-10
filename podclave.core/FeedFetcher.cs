using System;
using System.Net.Http;

using System.Xml.Serialization;

using Podclave.Core.Models;
using Podclave.Core.Models.Deserialization;
namespace Podclave.Core;

public class FeedFetcher
{
    private readonly IDownloader _downloader;

    public FeedFetcher(IDownloader downloader)
    {
        _downloader = downloader;
    }

    public async Task<Feed> Fetch(string url)
    {
        var downloadResult = await _downloader.Download(url);
        var feed = ParseFeed(downloadResult);

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