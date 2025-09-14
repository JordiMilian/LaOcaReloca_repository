using UnityEngine;
using System.Collections.Generic;

public class TilesFactory : MonoBehaviour
{
    [SerializeField] GameObject EmptyPrefab;
    public List<Tile_Profile> tileProfiles = new();

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
    public Tile_Profile GetRandomProfile() { return tileProfiles[Random.Range(0, tileProfiles.Count)]; }

}
