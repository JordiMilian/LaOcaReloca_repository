using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Basics/EmptyTile", fileName = "Tile_Empty")]
public class Tile_Empty : TileInfo
{
    public override string GetTooltipText()
    {
        return "No effect";
    }
}
