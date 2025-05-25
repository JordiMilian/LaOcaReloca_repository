using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_SimpleDamage : Tile_Base
{
    [SerializeField] float damage = 50;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.AddAcumulatedDamage(damage);
        yield return BoardController.L_JumpPlayerTo(indexInBoard - 1, true);
    }
}
