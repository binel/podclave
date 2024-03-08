using System.Xml.Serialization;

namespace Podclave.Core.Models.Deserialization;

[XmlRoot("rss")]
public class StandardPodcastFeed
{
    [XmlElement("channel")]
    public Channel Channel {get; set;}
}

public class Channel
{
    [XmlElement("title")]
    public string Title {get; set;}

    [XmlElement("pubDate")]
    public string _publishedAtShim;

    public DateTime PublishedAt 
    {
        get {
            return DateTime.Parse(_publishedAtShim);
        }
    }
}