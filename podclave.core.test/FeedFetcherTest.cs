using Podclave.Core;
using Podclave.Core.Test.Mocks;

namespace podclave.core.test;

public class FeedFetcherTests
{
    private FeedFetcher _feedFetcher;
    private MockDownloader _mockDownloader;

    [SetUp]
    public void Setup()
    {
        _mockDownloader = new MockDownloader();

        _feedFetcher = new FeedFetcher(_mockDownloader);
    }

    [Test]
    public async Task HappyPath()
    {
        var feed = await _feedFetcher.Fetch("test");

        Assert.That(feed.Name, Is.EqualTo("The Adventure Zone"));
        Assert.That(feed.PublishedAt, Is.EqualTo(new DateTime(2024, 03, 07, 06, 0, 0)));
    }
}