using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_simpleMoney : Tile_Base
{
    [SerializeField] int landedAmount = 10;
    [SerializeField] int steppedAmount = 1;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        GameController.AddMoney(steppedAmount);
    }
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        GameController.AddMoney(landedAmount);
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Add {steppedAmount} money\n{OnLanded} Add {landedAmount} money ";
    }
}
