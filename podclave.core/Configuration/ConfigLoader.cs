using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Podclave.Core.Configuration;

public interface IConfigLoader
{
    PodclaveConfig Load();
}

public class ConfigLoader: IConfigLoader
{
    public const string CONFIG_NAME = "config.xml";

    private readonly ILogger<ConfigLoader> _logger;

    private PodclaveConfig? _config;

    public ConfigLoader(ILogger<ConfigLoader> logger)
    {
        _logger = logger;
    }

    public PodclaveConfig Load()
    {
        if (_config != null)
        {
            return _config;
        }

        string path = string.Empty;
        // First look in the current directory
        if (File.Exists(CONFIG_NAME))
        {
            // TODO log something
            path = CONFIG_NAME;
        }
        else 
        {
            _logger.LogError("Could not find configuration file!");
            return new PodclaveConfig();
        }

        _logger.LogInformation("Found config file at path: {path}", path);

        PodclaveConfig config;
        try 
        {

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {

                XmlSerializer serializer = new XmlSerializer(typeof(PodclaveConfig));
                PodclaveConfig? deserializedConfig = (PodclaveConfig?)serializer.Deserialize(fs);
                
                if (deserializedConfig == null)
                {
                    _logger.LogError("Something went wrong while deserializing config.");
                    return new PodclaveConfig();
                }
                config = deserializedConfig;
            }
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError($"Config is not well formed. Error: {e.Message}");
            return new PodclaveConfig(); 
        }

        _config = config;
        return config;
    }
}