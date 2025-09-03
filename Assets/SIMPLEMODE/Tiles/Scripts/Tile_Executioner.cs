using UnityEngine;
using System.Collections;
public class Tile_Executioner : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() 
    { return $"{OnLanded} Destroy another random TILE"; }

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        Tile_Base randomTile = null;
        int randomIndex = 0;
        while(randomTile == null || randomTile is Tile_Start || randomTile is Tile_End || randomTile == this)
        {
            randomIndex = Random.Range(0, BoardController.TilesList.Count);
            randomTile = BoardController.TilesList[randomIndex];
        }
        BoardController.RemoveTile(randomIndex);

        
    }
}