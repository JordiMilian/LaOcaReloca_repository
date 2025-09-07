using UnityEngine;
using System.Collections;
using System;
public class Tile_Pessimist : Tile_Profile
{


    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }

    [SerializeField] float multiplierOnEnd = 2;
    public override void OnPlacedInBoard() 
    {
        base.OnPlacedInBoard();
        GameController.OnReachedEndTile_CardEffects.AddEffect(OnReachedEndEffect) ;
    }
    public override void OnRemovedFromBoard()
    {
        base.OnRemovedFromBoard();
        GameController.OnReachedEndTile_CardEffects.RemoveEffect(OnReachedEndEffect);

    }
    IEnumerator OnReachedEndEffect()
    {
        tileMovement.shakeTile(Intensity.mid);
        Tile.DamagesToDeal.Add(BaseDamage * multiplierOnEnd);
        yield return Tile.C_DealAllDamageToDeal(); 
    }
    public override string GetTooltipText()
    {
        return $"{OnReachedEnd} Deal this tile dmg x{multiplierOnEnd}";
    }
}