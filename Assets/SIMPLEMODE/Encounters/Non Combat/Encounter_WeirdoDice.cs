using System.Collections;
using UnityEngine;

public class Encounter_WeirdoDice : MonoBehaviour, IEncounter
{
    Dices_Controller dicesController;
    [SerializeField] GameObject Prefab_weirdDice;
    public bool MeetsRequirementsToSpawn()
    {
        dicesController = Dices_Controller.Instance;
        foreach(Dice dice in dicesController.availableDices)
        {
            if(dice.gameObject.name == Prefab_weirdDice.name) { return false; }
        }
        return true;
    }

    public IEnumerator OnEncounterEnter()
    {
        dicesController = Dices_Controller.Instance;
        dicesController.Button_Rolldices.onClick.AddListener(onClicked);
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        dicesController.SetMainButtonText("WEIRD DICE");
        yield break;
    }

    void onClicked()
    {
        dicesController.SpawnNewDice(Prefab_weirdDice);
        foreach (Dice dice in dicesController.availableDices)
        {
            if (dice.gameObject.name == "Dice_6F") 
            {
                dicesController.availableDices.Remove(dice);
                Destroy(dice);
            }
        }
    }

    public IEnumerator OnEncounterExit()
    {
        yield break;
    }
}
