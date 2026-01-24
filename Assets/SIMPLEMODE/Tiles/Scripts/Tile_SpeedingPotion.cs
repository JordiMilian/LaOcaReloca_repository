using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StringTools;
public class Tile_SpeedingPotion : Tile_Profile
{
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    {
        yield return base.OnPlayerStepped();
        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Tile, 1, true, true);

        foreach (TileController adjacentTile in adjacentTiles)
        {
            adjacentTile._Profile.SetStepsToCross(0);
        }
    }
   public override string GetTooltipText() { return $"{OnCrossed} Adjacent Tiles take 0 steps to cross"; }
}