using System.Collections;
using UnityEngine;

public class Tile_MultiplyDamage : Tile_Base
{
    [SerializeField] float multiplierOnLanded = 4;
    [SerializeField] float multiplierOnStepped = 1.5f;
    public override IEnumerator OnPlayerLanded()
    {
        Tile_Base randomTile = MathJ.GetRandomTileInBoard(this, true, true);
        randomTile.MultiplyBaseDamage(multiplierOnLanded);
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        Tile_Base randomTile = MathJ.GetRandomTileInBoard(this, true, true);
        randomTile.MultiplyBaseDamage(multiplierOnStepped);

        yield return base.OnPlayerStepped();
    }

    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the dmg of a RANDOM TILE x{multiplierOnLanded} \n{OnCrossed} Multiply the dmg of a RANDOM TILE x{multiplierOnStepped}";
    }
}
