

namespace Podclave.Core.Tasks;

public class WorkTask 
{
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime DoNotWorkBefore {get; set;}
}
