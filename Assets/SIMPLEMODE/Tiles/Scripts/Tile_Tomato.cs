using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Tomato : Tile_Plant
{
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        _Tile.SetBaseDamage(0);
    }
   public override string GetTooltipText() { return base.GetTooltipText() + $"\n{OnCrossed} Return DMG to 0"; }
}