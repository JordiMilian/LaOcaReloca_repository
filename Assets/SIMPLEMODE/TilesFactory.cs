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
    public TileController InstantiateTileCopy(TileInfo info)
    {
        GameObject newTile = Instantiate(EmptyPrefab);

        TileController controller = newTile.GetComponent<TileController>();
        controller.SetTileProfile(info.GetCopy());

        TileConfig thisTileConfig = ConfigsDatabase.GetTileConfigWithId(info._configId);
        controller.tileMaterial.SetTexture("_mainTexture", thisTileConfig._texture);

        return controller;
    }
    public TileController InstantiateTileFromConfig(TileConfig config)
    {
        GameObject newTileGO = Instantiate(EmptyPrefab);

        TileController controller = newTileGO.GetComponent<TileController>();
        controller.SetTileProfile(config._configInfo.GetCopy());
        controller.tileMaterial.SetTexture("_mainTexture", config._texture); //guarro i potser podem centralitzarho millor tot aixo. Esta molt dispers el setting de una tile nova
        controller._Info._configId = config.name;
        return controller;


        /*
        TileInfo newInfo = newTile._Info;
        newInfo._configId = config._configId;
        newInfo.tileTexture = config._texture;

        return newTile;
        */
    }

}
