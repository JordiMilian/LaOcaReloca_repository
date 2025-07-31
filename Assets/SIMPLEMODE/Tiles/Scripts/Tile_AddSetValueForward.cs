using UnityEngine;
using System.Collections;

public class Tile_AddSetValueForward : Tile_Base
{
    [SerializeField] int Amount = 5;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        Tile_Base nextTile = BoardController.TilesList[indexInBoard + 1];
        if (nextTile != null)
        {
            nextTile.AddDefaultCrossingDamage(Amount);
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override string GetTooltipText()
    {
        return $"On Stepped: Add {Amount} damage to the next tile";
    }
}
