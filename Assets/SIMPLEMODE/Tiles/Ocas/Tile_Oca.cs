using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_Oca : Tile_Base
{
    
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        Board_Controller_simple boardController = Board_Controller_simple.Instance;

        for (int i = indexInBoard + 1; i < boardController.TilesList.Count; i++)
        {
            if (boardController.TilesList[i] is Tile_Oca)
            {
                yield return boardController.L_JumpPlayerTo(i, false);
                yield break;
            }
            if(boardController.TilesList[i] is Tile_End)
            {
                yield return boardController.L_JumpPlayerTo(i, true);
                yield break;
            }
        }
    }
    public IEnumerator basePlayerLanded() { yield return base.OnPlayerLanded(); }
    public override float GetLandedDamageAmount()
    {
        return 0;
    }
    public override string GetTooltipText()
    {
        return "ON LANDED: jump to the next nearest Oca";
    }
    public override string GetTitleText()
    {
        return "OCA";
    }
}
