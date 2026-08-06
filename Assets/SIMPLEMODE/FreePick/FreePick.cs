using UnityEngine;

public class FreePick : MonoBehaviour
{
    public TileController SpawnTile(TileConfig info)
    {
        TileController instantiatedTile = TilesFactory.instance.InstantiateTileFromConfig(info);
        instantiatedTile.transform.position = transform.position;
        instantiatedTile.SetOriginTfData(new TileTfData(transform));
        instantiatedTile.SetToTfData();
        instantiatedTile.SetTileState(TileState.FreePick);
        return instantiatedTile;
    }
    public Toy_Controller SpawnToy(ToyConfig profile)
    {
        Toy_Controller instantiatedToy = ToysManager.Instance.InstantiateToyFromConfig(profile);
        instantiatedToy.transform.position = transform.position;
        instantiatedToy.originTf = transform;
        return instantiatedToy;
    }
}
