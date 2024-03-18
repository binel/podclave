

namespace Podclave.Core.Models;

public class Episode
{
    public string FeedName {get; set;}

    public string Title { get; set; }

    public DateTime PublishedAt { get; set; }
    
    public string MediaLink {get; set;}
}