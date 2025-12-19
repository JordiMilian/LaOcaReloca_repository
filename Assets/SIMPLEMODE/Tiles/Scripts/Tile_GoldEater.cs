using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_GoldEater : Tile_Profile
{
    [SerializeField] int moneyToRemove = 5;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); GameController.RemoveMoney(moneyToRemove); }
   public override string GetTooltipText() { return $"{OnCrossed} Loose {moneyToRemove} money"; }
}