using System.Collections;
using UnityEngine;

public class Encounter_MultiplySelectedTile : Encounter_SelectedTileEffect, IEncounter
{
    [SerializeField] float multiplier = 1.5f;
    public override void Button_OnMainButtonPressed()
    {
        
        TileController selectedTile = tileSelector.GetClosestTileInRange();
        if (selectedTile == null)
        {
            return;
        }
        StartCoroutine(C_mult());

        IEnumerator C_mult()
        {
            yield return selectedTile.C_MultiplyBaseDamage(multiplier);
            GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        }
       

        

    }

    public override string GetButtonText()
    {
        return "X "+ multiplier;
    }

    public override string GetTooltipDescription()
    {
        return "Multiply selected tile x" + multiplier;
    }

    public override bool MeetsRequirementsToSpawn()
    {
        return true;
    }

}
