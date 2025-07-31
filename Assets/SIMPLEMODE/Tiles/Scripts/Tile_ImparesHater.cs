using System.Collections;
using UnityEngine;

public class Tile_ImparesHater : Tile_Base
{
    //I dont like this tile, reconsider pls
    public override IEnumerator OnPlayerStepped()
    {
        if(GameController.dicesController.LastRolledValue%2 == 0)
        {
            AddDefaultCrossingDamage(10);
        }
        else 
        {
            AddDefaultCrossingDamage(-10);
        }
        yield return new WaitForSeconds(0.3f);

        yield return base.OnPlayerStepped();

    }
    public override string GetTooltipText()
    {
        return $"On Crossed: if last rolled value is ODD, add {GetDefaultCrossedDamage()}, else remove that amount";
    }
}
