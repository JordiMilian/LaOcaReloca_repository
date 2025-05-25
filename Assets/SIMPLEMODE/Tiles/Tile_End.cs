using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_End : Tile_Base
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override float GetLandedDamageAmount()
    {
        return 100;
    }
}
