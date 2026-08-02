using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/Basics/Start", fileName = "Tile_Start")]
public class Tile_Start : TileInfo
{
    public override string GetTooltipText()
    {
        return $"{Peaceful}";
    }
    public override float AddBaseDamage(float addedDamage)
    {
        return 0;
    }
    public override TileInfo GetCopy()
    {
        Tile_Start newInfo = (Tile_Start)CopyBaseStatsIntoOther(new Tile_Start());
        return newInfo;
    }
}
