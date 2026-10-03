using System.Collections;
using UnityEngine;

public class Encounter_WeirdoDice : Encounter
{
    Dices_Controller dicesController;
    [SerializeField] GameObject Prefab_weirdDice;

    public override string GetTooltipDescription()
    {
        return "Do you want a weird dice??";
    }

    //HOW CAN I SKIP??? TO DO
    public override bool MeetsRequirementsToSpawn()
    {
        dicesController = Dices_Controller.Instance;
        foreach(Dice dice in dicesController.availableDices)
        {
            if(dice.gameObject.name == Prefab_weirdDice.name) { return false; }
        }
        return true;
    }

    public override IEnumerator OnEncounterEnter()
    {
        yield return base.OnEncounterEnter();
        Board_Controller_simple board = Board_Controller_simple.Instance;

        if (board.isBoardAssembled)
        {
            yield return board.C_DisasembleBoard();
        }

        dicesController = Dices_Controller.Instance;
        dicesController.Button_Rolldices.onClick.AddListener(onClicked);
       
        dicesController.SetMainButtonText("WEIRD DICE");
        dicesController.Button_Rolldices.interactable = true;
    }

    void onClicked()
    {
        dicesController.SpawnNewDice(Prefab_weirdDice);
        for (int i = dicesController.availableDices.Count -1; i >= 0; i--)
        {
            Dice dice = dicesController.availableDices[i];
            if (dice.gameObject.name == "Dice_6F")
            {
                dicesController.availableDices.RemoveAt(i);
                Destroy(dice.gameObject);
            }
        }
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
    }
    void onSkiped()
    {
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
    }

}
