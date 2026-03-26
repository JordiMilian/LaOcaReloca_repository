using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile groups are used to store profiles for diferent usages. To separate them by tags or shop rarity/prize
/// To sete up a group you must go to the Profile Groups folder and create an instance of a new group
/// Then go to the TilesGroupRegistry and register that group: (the registry is inside the Resources Folder)
///     - Create a serialized variable with a name
///     - Add the variable in the GetAllGroups Method
///     - In the GetGroups() method you should program a way for the script to know how to add a newly created profile into its group. By using tags or rarity mainly
/// If the newly created group is a separated group in the shop to consider among the others (common, rare, etc) make sure to create a buyablesGroup_tiles too    
/// 
/// </summary>

[CreateAssetMenu(menuName = "ProfilesGroup", fileName = "new ProfilesGroup")]
public class ProfilesGroup : ScriptableObject
{
    public string GroupName;
    public List<TileStateClass> tilesList = new();

    public TileStateClass GetRandomProfile()
    {
        return tilesList[Random.Range(0, tilesList.Count)];
    }
}
