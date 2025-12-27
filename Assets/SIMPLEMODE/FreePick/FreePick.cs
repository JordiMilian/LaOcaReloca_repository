using UnityEngine;

public class FreePick : MonoBehaviour
{
    public void SpawnTile(Tile_Profile profile)
    {
        TileController instantiatedTile = TilesFactory.instance.InstantiateTile(profile);
        instantiatedTile.transform.position = transform.position;
        instantiatedTile.SetOriginTfData(new TileTfData(transform));
        instantiatedTile.SetToTfData();
        instantiatedTile.SetTileState(TileState.FreePick);
    }
    public void SpawnToy(Toy_Profile profile)
    {
        Toy_Controller instantiatedToy = ToysManager.Instance.InstantiateToy(profile);
        instantiatedToy.transform.position = transform.position;
        instantiatedToy.originTf = transform;
    }
}
