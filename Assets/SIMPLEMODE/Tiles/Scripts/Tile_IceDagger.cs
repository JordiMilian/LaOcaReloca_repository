using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_IceDagger : Tile_Profile
{
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); yield return BoardController.C_RemoveTile(_Tile.indexInBoard); }
   public override string GetTooltipText() { return Fragile; }
}