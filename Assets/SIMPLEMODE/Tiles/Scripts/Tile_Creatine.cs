using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Creatine", fileName = "Tile_Creatine")]
public class Tile_Creatine : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    [SerializeField] float multiplieValue = 3;
    public override float AddBaseDamage(float addedDamage)
    {
        float totalAddedDamage = addedDamage * multiplieValue;
        _Controller.SetBaseDamage(BaseDamage + totalAddedDamage);
        return totalAddedDamage;
    }
    public override string GetTooltipText() { return $"{OnAddedDamage} Multiply x{multiplieValue} the added amount"; }
}