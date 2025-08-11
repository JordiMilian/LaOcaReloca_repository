using System.Collections;
using UnityEngine;

public class Tile_AddOwnValueForward : Tile_Base
{
    [SerializeField] float PercentageToAdd = 20;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        Tile_Base nextTile = BoardController.TilesList[indexInBoard + 1];
        if(nextTile != null)
        {
            nextTile.AddPermaDamage
                (GetDefaultCrossedDamage() * (PercentageToAdd /100));
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override string GetTooltipText()
    {
        return $"On Landed: Add {PercentageToAdd}% of this tile damage to the next tile";
    }
}
