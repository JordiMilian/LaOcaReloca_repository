using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "TileProfile/Basics/End", fileName = "Tile_End")]
public class Tile_End : Tile_Profile
{
    [SerializeField] int moneyOnReached = 3;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        yield return ReachedEnd();

    }

    IEnumerator ReachedEnd()
    {
        GameController.AddMoney(moneyOnReached);
        GameController.remainingStepsToTake++;
        yield return _Tile.C_DealAllDamageToDeal();
        yield return GameController.OnReachedEndTile_CardEffects.C_ActivateEffects();
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override string GetTooltipText()
    {
        return StringTools.ExtraDiceRoll + $"\n{OnReached} Add {moneyOnReached} coins and return to Start";
    }
}
