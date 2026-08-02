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
        TileConfig config = null;

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
        freePick.SpawnTile(config._configInfo);
#endif
        

    }
    public void AttemptSpawn_Toy()
    {
#if UNITY_EDITOR
        Toy_Profile profile = null;

        string profileName = "Toy_" + toy_inputField.text + ".asset";

        string basicFolderPath = "Assets/SIMPLEMODE/Toys/Profiles/" + profileName;
        if (AssetDatabase.AssetPathExists(basicFolderPath))
        {
            profile = AssetDatabase.LoadAssetAtPath<Toy_Profile>(basicFolderPath);
        }
        else
        {
            string[] subfolderPaths = AssetDatabase.GetSubFolders("Assets/SIMPLEMODE/Toys/Profiles");
            foreach (string subfolderPath in subfolderPaths)
            {
                string fullPath = subfolderPath + "/" + profileName;
                if (AssetDatabase.AssetPathExists(fullPath))
                {
                    profile = AssetDatabase.LoadAssetAtPath<Toy_Profile>(fullPath);
                    break;
                }
            }
        }

        if (profile == null)
        {
            Debug.LogError("Toy Profile not found: " + profileName);
            return;
        }
        freePick.SpawnToy(profile);
#endif
    }
}
