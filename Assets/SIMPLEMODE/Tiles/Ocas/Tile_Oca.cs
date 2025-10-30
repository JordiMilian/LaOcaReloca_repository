using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "TileProfile/Ocas/Oca", fileName = "Tile_Oca")]
public class Tile_Oca : Tile_Profile
{
    
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        for (int i = _Tile.indexInBoard + 1; i < BoardController.TilesList.Count; i++)
        {
            if (base.BoardController.TilesList[i]._Profile is Tile_Oca)
            {
                yield return BoardController.L_JumpPlayerTo(i, false);
                GameController.AddMoney(GameController.MoneyToRoll);
                yield break;

            }
            if(BoardController.TilesList[i]._Profile is Tile_End)
            {
                yield return BoardController.L_JumpPlayerTo(i, true);
                yield break;
            }
        }
    }
    public IEnumerator basePlayerLanded() { yield return base.OnPlayerLanded(); } //use this in case you create an Oca that doesnt jump to the next Oca 
    public override string GetTooltipText()
    {
        string display = "X";
        if(GameController != null) { display = GameController.MoneyToRoll.ToString(); }
        return $"{MathJ.BoldText("ON LANDED: ")}jump to the next Oca and gain {display} money";
    }
}
