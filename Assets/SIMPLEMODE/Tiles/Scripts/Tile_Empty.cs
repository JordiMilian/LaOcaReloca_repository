using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Basics/EmptyTile", fileName = "Tile_Empty")]
public class Tile_Empty : Tile_Profile
{
    public override string GetTooltipText()
    {
        return "No effect";
    }
}
