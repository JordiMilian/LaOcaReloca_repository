using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Rose : Tile_Plant
{
    [SerializeField] int extraRollValue = 3;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override IEnumerator OnPlayerStepped() 
    {
        yield return base.OnPlayerStepped();
        GameController.dicesController.AddBoughtValue(extraRollValue);
    }
   public override string GetTooltipText() { return base.GetTooltipText() + $"\n{OnCrossed} Add +{extraRollValue} value to the next dice roll"; }
}