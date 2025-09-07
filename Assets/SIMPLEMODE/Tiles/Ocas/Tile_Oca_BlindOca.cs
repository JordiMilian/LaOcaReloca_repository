using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile_Oca_BlindOca : Tile_Oca
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.basePlayerLanded(); //we call the base from two behind

        List<TileController> boardOcas = GetAllOcaTiles();

        //if there is only this oca, jump to end
        if(boardOcas.Count <= 1)
        {
            yield return BoardController.L_JumpPlayerTo(BoardController.TilesList.Count -1, false);
            GameController.AddMoney(GameController.MoneyToRoll);
            yield break;
        }


        int randomIndex;
        //make sure we dont land in the same Oca as this
        do
        {
            randomIndex = Random.Range(0, boardOcas.Count);
        }
        while (boardOcas[randomIndex] == Tile);

        
        yield return BoardController.L_JumpPlayerTo(boardOcas[randomIndex].indexInBoard, false);
        GameController.AddMoney(GameController.MoneyToRoll);

    }
    public List<TileController> GetAllOcaTiles()
    {
        List<TileController> ocaTiles = new();
        foreach (TileController tile in BoardController.TilesList)
        {
            if (tile._Profile is Tile_Oca) { ocaTiles.Add(tile); }
        }
        return ocaTiles;
    }
    public override string GetTooltipText()
    {
        return  $"{MathJ.BoldText("ON LANDED: ")} Jump to another random Oca and gain {GameController.MoneyToRoll} money";
    }
}
