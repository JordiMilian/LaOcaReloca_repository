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
        /*
#if UNITY_EDITOR
        TileStateClass profile = null;

        string profileName = "Tile_" + inputField.text + ".asset";
        
        string basicFolderPath = "Assets/SIMPLEMODE/Tiles/Profiles/" + profileName;
        if(AssetDatabase.AssetPathExists(basicFolderPath))
        {
            profile = AssetDatabase.LoadAssetAtPath<TileStateClass>(basicFolderPath);
        }
        else
        {
            string[] subfolderPaths = AssetDatabase.GetSubFolders("Assets/SIMPLEMODE/Tiles/Profiles");
            foreach (string subfolderPath in subfolderPaths)
            {
                string fullPath = subfolderPath + "/" + profileName;
                if (AssetDatabase.AssetPathExists(fullPath))
                {
                    profile = AssetDatabase.LoadAssetAtPath<TileStateClass>(fullPath);
                    break;
                }
            }
        }
            
        if(profile == null)
        {
            Debug.LogError("Tile Profile not found: " + profileName);
            return;
        }
        freePick.SpawnTile(profile);
#endif
        */

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
