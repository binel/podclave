using Podclave.Core.Configuration;

namespace Podclave.Core.Tasks;

public class FetchFeedTask: WorkTask
{
    public PodcastConfig Podcast {get; set;}
}
