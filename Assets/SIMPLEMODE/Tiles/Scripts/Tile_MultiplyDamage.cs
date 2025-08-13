using System.Collections;
using UnityEngine;

public class Tile_MultiplyDamage : Tile_Base
{
    [SerializeField] float multiplierAdded = 2;
    public override IEnumerator OnPlayerStepped()
    {
        
        yield return GameController.Co_AddAcumulatedMultiplier(GameController.GetCurrentAcumulatedDamage()); //multiply the current damage X2
        yield return base.OnPlayerStepped();
    } 

    public override string GetTooltipText()
    {
        return $"{On.OnCrossed} Add {MathJ.AddMultiplier(multiplierAdded)} multiplier";
    }
}
