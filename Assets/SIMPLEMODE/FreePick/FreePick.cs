using UnityEngine;

public class FreePick : MonoBehaviour
{
    public TileController SpawnTile(Tile_Profile profile)
    {
        TileController instantiatedTile = TilesFactory.instance.InstantiateTile(profile);
        instantiatedTile.transform.position = transform.position;
        instantiatedTile.SetOriginTfData(new TileTfData(transform));
        instantiatedTile.SetToTfData();
        instantiatedTile.SetTileState(TileState.FreePick);
        return instantiatedTile;
    }
    public Toy_Controller SpawnToy(Toy_Profile profile)
    {
        Toy_Controller instantiatedToy = ToysManager.Instance.InstantiateToy(profile);
        instantiatedToy.transform.position = transform.position;
        instantiatedToy.originTf = transform;
        return instantiatedToy;
    }
}
