using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public static class ConfigsDatabase 
{
    public static Dictionary<string, TileConfig> configsDictionary;

    static ConfigsDatabase()
    {
        configsDictionary = Resources.LoadAll<TileConfig>("").ToDictionary(x => x._configId);
    }
    public static TileConfig GetTileConfigWithId(string id)
    {
        return configsDictionary[id];
    }
}
