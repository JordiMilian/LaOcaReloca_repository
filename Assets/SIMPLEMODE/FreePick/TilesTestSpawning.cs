using System;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class TilesTestSpawning : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField, toy_inputField;
    [SerializeField] FreePick freePick;

    public void AttemptSpawn()
    {
        
#if UNITY_EDITOR

        TileConfig config = ConfigsDatabase.GetTileConfigWithId(inputField.text);
        freePick.SpawnTile(config);
        /*

        string profileName = inputField.text + ".asset";
        
        string basicFolderPath = "Assets/SIMPLEMODE/Tiles/Configs/" + profileName;
        if(AssetDatabase.AssetPathExists(basicFolderPath))
        {
            config = AssetDatabase.LoadAssetAtPath<TileConfig>(basicFolderPath);
        }
        else
        {
            string[] subfolderPaths = AssetDatabase.GetSubFolders("Assets/SIMPLEMODE/Tiles/Configs");
            foreach (string subfolderPath in subfolderPaths)
            {
                string fullPath = subfolderPath + "/" + profileName;
                if (AssetDatabase.AssetPathExists(fullPath))
                {
                    config = AssetDatabase.LoadAssetAtPath<TileConfig>(fullPath);
                    break;
                }
            }
        }
            
        if(config == null)
        {
            Debug.LogWarning("TileConfig not found: " + profileName);
            return;
        }
        freePick.SpawnTile(config);
        */
#endif
        

    }
    public void AttemptSpawn_Toy()
    {
#if UNITY_EDITOR
        ToyConfig config = null;
        config = ConfigsDatabase.GetToyConfigWithId(toy_inputField.text);
        freePick.SpawnToy(config);

        /*
        string profileName = "Toy_" + toy_inputField.text + ".asset";

        string basicFolderPath = "Assets/SIMPLEMODE/Toys/Profiles/" + profileName;
        if (AssetDatabase.AssetPathExists(basicFolderPath))
        {
            config = AssetDatabase.LoadAssetAtPath<ToyConfig>(basicFolderPath);
        }
        else
        {
            string[] subfolderPaths = AssetDatabase.GetSubFolders("Assets/SIMPLEMODE/Toys/Profiles");
            foreach (string subfolderPath in subfolderPaths)
            {
                string fullPath = subfolderPath + "/" + profileName;
                if (AssetDatabase.AssetPathExists(fullPath))
                {
                    config = AssetDatabase.LoadAssetAtPath<ToyConfig>(fullPath);
                    break;
                }
            }
        }

        if (config == null)
        {
            Debug.LogError("Toy Profile not found: " + profileName);
            return;
        }
       */
#endif
    }
}
