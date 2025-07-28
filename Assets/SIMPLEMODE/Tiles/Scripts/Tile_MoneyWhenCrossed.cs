using System.Collections;
using UnityEngine;

public class Tile_MoneyWhenCrossed : Tile_Base
{
    [SerializeField] int money;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        GameController.AddMoney(money);
    }
    public override string GetTooltipText()
    {
        return $"On Crossed: Gain {money} money";
    }
}
