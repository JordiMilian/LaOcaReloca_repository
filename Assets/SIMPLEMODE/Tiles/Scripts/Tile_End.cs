using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "TileProfile/Basics/End", fileName = "Tile_End")]
public class Tile_End : TileStateClass
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        yield return ReachedEnd();

    }

    IEnumerator ReachedEnd()
    {
        yield return GameController.OnReachedEndTile_CardEffects.C_ActivateEffects();
        yield return base.OnPlayerLanded();
        yield return base.OnTileFinished();
        GameController_Simple.Instance.ChangeGameState(GameState.ReachedEnd);
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Return to Start";
    }
}
