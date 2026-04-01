using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class TilesFactory : MonoBehaviour
{
    
    [SerializeField] GameObject EmptyPrefab;

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
    public TileController InstantiateTile(TileInfo info)
    {
        GameObject newTile = Instantiate(EmptyPrefab);

        TileController controller = newTile.GetComponent<TileController>();
        controller.SetTileProfile(info.GetCopy());

        return controller;
    }

}
