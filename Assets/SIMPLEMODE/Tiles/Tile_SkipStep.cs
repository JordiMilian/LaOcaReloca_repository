using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile_SkipStep : Tile_Base
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        GameController.remainingStepsToTake++;
    }
    public override string GetTooltipText()
    {
        return "On Crossed: Skip this step";
    }
}
