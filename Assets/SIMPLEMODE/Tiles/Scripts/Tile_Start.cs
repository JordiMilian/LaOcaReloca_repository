using UnityEngine;

public class Tile_Start : Tile_Base
{
    public override float GetBaseDamage()
    {
        return 0;
    }
    public override string GetTooltipText()
    {
        return "START";
    }
}
