using Podclave.Core.Configuration;

namespace Podclave.Cli.Tasks;

public class FetchFeedTask: WorkTask
{
    public PodcastConfig Podcast {get; set;}
}