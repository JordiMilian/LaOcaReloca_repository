using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "TileConfig", fileName = "NewConfig")]
public class TileConfig : ScriptableObject
{
    
    [SerializeReference]
    [InlineProperty]
    public TileInfo _configInfo;
    public Texture _texture;

#if UNITY_EDITOR
    //On validate, move this profile to the proper groups according to tags and rarity
    
    private void OnValidate()
    {
        ProfileGroups_Registry registry = TilesGroupRegistry_singleton.Instance;

        //Remove from all groups
        foreach (ProfilesGroup group in registry.GetAllGroups())
        {
            if (group.tilesList.Remove(this))
            {
                EditorUtility.SetDirty(group);
            }
        }

        //Add them to the proper groups
        List<ProfilesGroup> properGroups = registry.GetProfileGroups(_configInfo);
        foreach (ProfilesGroup group in properGroups)
        {
            group.tilesList.Add(this);
            EditorUtility.SetDirty(group);
        } 
    }
    
#endif

}
