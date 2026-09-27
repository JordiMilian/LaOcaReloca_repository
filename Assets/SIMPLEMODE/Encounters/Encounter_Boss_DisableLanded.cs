using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter_Boss_DisableLanded : Encounter_Boss
{
    List<TileController> tilesDisabledLogic = new();
    List<TileController> tilesDisabledDmg = new();
    public override void ActivateSpecialBossEffect()
    {
        base.ActivateSpecialBossEffect();
        gameController.OnLanded_CardEffects.AddEffect(C_DisableLandedTile);
    }
    IEnumerator C_DisableLandedTile(TileController landedTile)
    {
        landedTile.tileMovement.shakeTile(Intensity.mid);

        if (!landedTile._Info.genericSkills.Contains(GenericSkills.DisabledLogic)) 
        { 
            landedTile._Info.AddGenericSkill(GenericSkills.DisabledLogic);
            tilesDisabledLogic.Add(landedTile);
        }
        if (!landedTile._Info.genericSkills.Contains(GenericSkills.DisabledDmg))
        {
            landedTile._Info.AddGenericSkill(GenericSkills.DisabledDmg);
            tilesDisabledDmg.Add(landedTile);
        }
        yield return new WaitForSeconds(0.5f);
    }
    public override void DeactivateSpecialBossEffect()
    {
        base.DeactivateSpecialBossEffect();
        gameController.OnLanded_CardEffects.RemoveEffect(C_DisableLandedTile);

        foreach(TileController t in tilesDisabledLogic)
        {
            t._Info.RemoveGenericSkill(GenericSkills.DisabledLogic);
        }
        foreach(TileController t in tilesDisabledDmg)
        {
            t._Info.RemoveGenericSkill(GenericSkills.DisabledDmg);
        }
    }
}
