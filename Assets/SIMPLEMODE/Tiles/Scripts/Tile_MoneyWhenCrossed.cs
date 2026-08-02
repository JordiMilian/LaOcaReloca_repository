using System.Collections;
using UnityEngine;

public class Tile_MoneyWhenCrossed : TileInfo
{
    [SerializeField] int money;
    public override TileInfo GetCopy()
    {
        Tile_MoneyWhenCrossed newInfo = (Tile_MoneyWhenCrossed)CopyBaseStatsIntoOther(new Tile_MoneyWhenCrossed());
        newInfo.money = money;
        return newInfo;
    }
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
