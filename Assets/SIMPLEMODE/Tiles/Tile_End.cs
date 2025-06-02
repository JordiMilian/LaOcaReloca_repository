using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_End : Tile_Base
{

    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        yield return GameController.AddAcumulatedDamage(50);
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override float GetLandedDamageAmount()
    {
        return 0;
    }
}
