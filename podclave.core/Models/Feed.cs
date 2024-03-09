
namespace Podclave.Core.Models;

public class Feed
{
    public string Name {get; set;}

    public DateTime PublishedAt {get; set;}

    public List<Episode> Episodes {get; set;} = new List<Episode>();
}