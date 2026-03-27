using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "TileProfile/Money/SimpleMoney", fileName = "Tile_SimpleMoney")]
public class Tile_simpleMoney : TileInfo
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
        return $"{OnCrossed} Get {steppedAmount} money\n{OnLanded} Get {landedAmount} money ";
    }
}
