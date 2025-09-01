using UnityEngine;
using System.Collections;
using System;
public class Tile_ReachedEndAddDamage : Tile_Base
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
        yield return GameController.Co_AddAcumulatedDamage(defaultCrossedDamage * multiplierOnEnd);
        yield break;
    }
    public override string GetTooltipText()
    {
        return $"{OnReachedEnd} Deal this tile dmg x{multiplierOnEnd}";
    }
}