using UnityEngine;
using System.Collections;
public class Tile_RatToken : Tile_Profile
{
   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    {
        yield return base.OnPlayerStepped();
        yield return new WaitForSeconds(0.3f);

        GameController.remainingStepsToTake++;
        yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
    }
   public override string GetTooltipText() { return $"{OnCrossed} Deal damage, skip this tile and destroy itself"; }
}