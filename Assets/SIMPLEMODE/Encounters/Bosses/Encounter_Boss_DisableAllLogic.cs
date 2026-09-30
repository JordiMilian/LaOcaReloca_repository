using System.Collections;
using UnityEngine;

public class Encounter_Boss_DisableAllLogic : Encounter_Boss
{
    
    IEnumerator C_DisableEnteredTile(TileController tileController)
    {
        tileController._Info.DisableTile(true, false);
        yield break;
    }
    public override void ActivateSpecialBossEffect()
    {
        base.ActivateSpecialBossEffect();
        Board_Controller_simple board = Board_Controller_simple.Instance;
        foreach (TileController tile in board.TilesList)
        {
            if(tile._Info is Tile_Start || tile._Info is Tile_End) { continue; }
            tile._Info.DisableTile(true, false);
        }
        GameController_Simple.Instance.OnAddedNewTileToBoard_CardEffect.AddEffect(C_DisableEnteredTile);
    }
    public override void DeactivateSpecialBossEffect()
    {
        base.DeactivateSpecialBossEffect();
        GameController_Simple.Instance.OnAddedNewTileToBoard_CardEffect.RemoveEffect(C_DisableEnteredTile);

    }
    public override float GetBossHealth(float baseHP)
    {
        return baseHP * 0.75f;
    }
    public override string GetTooltipDescription()
    {
        return "All tiles are disabled(logic)";
    }
}
