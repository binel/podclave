using System;
using System.Net.Http;

using System.Xml.Serialization;

using Podclave.Core.Models;
using Podclave.Core.Models.Deserialization;
namespace Podclave.Core;

public class FeedFetcher
{
    public async Task<FeedFetchResult> Fetch(string url)
    {
        FeedFetchResult result = new();

        var downloadResult = await DownloadFeed(url);

        if (downloadResult.IsError)
        {
            result.IsError = true;
            return result;
        }
        Console.WriteLine(downloadResult.Feed);
        var feed = ParseFeed(downloadResult.Feed);


        return result;
    }

    private FeedParseResult ParseFeed(string feed)
    {
        FeedParseResult result = new();

        XmlSerializer serializer = new XmlSerializer(typeof(StandardPodcastFeed));
        
        using (StringReader sr = new StringReader(feed))
        {
            StandardPodcastFeed? deserializedFeed = (StandardPodcastFeed?)serializer.Deserialize(sr);
            Console.WriteLine(deserializedFeed.Channel.Title);
        }           

        return result;
    }

    private async Task<FeedDownloadResult> DownloadFeed(string url)
    {
        FeedDownloadResult result = new();

        using (HttpClient client = new HttpClient())
        {
            try 
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    result.Feed = responseBody;
                }
            }
            catch (Exception e)
            {
                result.IsError = true;
                Console.WriteLine($"An error occured try to fetch feed from {url}. Error: {e.Message}");
            }
        }

        return result;
    }

    private class FeedDownloadResult 
    {
        public bool IsError {get; set;}

        public string Feed {get; set;}
    }

    private class FeedParseResult
    {
        public bool IsError {get; set;}

        public Feed Feed {get; set;}
    }
}

public class FeedFetchResult
{
    public bool IsError {get; set;}

    public List<Feed> Feed {get; set;}
}