using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "TileProfile/Ocas/ElevatorOca", fileName = "Tile_ElevatorOca")]
public class Tile_ElevatorOca : Tile_Oca
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    


    public override IEnumerator OnPlayerLanded() 
    { 
        yield return basePlayerLanded();
        yield return BoardController.L_JumpPlayerTo(BoardController.TilesList.Count - 1, true);

    }
    public override string GetTooltipText() { return $"{OnLanded} Jump to the END TILE"; }
}