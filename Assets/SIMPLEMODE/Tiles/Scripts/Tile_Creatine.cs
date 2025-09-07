using UnityEngine;
using System.Collections;
public class Tile_Creatine : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    [SerializeField] float multiplieValue = 3;
    public override float AddBaseDamage(float addedDamage)
    {
        float totalAddedDamage = addedDamage * multiplieValue;
        Tile.SetBaseDamage(BaseDamage + totalAddedDamage);
        return totalAddedDamage;
    }
    public override string GetTooltipText() { return $"{OnAddedDamage} Multiply x{multiplieValue} the added amount"; }
}