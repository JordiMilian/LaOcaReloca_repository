using UnityEngine;
using System.Collections;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/Ocas/ElevatorOca", fileName = "Tile_ElevatorOca")]
public class Tile_ElevatorOca : Tile_Oca
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    
    public override IEnumerator OnPlayerLanded() 
    { 
        yield return basePlayerLanded();
        GameController.SetRemainingRolls(GameController.RollsRemaining + 1);
        yield return BoardController.L_JumpPlayerTo(BoardController.TilesList.Count - 1, false);
    }
    public override string GetTooltipText() { return $"{OnLanded} Jump to the END TILE"; }
}