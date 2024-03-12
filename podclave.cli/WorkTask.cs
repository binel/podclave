
using System.Reflection.Metadata.Ecma335;

namespace Podclave.Cli;

public class WorkTask 
{
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime DoNotWorkBefore {get; set;}
}