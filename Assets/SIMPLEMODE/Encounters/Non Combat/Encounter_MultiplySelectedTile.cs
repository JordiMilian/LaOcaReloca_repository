using System.Collections;
using UnityEngine;

public class Encounter_MultiplySelectedTile : Encounter_SelectedTileEffect
{
    [SerializeField] float multiplier = 1.5f;
    public override void OnSelectedTileAction(TileController selectedTile)
    {
        
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

    public override string GetMainButtonText()
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
