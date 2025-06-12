using System.Collections;
using System.Linq;
using UnityEngine;

public class Tile_AddToEmptyTiles : Tile_Base
{
    [SerializeField] int addedDamageToEmptyTiles = 2;

    public override IEnumerator OnPlayerLanded()
    {
        foreach(Tile_Base tile in BoardController.TilesList)
        {
            if(tile.tileTags.Contains(TileTags.EmptyTile))
            {
                tile.SetDefaultCrossingDamage(tile.GetDefaultCrossedDamage() + addedDamageToEmptyTiles);
                tile.shakeTile(Intensity.large);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public override string GetTooltipText()
    {
        return $"On Landed: Add {addedDamageToEmptyTiles} damage to all empty tiles";
    }
}
