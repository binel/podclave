using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Podclave.Core.Configuration;

public interface IConfigLoader
{
    PodclaveConfig? Load();
}

public class ConfigLoader: IConfigLoader
{
    public const string CONFIG_NAME = "config.xml";

    public const string CONFIG_LOCATION_VAR_NAME = "PODCLAVE_CONFIG_PATH";

    private readonly ILogger<ConfigLoader> _logger;

    private PodclaveConfig? _config;

    public ConfigLoader(ILogger<ConfigLoader> logger)
    {
        _logger = logger;
    }

    public PodclaveConfig? Load()
    {
        if (_config != null)
        {
            return _config;
        }

        string path = string.Empty;
        var env_path = Environment.GetEnvironmentVariable(CONFIG_LOCATION_VAR_NAME);
        if (!string.IsNullOrEmpty(env_path))
        {
            if (!File.Exists(env_path))
            {
                _logger.LogWarning($"Config file not found at env path {env_path}!");
            }
            else 
            {
                path = env_path;
            }
        }
        // First look in the current directory
        else if (File.Exists(CONFIG_NAME))
        {
            _logger.LogInformation("Using configuration file from current directory.");
            path = CONFIG_NAME;
        }
        else 
        {
            _logger.LogError("Could not find configuration file! Cannot continue!");
            return null;
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
                    return null;
                }
                config = deserializedConfig;
            }
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError($"Config is not well formed. Error: {e.Message}");
            return null;
        }

        _config = config;
        return config;
    }
}