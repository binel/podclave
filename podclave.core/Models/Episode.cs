

namespace Podclave.Core.Models;

public class Episode
{
    public string FeedName {get; set;}

    public string Title { get; set; }

    public DateTime PublishedAt { get; set; }
    
    public string MediaLink {get; set;}

    public string Filename 
    {
        get 
        {
            if (PublishedAt == DateTime.MinValue)
            {
                throw new InvalidDataException("PublishedAt is not set");
            }

            // TODO eventually you need to verify that what you are 
            // downloading is actually an mp3. 
            return "E_" + PublishedAt.ToString("yyyy_MM_dd") + ".mp3";
        }
    }
}