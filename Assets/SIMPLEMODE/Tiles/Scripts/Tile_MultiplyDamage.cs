using System.Collections;
using UnityEngine;

public class Tile_MultiplyDamage : Tile_Base
{
    [SerializeField] int multiplier = 2;
    public override IEnumerator OnPlayerLanded()
    {
        Tile_Base randomTile = MathJ.GetRandomTileInBoard(this, true, true);
        randomTile.MultiplyPermaDamage(multiplier);
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        for (int i = 0; i < multiplier - 1; i++)
        {
            yield return GameController.Co_AddAcumulatedDamage(defaultCrossedDamage * (multiplier - 1));
        }
        
        yield return base.OnPlayerStepped();
    }

    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the dmg of a RANDOM TILE x{multiplier} \n{OnCrossed} Deal this tile damage x{multiplier}";
    }
}
