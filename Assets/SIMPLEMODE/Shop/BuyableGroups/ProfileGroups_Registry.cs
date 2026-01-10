using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Singletons/TilesGroupRegistry")]
public class ProfileGroups_Registry : ScriptableObject
{
    [SerializeField] ProfilesGroup Common, Rare, Legendary, Unique, Deprecated;
    [SerializeField] ProfilesGroup Curse, Tokens, Food;
    [SerializeField] ProfilesGroup Rest;
    public List<ProfilesGroup> GetGroups(Tile_Profile profile)
    {
        List<ProfilesGroup> groups = new();
        switch (profile.rarity)
        {
            case Rarity.Common: groups.Add(Common); break;
            case Rarity.Rare: groups.Add(Rare); break;
            case Rarity.Legendary: groups.Add(Legendary); break;
            case Rarity.Unique: groups.Add(Unique); break;
            case Rarity.Deprecated: groups.Add(Deprecated); return groups; //if its deprecated don't add it anywhere else
        }

        foreach (TileTags tag in profile.tileTags) 
        {
            switch(tag)
            {
                case TileTags.Token: groups.Add(Tokens);break;
                case TileTags.Curse: groups.Add(Curse);break;
                case TileTags.Food: groups.Add(Food);break;
            }
        }

        if(groups.Count == 0) { groups.Add(Rest); }
        return groups;
       
    }
    public ProfilesGroup[] GetAllGroups()
    {
        return new ProfilesGroup[]
        {
            Common,Rare,Legendary,Unique, Deprecated,
            Curse ,Tokens, Food,
            Rest 
        };
    }
}
public static class TilesGroupRegistry_singleton
{
     static ProfileGroups_Registry instance;
    public static ProfileGroups_Registry Instance
    {
        get
        {
            if (instance == null)
            {


#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    instance = UnityEditor.AssetDatabase.LoadAssetAtPath<ProfileGroups_Registry>(
                    "Assets/Resources/ProfileGroups_Registry.asset");
                }
                else
#endif
                {
                    instance = Resources.Load<ProfileGroups_Registry>("ProfileGroups_Registry");
                }
            }
            return instance;
        }
    }
}
