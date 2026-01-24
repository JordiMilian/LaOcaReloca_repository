using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile_Profile),true)]
public class TileController_Editor : Editor
{
    Tile_Profile refs;

    SerializedProperty rarityProp;
    SerializedProperty uniquePriceProp;
    private void OnEnable()
    {
        refs = target as Tile_Profile;

        rarityProp = serializedObject.FindProperty("rarity");
        uniquePriceProp = serializedObject.FindProperty("uniquePrice");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Rarity Enum
        EditorGUILayout.PropertyField(rarityProp);

        // Show unique price only if rarity is Unique
        if ((Rarity)rarityProp.enumValueIndex == Rarity.Unique)
        {
            EditorGUILayout.PropertyField(uniquePriceProp);
        }

        // Draw the rest of the default inspector
        DrawPropertiesExcluding(serializedObject, "rarity", "uniquePrice");

        // Show card description preview
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Description", refs.GetGenericSkillsText()+ refs.GetTooltipText(), EditorStyles.helpBox);

        serializedObject.ApplyModifiedProperties();
    }
}
