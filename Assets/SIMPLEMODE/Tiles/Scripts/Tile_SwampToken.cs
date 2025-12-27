using UnityEngine;
using System.Collections;
public class Tile_SwampToken : Tile_Profile
{
   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        if(GameController.remainingStepsToTake > 1)
        {
            GameController.remainingStepsToTake--;
            yield return BoardController.V_JumpPlayerToNewPos();
        }
    }
   public override string GetTooltipText() { return $"{StringTools.Unmovable}\nTakes 2 steps to cross"; }
}