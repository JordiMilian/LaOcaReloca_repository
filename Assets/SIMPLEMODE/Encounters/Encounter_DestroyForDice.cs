using System.Collections;
using UnityEngine;

public class Encounter_DestroyForDice : Encounter_SelectedTileEffect, IEncounter
{
    public override void Button_OnMainButtonPressed()
    {
        throw new System.NotImplementedException();
    }

    public override string GetButtonText()
    {
        throw new System.NotImplementedException();
    }

    public override string GetTooltipDescription()
    {
        throw new System.NotImplementedException();
    }


    public override bool MeetsRequirementsToSpawn()
    {
        return true;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
