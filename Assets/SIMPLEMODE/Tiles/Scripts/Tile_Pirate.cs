using UnityEngine;
using System.Collections;
public class Tile_Pirate : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }


    
    [SerializeField] int moneyOnLanded = 2;
    public override IEnumerator OnPlayerStepped()
    {
        DamagesToDeal.Add(GameController.GetCurrentMoney());
        yield return base.OnPlayerStepped();
    }
    public override IEnumerator OnPlayerLanded()
    {
        GameController.AddMoney(moneyOnLanded);
        yield return base.OnPlayerLanded(); 
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Deal damage equal to your CURRENT MONEY\n{OnLanded} Gain {moneyOnLanded} coins";
    }

}
