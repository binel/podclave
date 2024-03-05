using System;

using Podclave.Core.Configuration;

namespace Podclave.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigLoader.Load();

            Console.WriteLine($"Config value for base delay: {config.RequestDelayBaseSeconds}");
            Console.WriteLine($"Config value for random delay: {config.RequestDelayRandomOffsetSeconds}");
            Console.WriteLine($"Number of podcasts: {config.Podcasts.Count}");
            Console.WriteLine($"First podcast url: {config.Podcasts[0].FeedUrl}");
        }
    }
}