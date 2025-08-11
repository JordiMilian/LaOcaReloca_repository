using System.Collections;
using UnityEngine;

public class Tile_MultiplyDamage : Tile_Base
{

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.Co_AddAcumulatedDamage(GameController.GetCurrentAcumulatedDamage()); //multiply the current damage X2
    }
    public override string GetTooltipText()
    {
        return "On Landed: multiply the current damage by 2";
    }
}
