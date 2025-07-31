using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_End : Tile_Base
{
    [SerializeField] int moneyOnReached = 3;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        GameController.AddMoney(moneyOnReached);
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override string GetTooltipText()
    {
        return $"{OnSomething(On.OnReached)} Add {moneyOnReached} coins and return to Start";
    }
}
