using UnityEngine;
using System.Collections;
public class Tile_MoreDamageAdded : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    
    public override void AddPermaDamage(float addedDamage)
    {
        float totalAddedDamage = addedDamage * 2;
        SetDefaultCrossingDamage(defaultCrossedDamage + totalAddedDamage);
        tileMovement.shakeTile(Intensity.mid);
        if (totalAddedDamage >= 0)
        {
            tileMovement.DisplayMessage("+" + MathJ.FloatToString(totalAddedDamage, 1), TileMessageType.AddPermaDamage);
        }
        else
        {
            tileMovement.DisplayMessage(MathJ.FloatToString(totalAddedDamage, 1), TileMessageType.AddPermaDamage);
        }
    }
    public override string GetTooltipText() { return $"{OnAddedDamage} Doble the added amount"; }
}