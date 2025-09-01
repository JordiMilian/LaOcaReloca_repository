using UnityEngine;
using System.Collections;
public class Tile_Oca_ToEnd : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    


    public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        yield return BoardController.L_JumpPlayerTo(BoardController.TilesList.Count - 1, true);

    }
    public override string GetTooltipText() { return $"{OnLanded} Jump to the END TILE"; }
}