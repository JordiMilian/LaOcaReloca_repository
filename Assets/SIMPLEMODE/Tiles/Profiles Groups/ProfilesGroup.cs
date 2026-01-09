using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProfilesGroup", fileName = "new ProfilesGroup")]
public class ProfilesGroup : ScriptableObject
{
    public string GroupName;
    public List<Tile_Profile> tilesList = new();
    public float ChangeToAppear;

    public Tile_Profile GetRandomProfile()
    {
        return tilesList[Random.Range(0, tilesList.Count)];
    }
}
