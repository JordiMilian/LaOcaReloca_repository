using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/Landmine", fileName = "Tile_Landmine")]
public class Tile_Landmine : Tile_Profile
{
    public override string GetTooltipText()
    {
        return $"{OnLanded} Destroy the Tile forward and this Tile";
    }

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        if (Tile.indexInBoard < BoardController.TilesList.Count - 2) { BoardController.RemoveTile(Tile.indexInBoard + 1); }
        BoardController.RemoveTile(Tile.indexInBoard);
    }
}
