using UnityEngine;
using System.Collections;
using static StringTools;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_SlowingPotion : TileInfo
{
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    {
        yield return base.OnPlayerStepped();
        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Controller, 1, true, true);

        foreach(TileController adjacentTile in adjacentTiles)
        {
            adjacentTile._Info.SetStepsToCross(adjacentTile._Info.StepsToCross +1);
        }
    }
   public override string GetTooltipText() { return $"{OnCrossed} Adjacent Tiles take +1 steps to cross"; }
    public override TileInfo GetCopy()
    {
        Tile_SlowingPotion newInfo = (Tile_SlowingPotion)CopyBaseStatsIntoOther(new Tile_SlowingPotion());
        return newInfo;
    }
}