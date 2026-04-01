using System.Collections;
using UnityEngine;

public class Encounter_Gentriifier : Encounter_SelectedTileEffect
{
    public override void Button_OnMainButtonPressed()
    {
        TileController controler = tileSelector.GetClosestTileInRange();
        if (controler == null) { return; }
        if(controler._Info is Tile_Start || controler._Info is Tile_End) { return; }

        TileInfo selectedTile =controler._Info;
        for (int i = selectedTile.genericSkills.Count -1 ; i >= 0; i--)
        {
            selectedTile.genericSkills.RemoveAt(i);
        }
        selectedTile.SetStepsToCross(1);
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
    }

    public override string GetButtonText()
    {
        return "GENTRIFY TILE";
    }

    public override string GetTooltipDescription()
    {
        return "Remove all generic skills from the selected tile and set its steps to 1";
    }
}
