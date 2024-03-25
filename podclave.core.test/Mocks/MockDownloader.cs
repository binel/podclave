using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Podclave.Core;

namespace Podclave.Core.Test.Mocks;

public class MockFeedDownloader : IFeedDownloader
{
    private const string TAZ_FEED = "SampleFeeds/taz_2024_03_07.rss";
    private const string DAMES_FEED = "SampleFeeds/dames_2024_03_25.rss";

    private string _selectedFeed = TAZ_FEED;

    public Task<string> Download(string feedUrl)
    {
        return Task.FromResult(File.ReadAllText(_selectedFeed));
    }

    public void SetToReturnTaz()
    {
        _selectedFeed = TAZ_FEED;
    }

    public void SetToReturnDames()
    {
        _selectedFeed = DAMES_FEED;
    }
}