using System.Collections;
using UnityEngine;

public class Tile_Feeder : Tile_Profile
{
    [SerializeField] float PercentageToAdd = 20;
    [SerializeField] float CrossedDamage = 5;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        TileController nextTile = BoardController.TilesList[Tile.indexInBoard + 1];
        if(nextTile != null)
        {
            nextTile.AddBaseDamage
                (BaseDamage * (PercentageToAdd /100));
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController nextTile = BoardController.TilesList[Tile.indexInBoard + 1];
        if (nextTile != null)
        {
            nextTile.AddBaseDamage(CrossedDamage);
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add {PercentageToAdd}% of this tile damage to the next tile \n{OnCrossed} Add {MathJ.AddDamage(CrossedDamage)} forward";
    }
}
