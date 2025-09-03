using UnityEngine;
using System.Collections;
public class Tile_Creatine : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    [SerializeField] float multiplieValue = 3;
    public override void AddBaseDamage(float addedDamage)
    {
        

        float totalAddedDamage = addedDamage * multiplieValue;
        SetBaseDamage(BaseDamage + totalAddedDamage);
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
    public override string GetTooltipText() { return $"{OnAddedDamage} Multiply x{multiplieValue} the added amount"; }
}