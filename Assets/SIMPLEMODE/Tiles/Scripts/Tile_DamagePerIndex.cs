using System.Collections;
using UnityEngine;

public class Tile_DamagePerIndex : Tile_Base
{
    [SerializeField] int damagePerIndex;
    public override IEnumerator OnPlayerStepped()
    {
        yield return GameController.Co_AddAcumulatedDamage(BoardController.TilesList.Count * damagePerIndex);
        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Deal {MathJ.AddDamage(damagePerIndex)} per tile in board";
    }
}
