using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/Basics/Start", fileName = "Tile_Start")]
public class Tile_Start : TileStateClass
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
