using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_End : Tile_Base
{
    [SerializeField] int moneyOnReached = 3;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        yield return ReachedEnd();

    }
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return ReachedEnd();
    }

    IEnumerator ReachedEnd()
    {
        GameController.AddMoney(moneyOnReached);
        yield return C_DealAllDamageToDeal();
        yield return GameController.OnReachedEndTile_CardEffects.C_ActivateEffects();
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override string GetTooltipText()
    {
        return $"{OnReached} Add {moneyOnReached} coins and return to Start";
    }
}
