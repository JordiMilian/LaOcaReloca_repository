using System.Collections;
using UnityEngine;

public class Encounter_Gambling : MonoBehaviour, IEncounter
{
    [SerializeField] GameObject CanvasRoot;
    int currentBet;
    public IEnumerator OnEncounterEnter()
    {
        CanvasRoot.SetActive(false);
        Board_Controller_simple boardController = Board_Controller_simple.Instance;
        if(boardController.isBoardAssembled)
        {
            yield return boardController.C_DisasembleBoard();
        }
        currentBet = 0;
        CanvasRoot.SetActive(true);

    }

    public void Button_AddBet()
    {
        currentBet++;
    }
    public void Button_FinishBet()
    {

    }

    public IEnumerator OnEncounterExit()
    {
        throw new System.NotImplementedException();
    }
}
