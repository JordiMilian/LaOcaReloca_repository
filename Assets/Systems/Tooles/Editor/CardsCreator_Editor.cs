using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Unity.VisualScripting;

[CustomEditor(typeof(CardsCreator))]
public class CardsCreator_Editor : Editor
{
    CardsCreator Refs;
    SerializedProperty assetNameProperty;
    SerializedProperty baseCardPrefab_Property;
    SerializedProperty targetCardPrefab_property;
    SerializedProperty colorProperty;
    SerializedProperty crossedDamage;
    SerializedProperty displayName;
    SerializedProperty shopController;
    SerializedProperty cardRarity;
    SerializedProperty tileTagsProp;
    private void OnEnable()
    {
        Refs = (CardsCreator)target;
        assetNameProperty = serializedObject.FindProperty("AssetName");
        baseCardPrefab_Property = serializedObject.FindProperty("BaseCardPrefab");
        targetCardPrefab_property = serializedObject.FindProperty("TargetPrefab");
        colorProperty = serializedObject.FindProperty("CardColor");
        crossedDamage = serializedObject.FindProperty("DefaultCrossedDamage");
        displayName = serializedObject.FindProperty("DisplayName");
        shopController = serializedObject.FindProperty("shopController");
        cardRarity = serializedObject.FindProperty("cardRarity");
        tileTagsProp = serializedObject.FindProperty("tileTags");
    }
    string EditingTileName = "test";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Undo.RecordObject(Refs, "CardsCreator");

        #region BASIC DISPLAY
        // TITLES AND HELPBOX
        GUILayout.Space(5);
        GUILayout.Label("CARDS CREATION TOOL", EditorStyles.whiteBoldLabel);
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Before creating a new tile you must create a script with the same name and wait for compilation", MessageType.Info);

        //BASIC REFERENCES
        EditorGUILayout.PropertyField(baseCardPrefab_Property);
        EditorGUILayout.PropertyField(shopController);

        //ASSET NAME
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(assetNameProperty);
        #endregion

        string scriptName = $"Tile_{assetNameProperty.stringValue}";
        string scriptPath = $"Assets/SIMPLEMODE/Tiles/Scripts/{scriptName}.cs";
        string PrefabName = $"TilePrefab_{assetNameProperty.stringValue}";
        string prefabPath = $"Assets/SIMPLEMODE/Tiles/Prefabs/{PrefabName}.prefab";

        #region ASSET NAME CHECK
        if (assetNameProperty.stringValue == "")
        {
            //EditorGUILayout.HelpBox("Card name can't be empty", MessageType.Warning);
            goto skipCardsCreation;
        }
        if(assetNameProperty.stringValue.Contains(' '))
        {
            EditorGUILayout.HelpBox("Card name can't contain spaces", MessageType.Warning);
            goto skipCardsCreation;
        }
        #endregion
        #region SCRIPT CREATION
        bool createScriptButton_IsEnabled = true;
        if (AssetDatabase.AssetPathExists(scriptPath))
        {
            createScriptButton_IsEnabled = false;
        }
        GUI.enabled = createScriptButton_IsEnabled;
        if (GUILayout.Button("CREATE SCRIPT"))
        {
            File.WriteAllText(scriptPath, GetEmptyScriptContent(scriptName));
            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
        #endregion
        #region PREFAB CREATION
        GUILayout.Space(10);

        bool createPrefabButton_IsEnabled = true;
        if (baseCardPrefab_Property.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Base Card Prefab is not assigned", MessageType.Warning);
            createPrefabButton_IsEnabled = false;
        }
        else if (AssetDatabase.AssetPathExists(prefabPath))
        {
            EditorGUILayout.HelpBox($"A prefab with name {PrefabName} already exists", MessageType.Warning);
            createPrefabButton_IsEnabled = false;
        }
        else if (!AssetDatabase.AssetPathExists(scriptPath))
        {
            createPrefabButton_IsEnabled = false;
        }

        GUI.enabled = createPrefabButton_IsEnabled;
        EditorGUILayout.PropertyField(displayName);
        EditorGUILayout.PropertyField(colorProperty);
        EditorGUILayout.PropertyField(crossedDamage);
        EditorGUILayout.PropertyField(cardRarity);
        EditorGUILayout.PropertyField(tileTagsProp);

        if (displayName.stringValue == "" || (Rarity)cardRarity.enumValueIndex == Rarity.none)
        {
            EditorGUILayout.HelpBox("Make sure to fill all info", MessageType.Warning);
            GUI.enabled = false;
        }

        if (GUILayout.Button("CREATE CARD PREFAB"))
        {
            CreateNewCardPrefab();
        }
        GUI.enabled = true;
        //
        void CreateNewCardPrefab()
        {
            targetCardPrefab_property.objectReferenceValue =
                    PrefabUtility.SaveAsPrefabAsset(
                        (GameObject)baseCardPrefab_Property.objectReferenceValue,
                        prefabPath);

            GameObject targetPrefab_GO = (GameObject)targetCardPrefab_property.objectReferenceValue;

            MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            Type tileLogicType = scriptAsset.GetClass();
            targetPrefab_GO.AddComponent(tileLogicType);

            Tile_Base tileLogic = targetPrefab_GO.GetComponent<Tile_Base>();
            tileLogic.tileColor = colorProperty.colorValue;
            tileLogic.TitleText = displayName.stringValue;
            tileLogic.defaultCrossedDamage = crossedDamage.floatValue;
            tileLogic.rarity = (Rarity)cardRarity.enumValueIndex;
            //set tile tags somehow

            ShopController shop = shopController.objectReferenceValue as ShopController;
            shop.TilesAppeareable.Add(targetPrefab_GO);

            EditorUtility.SetDirty(shop);
            EditorUtility.SetDirty(targetPrefab_GO);
            AssetDatabase.SaveAssets();
        }
        #endregion
        
        skipCardsCreation:


        GUILayout.Space(5);
        GUILayout.Label("CARDS EDITING TOOL", EditorStyles.whiteBoldLabel);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tile Name");
        EditingTileName = GUILayout.TextField(EditingTileName);
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
    string GetEmptyScriptContent(string scriptName)
    {
        return $"using UnityEngine;\n" +
            "using System.Collections;\n" +
            $"public class {scriptName} : Tile_Base\n" +
            "{\n" +
            "   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }\n"+
            "   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }\n" +
            "   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }\n" +
            "   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }\n" +
            "   //public virtual string GetTooltipText() { }\n" +
            "}";
    }
}
