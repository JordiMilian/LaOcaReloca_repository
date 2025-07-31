using System.Collections;
using UnityEngine;

public class Tile_DamageEveryThirdPass : Tile_Base
{
    [SerializeField] int timesCrossed;
    [SerializeField] int timesNeededToCross = 3;
    [SerializeField] float multiplyCurrentDamageBy = 3;
    public override IEnumerator OnPlayerStepped()
   {
        timesCrossed++;
        if(timesCrossed >= timesNeededToCross)
        {
            MultiplyCrossingDamage(multiplyCurrentDamageBy); ;
            timesCrossed = 0;
        }
        yield return base.OnPlayerStepped();
   }
    public override string GetTooltipText()
    {
        return $"On Crossed: Every {timesNeededToCross} times crossed, multiply this tile damage by {multiplyCurrentDamageBy}";
    }
}
