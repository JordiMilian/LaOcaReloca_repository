using System.Collections;
using UnityEngine;

public class Tile_DamagePerIndex : Tile_Base
{
    [SerializeField] int damagePerIndex;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.Co_AddAcumulatedDamage(indexInBoard * damagePerIndex);
    }
    public override string GetTooltipText()
    {
        return $"On Landed: Add {damagePerIndex} damage per index of tile in board";
    }
}
