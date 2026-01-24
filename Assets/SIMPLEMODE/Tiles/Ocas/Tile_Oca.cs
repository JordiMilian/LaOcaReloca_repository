using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static StringTools;

[CreateAssetMenu(menuName = "TileProfile/Ocas/Oca", fileName = "Tile_Oca")]
public class Tile_Oca : Tile_Profile
{
    
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return _Tile.C_DealAllDamageToDeal();

        for (int i = _Tile.indexInBoard + 1; i < BoardController.TilesList.Count; i++)
        {
            if (BoardController.TilesList[i]._Profile is Tile_Oca)
            {
                yield return BoardController.L_JumpPlayerTo(i, false);
                yield break;

            }
            if(BoardController.TilesList[i]._Profile is Tile_End)
            {
                yield return BoardController.L_JumpPlayerTo(i, false);
                yield break;
            }
        }
    }

    //use this in case you create an Oca that doesnt jump to the next Oca 
    public IEnumerator basePlayerLanded() { yield return base.OnPlayerLanded(); yield return _Tile.C_DealAllDamageToDeal(); } 
    public override string GetTooltipText()
    {
        string display = "X";
        if(GameController != null) { display = GameController.MoneyToRoll.ToString(); }
        return $"{OnLanded} jump to the next Oca.";
    }
}
