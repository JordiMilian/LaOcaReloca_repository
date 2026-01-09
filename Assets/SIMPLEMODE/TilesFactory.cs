using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class TilesFactory : MonoBehaviour
{
    
    [SerializeField] GameObject EmptyPrefab;
    public List<Tile_Profile> tileProfiles = new();
    public ProfilesGroup[] ProfileGroups;



    public static TilesFactory instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public TileController InstantiateTile(Tile_Profile profile)
    {
        GameObject newTile = Instantiate(EmptyPrefab);

        TileController controller = newTile.GetComponent<TileController>();
        controller.SetTileProfile(profile);

        return controller;
    }

    public TileController InstantiateRandomTile()
    {
        return InstantiateTile(tileProfiles[Random.Range(0, tileProfiles.Count - 1)]);
    }
    public Tile_Profile GetRandomProfile(string[] ignoreGroups) 
    {
        float totalChance = 0;
        //Add up all the chances
        foreach(ProfilesGroup group in ProfileGroups) 
        { 
            if (ignoreGroups.Contains(group.GroupName)){ continue; } 
            totalChance += group.ChangeToAppear; }

        float randomChance = Random.Range(0, totalChance);
        float counting = 0;
        foreach( ProfilesGroup group in ProfileGroups )
        {
            if (ignoreGroups.Contains(group.GroupName)) { continue; }
            float prev = counting;
            counting += group.ChangeToAppear;
            if(randomChance <= counting && randomChance > prev)
            {
                return group.GetRandomProfile();
            }
        }
        return null;
        return tileProfiles[Random.Range(0, tileProfiles.Count)];
    }

}
