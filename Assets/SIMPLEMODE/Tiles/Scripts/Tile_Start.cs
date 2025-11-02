using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/Basics/Start", fileName = "Tile_Start")]
public class Tile_Start : Tile_Profile
{
    public override string GetTooltipText()
    {
        return $"{Peaceful}";
    }
    public override float AddBaseDamage(float addedDamage)
    {
        return 0;
    }
}
