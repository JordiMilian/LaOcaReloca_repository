using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "TileProfile/LandForward", fileName = "Tile_LandForward")]
public class Tile_LandForward : TileInfo
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return GameController.C_AddAcumulatedDamage(-GameController.GetCurrentAcumulatedDamage());
        yield return BoardController.L_JumpPlayerTo(_Controller.indexInBoard + 1, true);
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} remove all current damage and LAND on the tile forward";
    }
}
