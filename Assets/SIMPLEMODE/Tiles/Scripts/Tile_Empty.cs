using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Basics/EmptyTile", fileName = "Tile_Empty")]
public class Tile_Empty : TileStateClass
{
    public override string GetTooltipText()
    {
        return "No effect";
    }
}
