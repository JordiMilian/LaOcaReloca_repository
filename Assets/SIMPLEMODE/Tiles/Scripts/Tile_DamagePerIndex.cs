using System.Collections;
using UnityEngine;

public class Tile_DamagePerIndex : Tile_Base
{
    [SerializeField] int damagePerIndex;
    public override IEnumerator OnPlayerStepped()
    {
        yield return GameController.Co_AddAcumulatedMultiplier(BoardController.TilesList.Count * damagePerIndex);
        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{ On.OnLanded} Add {MathJ.AddMultiplier(damagePerIndex)} multiplier per tile in board";
    }
}
