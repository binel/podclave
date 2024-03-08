using Podclave.Core;

namespace Podclave.Core.Test.Mocks;

public class MockDownloader : IDownloader
{
    public Task<string> Download(string feedUrl)
    {
        return Task.FromResult(File.ReadAllText("SampleFeeds/taz_2024_03_07.rss"));
    }
}