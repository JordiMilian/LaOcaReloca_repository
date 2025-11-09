using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(ProfilesCreator))]
public class ProfilesCreator_Editor : Editor
{
    ProfilesCreator data;
    SerializedProperty prop_assetName, prop_folderName, prop_title;
    SerializedProperty prop_color, prop_baseDamage;
    SerializedProperty prop_rarity, prop_tag;
    bool useFolder;
    SerializedProperty prop_toyAssetName, prop_toyTitle;

    private void OnEnable()
    {
        data = (ProfilesCreator)target;
        prop_assetName = serializedObject.FindProperty("assetName");
        prop_folderName = serializedObject.FindProperty("folderName");
        prop_title = serializedObject.FindProperty("title");
        prop_color = serializedObject.FindProperty("color");
        prop_baseDamage = serializedObject.FindProperty("baseDamage");
        prop_rarity = serializedObject.FindProperty("rarity");
        prop_tag = serializedObject.FindProperty("tag");

        prop_toyAssetName = serializedObject.FindProperty("ToyAssetName");
        prop_toyTitle = serializedObject.FindProperty("ToyTitle");
    }

    public override void OnInspectorGUI()
    {
        CreateTileProfile();
        ChangeName();
        ToyProfileCreation();
    }

    string newName;
    string oldName;
    string newTitle;
    void CreateTileProfile()
    {
        bool showCreateScriptButton = true, showCreateInstanceButton = true;
        bool showDeleteTile = false;

        base.OnInspectorGUI();
        Undo.RecordObject(data, "CardsCreator");

        GUILayout.Space(5);
        GUILayout.Label("TILES PROFILES CREATION TOOL", EditorStyles.whiteBoldLabel);
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "To create a new Tile Profile you must first creat the script. " +
            "Wait until is assembles the new script and then create the instance",
            MessageType.Info);
        EditorGUILayout.PropertyField(prop_assetName);

        GUILayout.BeginHorizontal();

        useFolder = GUILayout.Toggle(useFolder, "Use Folder");
        GUI.enabled = useFolder;
        EditorGUILayout.PropertyField(prop_folderName);
        GUI.enabled = true;

        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        #region GET STRINGS
        string assetName = "Tile_" + prop_assetName.stringValue;
        string folderName = prop_folderName.stringValue;

        string instancepath;
        if (!useFolder)
        {
            instancepath = $"Assets/SIMPLEMODE/Tiles/Profiles/{assetName}.asset";
        }
        else { instancepath = $"Assets/SIMPLEMODE/Tiles/Profiles/{folderName}/{assetName}.asset"; }
        string scriptpath = $"Assets/SIMPLEMODE/Tiles/Scripts/{assetName}.cs";
        #endregion
        #region STRING CHECK
        if (assetName.Contains(' ') || prop_assetName.stringValue == "")
        {
            EditorGUILayout.HelpBox("Not valid asset name", MessageType.Error);
            return;
        }
        Type script_Type = Type.GetType(assetName + ", Assembly-CSharp");
        if (AssetDatabase.AssetPathExists(instancepath))
        {
            EditorGUILayout.HelpBox("That profile already exists", MessageType.Warning);
            showDeleteTile = true;
            showCreateScriptButton = false;
            showCreateInstanceButton = false;
        }
        else if (script_Type != null)
        {
            EditorGUILayout.HelpBox("A script with that name already exists, now create the instance", MessageType.Warning);
            showCreateScriptButton = false;
        }
        else
        {
            showCreateInstanceButton = false;
        }

        #endregion
        #region BUTTONS
        if (showCreateScriptButton)
        {
            if (GUILayout.Button("Create Script"))
            {
                //Create the script in the folder
                File.WriteAllText(scriptpath, GetEmptyTileProfileContent(assetName));
                AssetDatabase.Refresh();
                //We should wait for compiling time
            }
        }

        if (showCreateInstanceButton)
        {
            EditorGUILayout.PropertyField(prop_title);
            EditorGUILayout.PropertyField(prop_baseDamage);
            EditorGUILayout.PropertyField(prop_color);
            EditorGUILayout.PropertyField(prop_rarity); //TO DO Unique price
            EditorGUILayout.PropertyField(prop_tag);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Create Instance"))
            {
                //Create the folder if it doesnt exist
                string folderPath = $"Assets/SIMPLEMODE/Tiles/Profiles/{folderName}";
                if (!AssetDatabase.AssetPathExists(folderPath))
                {
                    AssetDatabase.CreateFolder("Assets/SIMPLEMODE/Tiles/Profiles", folderName);
                }
                //Find the type of Scriptable Object
                Type SO_type = Type.GetType(assetName + ", Assembly-CSharp");
                if (SO_type == null)
                {
                    Debug.LogError("Could not find type: " + assetName);
                    return;
                }

                Tile_Profile instance = (Tile_Profile)ScriptableObject.CreateInstance(SO_type);
                if (instance == null)
                {
                    Debug.LogError("Could not create instance of: " + assetName);
                    return;
                }

                instance.tileColor = prop_color.colorValue;
                instance.Title = prop_title.stringValue;
                instance.BaseDamage = prop_baseDamage.floatValue;
                instance.rarity = (Rarity)prop_rarity.enumValueIndex;
                instance.tileTags = new TileTags[] { (TileTags)prop_tag.enumValueIndex };

                AssetDatabase.CreateAsset(instance, instancepath);
                data.factory.tileProfiles.Add(instance);
                AssetDatabase.SaveAssets();

                Selection.activeObject = instance;
            }
        }
        if (showDeleteTile)
        {
            if (GUILayout.Button("Delete tile"))
            {
                //remove it from the factory
                Tile_Profile profileToDelete = AssetDatabase.LoadAssetAtPath<Tile_Profile>(instancepath);
                data.factory.tileProfiles.Remove(profileToDelete);

                AssetDatabase.DeleteAsset(scriptpath);
                AssetDatabase.DeleteAsset(instancepath);
            }
        }
        #endregion
    }
    void ChangeName()
    {
        GUILayout.Space(5);
        GUILayout.Label("TILES CHANGING NAME TOOL", EditorStyles.whiteBoldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Old asset name");
        oldName = EditorGUILayout.TextField(oldName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("New asset name");
        newName = EditorGUILayout.TextField(newName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("New title name");
        newTitle = EditorGUILayout.TextField(newTitle);
        GUILayout.EndHorizontal();


        string oldAssetName = $"Tile_{oldName}";
        string newAssetName = $"Tile_{newName}";
        string scriptPath = $"Assets/SIMPLEMODE/Tiles/Scripts/{oldAssetName}.cs";

        //Find the profile instance among the foulders
        string[] GUIDsFound = AssetDatabase.FindAssets(oldAssetName, new string[] { "Assets/SIMPLEMODE/Tiles/Profiles"});
        int rightIndex = -1;
        for (int i = 0; i < GUIDsFound.Length; i++)
        {
            string foundFileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(GUIDsFound[i]));
            if (oldAssetName + ".asset" == foundFileName)
            {
                rightIndex = i;
                Debug.Log($"Found {foundFileName}");
            }
        }
        if(rightIndex > -1 && AssetDatabase.AssetPathExists(scriptPath))
        {
            if (GUILayout.Button("CHANGE NAME"))
            {
                //rename the profile
                AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(GUIDsFound[rightIndex]), newAssetName);

                //Rename the script
                string scriptContent = File.ReadAllText(scriptPath);
                scriptContent = scriptContent.Replace(oldAssetName, newAssetName);
                File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.RenameAsset(scriptPath, newAssetName);

                //Rename title
                Tile_Profile profile = AssetDatabase.LoadAssetAtPath<Tile_Profile>(AssetDatabase.GUIDToAssetPath(GUIDsFound[rightIndex]));
                profile.Title = newTitle;

                AssetDatabase.Refresh();
            }
        }

        
    }
    void ToyProfileCreation()
    {
        bool showCreateScriptButton = true, showCreateInstanceButton = true;
        Undo.RecordObject(data, "ToyCreator");

        GUILayout.Space(5);
        GUILayout.Label("TOY PROFILES CREATION TOOL", EditorStyles.whiteBoldLabel);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(prop_toyAssetName);
        serializedObject.ApplyModifiedProperties();

        #region GET STRINGS
        string assetName = "Toy_" + prop_toyAssetName.stringValue;
        string ScriptPath = $"Assets/SIMPLEMODE/Toys/Profiles/Scripts/{assetName}.cs";
        string instancePath = $"Assets/SIMPLEMODE/Toys/Profiles/{assetName}.asset";
        #endregion
        #region STRING CHECK
        if (assetName.Contains(' ') || prop_toyAssetName.stringValue == "")
        {
            EditorGUILayout.HelpBox("Not valid asset name", MessageType.Error);
            return;
        }
        Type script_Type = Type.GetType(assetName + ", Assembly-CSharp");
        if (AssetDatabase.AssetPathExists(instancePath))
        {
            EditorGUILayout.HelpBox("That profile already exists", MessageType.Warning);
            //delete button if necessary
            showCreateScriptButton = false;
            showCreateInstanceButton = false;
        }
        else if (script_Type != null)
        {
            EditorGUILayout.HelpBox("A script with that name already exists, now create the instance", MessageType.Warning);
            showCreateScriptButton = false;
        }
        else
        {
            showCreateInstanceButton = false;
        }
        #endregion
        #region BUTTONS
        if (showCreateScriptButton)
        {
            if (GUILayout.Button("Create Script"))
            {
                //Create the script in the folder
                File.WriteAllText(ScriptPath, GetEmptyToyProfileContent(assetName));
                AssetDatabase.Refresh();
            }
        }
        if (showCreateInstanceButton)
        {
            EditorGUILayout.PropertyField(prop_toyTitle);

            if (GUILayout.Button("Create Instance"))
            {
                Type SO_type = Type.GetType(assetName + ", Assembly-CSharp");
                if (SO_type == null)
                {
                    Debug.LogError("Could not find type: " + assetName);
                    return;
                }

                Toy_Profile instance = (Toy_Profile)ScriptableObject.CreateInstance(SO_type);
                if (instance == null)
                {
                    Debug.LogError("Could not create instance of: " + assetName);
                    return;
                }

                instance.Title = prop_toyTitle.stringValue;

                AssetDatabase.CreateAsset(instance, instancePath);
                data.toysManager.AllToyProfiles.Add(instance);
                AssetDatabase.SaveAssets();

                Selection.activeObject = instance;
            }
        }
        #endregion
    }


    string GetEmptyTileProfileContent(string scriptName)
    {
        return $"using UnityEngine;\n" +
            "using System.Collections;\n" +
            "using static StringTools;\n" +
            $"public class {scriptName} : Tile_Profile\n" +
            "{\n" +
            "   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }\n" +
            "   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }\n" +
            "   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }\n" +
            "   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }\n" +
            "   //public override string GetTooltipText() { }\n" +
            "}";
    }
    string GetEmptyToyProfileContent(string scriptName)
    {
        return $"using UnityEngine;\n" +
            "using System.Collections;\n" +
            "using static StringTools;\n" +
            $"public class {scriptName} : Toy_Profile\n" +
            "{\n" +
            "   public override void OnActivatedToy()\n" +
            "{\n" +
            "\n" +
            "}\n" +
            "   public override void OnDeactivatedToy()" +
            "{\n" +
            "\n" +
            "}\n" +
            "  public override string GetTooltipDescription() { return string.Empty; }\n" +
            "}";
    }
}
