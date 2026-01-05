using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Encounter_End : MonoBehaviour, IEncounter
{
    [SerializeField] PlayableDirector timeline_Enter;
    [SerializeField] GameObject CanvasRoot;
    Dices_Controller dicesController;
    public IEnumerator OnEncounterEnter()
    {
        CanvasRoot.SetActive(false);
        CamerasManager.instance.SetCameraPriority("CinemachineCamera_Board", 99);
        timeline_Enter.Play();
        yield return new WaitForSeconds((float)timeline_Enter.duration);
        CanvasRoot.SetActive(true);

        dicesController = GameController_Simple.Instance.dicesController;

        dicesController.Button_Rolldices.onClick.AddListener(button_Restart);
        dicesController.Button_Rolldices.interactable = true;
        dicesController.SetMainButtonText("RESTART");

    }

    public IEnumerator OnEncounterExit()
    {
        CanvasRoot.SetActive(false);
        dicesController.Button_Rolldices.onClick.RemoveListener(button_Restart);
        CamerasManager.instance.SetCameraPriority("CinemachineCamera_Board", 0);
        yield break;
    }

    public void button_Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
