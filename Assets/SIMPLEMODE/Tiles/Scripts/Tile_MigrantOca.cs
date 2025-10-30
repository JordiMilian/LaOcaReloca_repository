using UnityEngine;
using System.Collections;
public class Tile_MigrantOca : Tile_Oca
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    [SerializeField] int moneyPerTile = 1;
    public override IEnumerator OnPlayerLanded() 
   { 
        int startingIndex = Tile.indexInBoard;

        yield return basePlayerLanded();

        for (int i = Tile.indexInBoard + 1; i < BoardController.TilesList.Count; i++)
        {
            if (BoardController.TilesList[i]._Profile is Tile_Oca)
            {
                yield return BoardController.L_JumpPlayerTo(i, false);
                break;

            }
            if (BoardController.TilesList[i]._Profile is Tile_End)
            {
                yield return BoardController.L_JumpPlayerTo(i, true);
                break;
            }
        }

        int endIndex = BoardController.PlayerIndex;
        GameController.AddMoney((endIndex - startingIndex) * 1);
   }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnLanded} Jump to the next Oca and gain {moneyPerTile} money per tile skipped"; }


}