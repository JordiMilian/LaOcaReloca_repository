using System.Collections;
using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/Landmine", fileName = "Tile_Landmine")]
public class Tile_Landmine : TileStateClass
{
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Destroy the Tile forward";
    }
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        if (_Tile.indexInBoard < BoardController.TilesList.Count - 2) { yield return BoardController.C_RemoveTile(_Tile.indexInBoard + 1); }
    }
}
