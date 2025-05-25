using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile_Oca_JumpToRandom : Tile_Oca
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.basePlayerLanded(); //we call the base from two behind

        List<Tile_Oca> boardOcas = BoardController.GetAllOcaTiles();

        int randomIndex;
        //make sure we dont land in the same Oca as this
        do
        {
            randomIndex = Random.Range(0, boardOcas.Count);
        }
        while (boardOcas[randomIndex].indexInBoard != indexInBoard);

       
        yield return BoardController.L_JumpPlayerTo(boardOcas[randomIndex].indexInBoard, false);
        
    }
}
