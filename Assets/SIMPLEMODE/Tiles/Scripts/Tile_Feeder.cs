using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Feeder", fileName = "Tile_Feeder")]
public class Tile_Feeder : TileStateClass
{
    [SerializeField] float PercentageToAdd = 20;
    [SerializeField] float CrossedDamage = 5;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        TileController nextTile = BoardController.TilesList[_Tile.indexInBoard + 1];
        if(nextTile != null)
        {
            yield return nextTile.AddBaseDamage
                (BaseDamage * (PercentageToAdd /100));
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController nextTile = BoardController.TilesList[_Tile.indexInBoard + 1];
        if (nextTile != null)
        {
            yield return nextTile.AddBaseDamage(CrossedDamage);
            yield return new WaitForSeconds(0.3f);
        }
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Increase by {PercentageToAdd}% of this tile damage the tile forward \n{OnCrossed} Increase {MathJ.AddDamage(CrossedDamage)} the tile forward";
    }
}
