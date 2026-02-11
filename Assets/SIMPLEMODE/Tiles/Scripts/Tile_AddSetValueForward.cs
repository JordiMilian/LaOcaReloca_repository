using UnityEngine;
using System.Collections;

public class Tile_AddSetValueForward : Tile_Profile
{
    [SerializeField] int Amount = 5;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController nextTile = BoardController.TilesList[_Tile.indexInBoard + 1];
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
