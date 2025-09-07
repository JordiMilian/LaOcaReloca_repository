using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_LandForward : Tile_Profile
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.C_AddAcumulatedDamage(-GameController.GetCurrentAcumulatedDamage());
        yield return BoardController.L_JumpPlayerTo(Tile.indexInBoard + 1, true);
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} remove all current damage and LAND on the tile forward";
    }
}
