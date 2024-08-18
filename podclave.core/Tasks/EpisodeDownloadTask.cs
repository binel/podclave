
using Podclave.Core.Models;

namespace Podclave.Core.Tasks;

public class EpisodeDownloadTask : WorkTask
{
    public Episode Episode {get; set;}
}
