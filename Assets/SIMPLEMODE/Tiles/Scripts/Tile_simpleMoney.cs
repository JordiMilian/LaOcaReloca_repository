using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_simpleMoney : Tile_Base
{
    [SerializeField] int moneyAmount = 10;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        GameController.AddMoney(moneyAmount);
    }
    public override string GetTooltipText()
    {
        return $"On Landed: Add {moneyAmount} money";
    }
}
