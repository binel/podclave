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

            var feedFetcher = new FeedFetcher();
            var fetchTask = feedFetcher.Fetch(config.Podcasts[0].FeedUrl);
            fetchTask.Wait();

        }
    }
}