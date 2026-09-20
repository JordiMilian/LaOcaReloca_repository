using System.Collections;
using UnityEngine;

public class Encounter_Boss_RandomMover : Encounter_Boss
{
    public override void ActivateSpecialBossEffect()
    {
        base.ActivateSpecialBossEffect();
        gameController.OnFinishedRoll_CardEffects.AddEffect(C_jumpPlayerToRandomPos);
    }
    IEnumerator C_jumpPlayerToRandomPos()
    {
        int randomIndex = Random.Range(1, gameController.BoardController.TilesList.Count - 1);
        while (randomIndex == gameController.BoardController.PlayerIndex)
        {
            randomIndex = Random.Range(1, gameController.BoardController.TilesList.Count - 1);
        }
        
        yield return gameController.BoardController.L_JumpPlayerTo(randomIndex, false, false);
    }
    public override void DeactivateSpecialBossEffect()
    {
        base.DeactivateSpecialBossEffect();
        gameController.OnFinishedRoll_CardEffects.RemoveEffect(C_jumpPlayerToRandomPos);
    }
}
