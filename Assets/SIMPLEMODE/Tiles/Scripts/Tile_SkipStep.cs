using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile_SkipStep : TileInfo
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        GameController.remainingStepsToTake++;
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Skip this step";
    }
}
