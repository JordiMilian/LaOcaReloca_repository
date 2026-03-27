using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(menuName = "TileProfile/EndSynergy/Pessimiest", fileName = "Tile_Pessimist")]
public class Tile_Pessimist : TileInfo
{
   
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }

    [SerializeField] float multiplierOnEnd = 2;
    [SerializeField] float extraDamageOnLanded = .5f;
    public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnReachedEndTile_CardEffects.AddEffect(OnReachedEndEffect) ;
    }
    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnReachedEndTile_CardEffects.RemoveEffect(OnReachedEndEffect);

    }
    IEnumerator OnReachedEndEffect()
    {
        tileMovement.shakeTile(Intensity.mid);
        _Controller.DamagesToDeal.Add(BaseDamage * multiplierOnEnd);
        yield return _Controller.C_DealAllDamageToDeal(); 
    }
    public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        multiplierOnEnd += extraDamageOnLanded;
    }
    public override string GetTooltipText()
    {
        return $"{OnReachedEnd} Deal this tile dmg x{multiplierOnEnd} \n {OnLanded} Increase that amount by +{extraDamageOnLanded}";
    }
}