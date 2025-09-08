using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "TileProfile/Executioner", fileName = "Tile_Executioner")]
public class Tile_Executioner : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() 
    { return $"{OnLanded} Destroy another random TILE"; }

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        if (BoardController.TilesList.Count == 3) { yield break; } //if its just this tile + Start + End

        TileController randomTile = null;
        int randomIndex = 0;
        while(randomTile == null || randomTile == this)
        {
            randomIndex = Random.Range(1, BoardController.TilesList.Count-1);
            randomTile = BoardController.TilesList[randomIndex];
        }
        BoardController.RemoveTile(randomIndex);

        
    }
}