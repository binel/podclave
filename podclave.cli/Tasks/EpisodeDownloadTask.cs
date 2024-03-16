
using Podclave.Core.Models;

namespace Podclave.Cli.Tasks;

public class EpisodeDownloadTask : WorkTask
{
    public Episode Episode {get; set;}
}