using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_LandForward : Tile_Base
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.AddAcumulatedDamage(-GameController.GetCurrentAcumulatedDamage());
        yield return BoardController.L_JumpPlayerTo(indexInBoard + 1, true);
    }
    public override string GetTooltipText()
    {
        return "On Landed: remove all current damage and LAND on the tile forward";
    }
}
