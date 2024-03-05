using System;
using System.IO;
using System.Xml.Serialization;

namespace Podclave.Core.Configuration;

public static class ConfigLoader
{
    public const string CONFIG_NAME = "config.xml";

    public static PodclaveConfig Load()
    {
        string path = string.Empty;
        // First look in the current directory
        if (File.Exists(CONFIG_NAME))
        {
            // TODO log something
            path = CONFIG_NAME;
        }
        else 
        {
            Console.WriteLine("Could not find configuration file!");
            return new PodclaveConfig();
        }

        Console.WriteLine($"Found config file at path: {path}");

        PodclaveConfig config;
        try 
        {

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {

                XmlSerializer serializer = new XmlSerializer(typeof(PodclaveConfig));
                PodclaveConfig? deserializedConfig = (PodclaveConfig?)serializer.Deserialize(fs);
                
                if (deserializedConfig == null)
                {
                    Console.WriteLine("Something went wrong while deserializing config.");
                    return new PodclaveConfig();
                }
                config = deserializedConfig;
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"Config is not well formed. Error: {e.Message}");
            return new PodclaveConfig(); 
        }

        return config;
    }
}