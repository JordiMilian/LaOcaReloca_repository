using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Shareholder : TileInfo
{
   public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnAddedMoney_CardEffects.AddEffect(OnAddedMoney);
    }
    IEnumerator OnAddedMoney(int amount)
    {
        yield return _Controller.AddBaseDamage(amount);
        yield break;
    }

   public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard(); 
        GameController.OnAddedMoney_CardEffects.RemoveEffect(OnAddedMoney);
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return StringTools.OnCustomMessaje("WHEN GETTING MONEY") + "Increase this tile DMG by that amount of money"; }
}