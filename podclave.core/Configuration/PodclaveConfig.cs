using System;
using System.Xml.Serialization;

namespace Podclave.Core.Configuration;

[XmlRoot("PodclaveConfig")]
public class PodclaveConfig
{
    /// <summary>
    /// The root directory where podclave should store podcasts and other 
    /// configuration files. 
    /// </summary>
    [XmlElement("BaseDirectory")]
    public string BaseDirectory {get; set;}

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
    /// How many hours before a feed should be re-downloaded to check 
    /// for new episodes. 
    /// </summary>
    [XmlElement("FeedFetchIntervalHours")]
    public int FeedFetchIntervalHours {get; set;}

    /// <summary>
    /// Minimum time in seconds between when feeds may be downloaded. This 
    /// is to be polite to sites that host multiple feeds so we don't DOS them. 
    /// </summary>
    [XmlElement("FeedFetchCooldownSeconds")]
    public int FeedFetchCooldownSeconds {get; set;}

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
    /// Name of the feed - only impacts logging statements
    /// </summary>
    [XmlElement("Name")]
    public string Name {get; set;}

    /// <summary>
    /// What the name of the directory that stores this podcast should be. 
    /// This will be a directory within the Podclave base directory
    /// </summary>
    [XmlElement("DirectoryName")]
    public string DirectoryName {get; set;}

    /// <summary>
    /// URL of the RSS feed 
    /// </summary>
    [XmlElement("FeedUrl")]
    public string? FeedUrl {get; set;}

    /// <summary>
    /// If set to true, the number of episodes that would be downloaded 
    /// will be logged, but they will not actually be downloaded. 
    /// </summary>
    [XmlElement("DryRun")]
    public bool DryRun {get; set;}

    /// <summary>
    /// If set to true, this podcast will not be downloaded
    /// </summary>
    [XmlElement("Ignore")]
    public bool Ignore {get; set;}

    /// <summary>
    /// If set, any episodes published before this date will not be downloaded
    /// </summary>
    [XmlElement("IgnoreEpisodesBefore")]
    public DateTime IgnoreEpisodesBefore {get; set;}
}