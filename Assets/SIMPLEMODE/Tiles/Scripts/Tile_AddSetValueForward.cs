using UnityEngine;
using System.Collections;

public class Tile_AddSetValueForward : TileInfo
{
    [SerializeField] int Amount = 5;
    public override TileInfo GetCopy()
    {
        Tile_AddSetValueForward newInfo = (Tile_AddSetValueForward)CopyBaseStatsIntoOther(new Tile_AddSetValueForward());
        newInfo.Amount = Amount;
        return newInfo;
    }
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController nextTile = BoardController.TilesList[_Controller.indexInBoard + 1];
        if (nextTile != null)
        {
            yield return nextTile.AddBaseDamage(Amount);
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override string GetTooltipText()
    {
        return $"On Stepped: Add {Amount} damage to the next tile";
    }
}
