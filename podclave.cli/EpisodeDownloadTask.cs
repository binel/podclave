
using Podclave.Core.Models;

namespace Podclave.Cli;

public class EpisodeDownloadTask : WorkTask
{
    public Episode Episode {get; set;}
}