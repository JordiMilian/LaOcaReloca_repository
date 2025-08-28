using System.Collections;
using UnityEngine;

public class Encounter_Test : MonoBehaviour, IEncounter
{
    CamerasManager cameras;
    [SerializeField] GameObject canvasRoot;
    public IEnumerator OnEncounterEnter()
    {
        canvasRoot.SetActive(false);
        cameras = CamerasManager.instance;

        cameras.SetCameraPriority("CinemachineCamera_Board", 15);
        if(Board_Controller_simple.Instance.isBoardAssembled)
        {
            yield return Board_Controller_simple.Instance.C_DisasembleBoard();
        }

        canvasRoot.SetActive(true);
    }
    public void button_ReturnToFreeMode()
    {
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        canvasRoot.SetActive(false);
    }
    public IEnumerator OnEncounterExit()
    {

        cameras.SetCameraPriority("CinemachineCamera_Board", 0);
        yield break;
    }

}
