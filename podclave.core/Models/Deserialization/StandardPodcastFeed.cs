using System.Xml.Serialization;
using Podclave.Core.Exceptions;

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
            if (string.IsNullOrEmpty(_publishedAtShim))
            {
                // TODO maybe do something else to indicate it didn't have a pubDate. Some 
                // feeds seem to not include this
                return DateTime.MinValue;
            }

            if (DateTime.TryParse(_publishedAtShim, out DateTime result))
            {
                return result;
            }

            throw new FeedParseException($"Unable to parse pubDate of {_publishedAtShim}");
        }
    }

    [XmlElement("item")]
    public List<Item> Items {get; set;} = new List<Item>();
}

public class Item 
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

    [XmlElement("enclosure")]
    public Enclosure Enclosure {get; set;}
}

public class Enclosure 
{
    [XmlAttribute("url")]
    public string Url {get; set;}

    [XmlAttribute("types")]
    public string Type {get; set;}
}