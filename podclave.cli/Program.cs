using System;

using Podclave.Core;
using Podclave.Core.Configuration;

namespace Podclave.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigLoader.Load();

            var feedFetcher = new FeedFetcher(new Downloader());


            foreach(var podcast in config.Podcasts)
            {
                var fetchTask = feedFetcher.Fetch(podcast.FeedUrl);
                fetchTask.Wait();

                if (podcast.DryRun)
                {
                    Console.WriteLine("Dry run set");
                }
            }
        }
    }
}