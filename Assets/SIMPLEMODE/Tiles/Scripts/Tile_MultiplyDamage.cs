using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/Multiplier", fileName = "Tile_Multiplier")]
public class Tile_MultiplyDamage : TileInfo
{
    [SerializeField] float multiplierOnLanded = 4;
    [SerializeField] float multiplierOnStepped = 1.5f;
    public override IEnumerator OnPlayerLanded()
    {
        TileController randomTile = MathJ.GetRandomTileInBoard(_Controller, true, true);
        yield return randomTile.C_MultiplyBaseDamage(multiplierOnLanded);
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        TileController randomTile = MathJ.GetRandomTileInBoard(_Controller, true, true);
        yield return randomTile.C_MultiplyBaseDamage(multiplierOnStepped);

        yield return base.OnPlayerStepped();
    }
    public override TileInfo GetCopy()
    {
        Tile_MultiplyDamage newInfo = (Tile_MultiplyDamage)CopyBaseStatsIntoOther(new Tile_MultiplyDamage());
        newInfo.multiplierOnLanded = multiplierOnLanded;
        newInfo.multiplierOnStepped = multiplierOnStepped;
        return newInfo;
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the dmg of a RANDOM TILE x{multiplierOnLanded} \n{OnCrossed} Multiply the dmg of a RANDOM TILE x{multiplierOnStepped}";
    }
}
