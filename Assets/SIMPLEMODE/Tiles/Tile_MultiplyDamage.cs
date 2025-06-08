using System.Collections;
using UnityEngine;

public class Tile_MultiplyDamage : Tile_Base
{

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.AddAcumulatedDamage(GameController.GetCurrentAcumulatedDamage()); //multiply the current damage X2
    }
}
