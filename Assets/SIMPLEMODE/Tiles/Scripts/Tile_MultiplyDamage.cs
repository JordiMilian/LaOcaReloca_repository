using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/Multiplier", fileName = "Tile_Multiplier")]
public class Tile_MultiplyDamage : Tile_Profile
{
    [SerializeField] float multiplierOnLanded = 4;
    [SerializeField] float multiplierOnStepped = 1.5f;
    public override IEnumerator OnPlayerLanded()
    {
        TileController randomTile = MathJ.GetRandomTileInBoard(Tile, true, true);
        randomTile.MultiplyBaseDamage(multiplierOnLanded);
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        TileController randomTile = MathJ.GetRandomTileInBoard(Tile, true, true);
        randomTile.MultiplyBaseDamage(multiplierOnStepped);

        yield return base.OnPlayerStepped();
    }

    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the dmg of a RANDOM TILE x{multiplierOnLanded} \n{OnCrossed} Multiply the dmg of a RANDOM TILE x{multiplierOnStepped}";
    }
}
