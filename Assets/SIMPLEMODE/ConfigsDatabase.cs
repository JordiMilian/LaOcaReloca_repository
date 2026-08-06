using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ConfigsDatabase 
{
    public static Dictionary<string, TileConfig> configsDictionary;
    public static Dictionary<string , ToyConfig> toyConfigDictionary;

    static ConfigsDatabase()
    {
        configsDictionary = new Dictionary<string, TileConfig>();
        TileConfig[] configsArray = Resources.LoadAll<TileConfig>("");
        foreach (TileConfig config in configsArray)
        {
            if (configsDictionary.ContainsKey(config.name)) { Debug.Log("duplicated key: " + config.name); }
            configsDictionary.Add(config.name, config);
        }

        toyConfigDictionary = new Dictionary<string, ToyConfig>();
        ToyConfig[] toyConfigsArray = Resources.LoadAll<ToyConfig>("");
        foreach (ToyConfig config in toyConfigsArray)
        {
            if (toyConfigDictionary.ContainsKey(config.name)) { Debug.Log("duplicated key: " + config.name); }
            toyConfigDictionary.Add(config.name, config);
        }
    }
    public static TileConfig GetTileConfigWithId(string id)
    {
        if (configsDictionary.ContainsKey(id)) { return configsDictionary[id]; }
        else { Debug.LogError(id + " not found in database"); return null; }
        
    }
    public static ToyConfig GetToyConfigWithId(string id)
    {
        if (toyConfigDictionary.ContainsKey(id)) { return toyConfigDictionary[id]; }
        else { Debug.LogError(id + " not found in database"); return null; }
    }
}
