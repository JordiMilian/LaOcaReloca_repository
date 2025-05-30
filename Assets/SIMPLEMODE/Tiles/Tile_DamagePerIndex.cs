using System.Collections;
using UnityEngine;

public class Tile_DamagePerIndex : Tile_Base
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.AddAcumulatedDamage(indexInBoard);
    }
}
