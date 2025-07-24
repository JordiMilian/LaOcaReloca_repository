using System.Collections;
using UnityEngine;

public class Tile_ImparesHater : Tile_Base
{
    public override IEnumerator OnPlayerStepped()
    {
        if(GameController.dicesController.LastRolledValue%2 == 0)
        {
            SetDefaultCrossingDamage(GetDefaultCrossedDamage());
        }
        else 
        {
            SetDefaultCrossingDamage(-GetDefaultCrossedDamage());
        }
        shakeTile(Intensity.mid);
        yield return new WaitForSeconds(0.3f);

        yield return base.OnPlayerStepped();

    }
    public override string GetTooltipText()
    {
        return $"On Crossed: if last rolled value is ODD, add {GetDefaultCrossedDamage()}, else remove that amount";
    }
}
