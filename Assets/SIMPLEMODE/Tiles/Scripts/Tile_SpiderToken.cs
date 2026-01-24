using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_SpiderToken : Tile_Profile
{
    [SerializeField] float poison = 10;
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        GameController.ApplyPoison(poison);

        GameController.remainingStepsToTake++;
        yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
    }
   public override string GetTooltipText() { return base.GetTooltipText()+$"{OnCrossed} Apply {poison} poison"; }
}