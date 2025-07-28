using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile_Oca_JumpToRandom : Tile_Oca
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.basePlayerLanded(); //we call the base from two behind

        List<Tile_Oca> boardOcas = GetAllOcaTiles();

        int randomIndex;
        //make sure we dont land in the same Oca as this
        do
        {
            randomIndex = Random.Range(0, boardOcas.Count);
        }
        while (boardOcas[randomIndex].indexInBoard == indexInBoard);

        
        yield return BoardController.L_JumpPlayerTo(boardOcas[randomIndex].indexInBoard, false);
        GameController.AddMoney(GameController.MoneyToRoll);

    }
    public List<Tile_Oca> GetAllOcaTiles()
    {
        List<Tile_Oca> ocaTiles = new();
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile is Tile_Oca) { ocaTiles.Add(tile as Tile_Oca); }
        }
        return ocaTiles;
    }
    public override string GetTooltipText()
    {
        return  $"{MathJ.BoldText("ON LANDED: ")} Jump to another random Oca and gain {GameController.MoneyToRoll} money";
    }
}
