using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter_Boss_DisableLanded : Encounter_Boss
{
    public override void ActivateSpecialBossEffect()
    {
        base.ActivateSpecialBossEffect();
        gameController.OnFinishedRoll_CardEffects.AddEffect(C_DisableLandedTile);
    }
    IEnumerator C_DisableLandedTile()
    {
        TileController landedTile = gameController.BoardController.GetCurrentPlayerTile();

        if(landedTile._Info is Tile_Start ||  landedTile._Info is Tile_End) { yield break; }
        landedTile.tileMovement.shakeTile(Intensity.mid);
        landedTile._Info.DisableTile(true, true);
        yield return new WaitForSeconds(0.5f);
    }
    public override void DeactivateSpecialBossEffect()
    {
        base.DeactivateSpecialBossEffect();
        gameController.OnFinishedRoll_CardEffects.RemoveEffect(C_DisableLandedTile);
    }
    public override float GetBossHealth(float baseHP)
    {
        return baseHP * 1.5f;
    }

    public override string GetTooltipDescription()
    {
        return "Landed tiles get disabled(logic) and disabled(dmg)";
    }
}
