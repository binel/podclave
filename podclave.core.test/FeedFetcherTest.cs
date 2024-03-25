using System.Text;
using Microsoft.Extensions.Logging;
using Podclave.Core;
using Podclave.Core.Models;
using Podclave.Core.Test.Mocks;

namespace podclave.core.test;

public class FeedFetcherTests
{
    private FeedFetcher _feedFetcher;
    private MockFeedDownloader _mockDownloader;

    [SetUp]
    public void Setup()
    {
        _mockDownloader = new MockFeedDownloader();
        _feedFetcher = new FeedFetcher(_mockDownloader, new MockLogger<FeedFetcher>());
    }

    [Test]
    public async Task Taz()
    {
        _mockDownloader.SetToReturnTaz();
        var feed = await _feedFetcher.Fetch("test");

        Assert.That(feed.Name, Is.EqualTo("The Adventure Zone"));
        Assert.That(feed.Episodes.Count, Is.Not.EqualTo(0));
    }

    [Test]
    public async Task Dames()
    {
        _mockDownloader.SetToReturnDames();
        var feed = await _feedFetcher.Fetch("test");

        Assert.That(feed.Name, Is.EqualTo("LegendLark"));
        Assert.That(feed.Episodes.Count, Is.Not.EqualTo(0));
    }
}