using System.Collections;
using UnityEngine;

public class Encounter_GiveExtraDiceroll : Encounter_SelectedTileEffect
{
    public override void OnSelectedTileAction(TileController selectedTile)
    {
        if (selectedTile == null || selectedTile._Info is Tile_Start || selectedTile._Info is Tile_End)
        {
            return;
        }
        selectedTile._Info.AddGenericSkill(GenericSkills.ExtraDiceroll);

        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
   
    }

    public override string GetMainButtonText()
    {
        return "Give Tile +ExtraDiceroll";
    }

    public override string GetTooltipDescription()
    {
        return "Give selected tile +ExtraDiceroll";
    }

    public override bool MeetsRequirementsToSpawn()
    {
        return Board_Controller_simple.Instance.TilesList.Count > 2;
    }

}
