using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_RacoonToken : Tile_Profile
{
    [SerializeField] int moneyOnLanded = 1;
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped()
   { 
        yield return base.OnPlayerStepped();
        GameController.AddMoney(moneyOnLanded);
    }
   public override string GetTooltipText() { return base.GetTooltipText() + $"{OnCrossed} Get {moneyOnLanded} money"; }
}