using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class TilesFactory : MonoBehaviour
{
    
    [SerializeField] GameObject EmptyPrefab;
    public List<TileStateClass> tileProfiles = new();//USELESS, WE USE PROFILE GROUPS NOW



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
    public TileController InstantiateTile(TileStateClass profile)
    {
        GameObject newTile = Instantiate(EmptyPrefab);

        TileController controller = newTile.GetComponent<TileController>();
        controller.SetTileProfile(profile);

        return controller;
    }
}
