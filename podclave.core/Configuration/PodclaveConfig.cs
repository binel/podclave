using System;
using System.Xml.Serialization;

namespace Podclave.Core.Configuration;

[XmlRoot("PodclaveConfig")]
public class PodclaveConfig
{
    /// <summary>
    /// The minimum amount of time between requests in seconds. The 
    /// actual delay between requests is influenced by
    /// RequestDelayRandomOffsetSeconds, which adds in a random delay. 
    /// </summary>
    [XmlElement("RequestDelayBaseSeconds")]
    public int RequestDelayBaseSeconds {get; set;} 

    /// <summary>
    /// To choose the delay between requests, a number between 0 and 
    /// RequestDelayRandomOffsetSeconds is chosen and added to 
    /// RequestDelayBaseSeconds.
    /// </summary>
    [XmlElement("RequestDelayRandomOffsetSeconds")]
    public int RequestDelayRandomOffsetSeconds {get; set;}

    /// <summary>
    /// The collection of podcasts that are being managed 
    /// </summary>
    [XmlArray("Podcasts")]
    [XmlArrayItem("Podcast")]
    public List<PodcastConfig> Podcasts { get; set; } = new List<PodcastConfig>();
}

public class PodcastConfig
{
    /// <summary>
    /// URL of the RSS feed 
    /// </summary>
    [XmlElement("FeedUrl")]
    public string? FeedUrl {get; set;}
}