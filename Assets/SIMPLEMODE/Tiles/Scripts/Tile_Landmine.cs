using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/Landmine", fileName = "Tile_Landmine")]
public class Tile_Landmine : Tile_Profile
{
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Destroy the Tile forward and this Tile";
    }

    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        if (_Tile.indexInBoard < BoardController.TilesList.Count - 2) { BoardController.RemoveTile(_Tile.indexInBoard + 1); }
        BoardController.RemoveTile(_Tile.indexInBoard);
    }
}
