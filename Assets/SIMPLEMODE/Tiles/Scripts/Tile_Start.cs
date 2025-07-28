using UnityEngine;

public class Tile_Start : Tile_Base
{
    public override float GetCrossedDamageAmount()
    {
        return 0;
    }
    public override string GetTooltipText()
    {
        return "START";
    }
}
