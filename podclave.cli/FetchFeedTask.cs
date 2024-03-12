
using System.Reflection.Metadata.Ecma335;
using Podclave.Core.Configuration;

namespace Podclave.Cli;

public class FetchFeedTask: WorkTask
{
    public PodcastConfig Podcast {get; set;}
}