using System.Collections;
using UnityEngine;

public class Encounter_RollInRange : MonoBehaviour, IEncounter
{
    Dices_Controller dicesController;
    [SerializeField] int minValue = 5, maxValue = 10;
    [SerializeField] float AddedDamageOnWin = 100;
    [SerializeField] float RemoveDamageOnLost = 10;
    [SerializeField] int tilesAffectedOnWin = 1, tilesAffectedOnLost = 5;

    [SerializeField] GameObject CanvasRoot;
    public IEnumerator OnEncounterEnter()
    {
        CanvasRoot.SetActive(false);
        dicesController = Dices_Controller.Instance;
        Board_Controller_simple boardController = Board_Controller_simple.Instance;

        if(!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        CanvasRoot.SetActive(true);
        dicesController.Button_Rolldices.onClick.AddListener(Button_OnRollPressed);

        dicesController.EnableRollButton();
        dicesController.DisableAddExtraRollValueButton();
    }

    public void Button_OnRollPressed()
    {
        StartCoroutine(rollPressedCoroutine());

        IEnumerator rollPressedCoroutine()
        {
            
            yield return dicesController.RollDicesCoroutine();
            CanvasRoot.SetActive(false);

            if (dicesController.LastRolledValue <= maxValue && dicesController.LastRolledValue >= minValue)
            {
                for (int i = 0; i < tilesAffectedOnWin; i++)
                {
                    TileController randomTile = MathJ.GetRandomTileInBoard(null, false, true, false);
                    randomTile.AddBaseDamage(AddedDamageOnWin);
                }
            }
            else
            {
                for (int i = 0; i < tilesAffectedOnLost; i++)
                {
                    TileController randomTile = MathJ.GetRandomTileInBoard(null, false, true, false);
                    yield return randomTile.RemoveBaseDamage(RemoveDamageOnLost);
                }
            }
            yield return new WaitForSeconds(1f);
            GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        }
    }

    public IEnumerator OnEncounterExit()
    {
        
        dicesController.Button_Rolldices.onClick.RemoveListener(Button_OnRollPressed);
        yield break;
    }
}
