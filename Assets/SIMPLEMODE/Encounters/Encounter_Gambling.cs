using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Encounter_Gambling : MonoBehaviour, IEncounter, ITooltip
{
    [SerializeField] GameObject CanvasRoot;
    [SerializeField] GameObject DicePrefab;
    [SerializeField] Button button_AddBet;
    [SerializeField] PlayableDirector timeline_Enter, timeline_HideLost, timeline_HideWin;
    Dice dice;
    GameObject diceGO;
    int currentBet;
    bool hasWon = false;
    public IEnumerator OnEncounterEnter()
    {
        CanvasRoot.SetActive(false);
        Board_Controller_simple boardController = Board_Controller_simple.Instance;
        if(boardController.isBoardAssembled)
        {
            yield return boardController.C_DisasembleBoard();
        }

        CamerasManager cameras = CamerasManager.instance;
        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        timeline_Enter.Play();
        yield return new WaitForSeconds((float)timeline_Enter.duration);

        currentBet = 0;
        CanvasRoot.SetActive(true);

        diceGO = Instantiate(DicePrefab);
        dice = diceGO.GetComponent<Dice>();

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(Button_FinishBet);
        Dices_Controller.Instance.EnableRollButton();

        TooltipManager.Instance.ForceTooltip(this);
    }

    public void Button_AddBet()
    {
        currentBet++;
        GameController_Simple.Instance.RemoveMoney(1);
    }
    public void Button_FinishBet()
    {
        button_AddBet.interactable = false;

        StartCoroutine(rollCoinCoroutine());

        IEnumerator rollCoinCoroutine()
        {
            dice.RollDice();
            yield return new WaitForSeconds(1f);

            while (!dice.rb.IsSleeping()) { yield return null; }

            dice.UpdateFaceupValue();

            if(dice.FaceUpValue > 3)
            {
                GameController_Simple.Instance.AddMoney(currentBet * 2);
                hasWon = true;
            }
            else
            {
                hasWon = false;
            }

            yield return new WaitForSeconds(.5f);

            Destroy(diceGO);

            GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        }
    }

    public IEnumerator OnEncounterExit()
    {
        Dices_Controller.Instance.Button_Rolldices.onClick.RemoveListener(Button_FinishBet);
        TooltipManager.Instance.StopForcingThisTooltip(this);
        CanvasRoot.SetActive(false);

        CamerasManager cameras = CamerasManager.instance;

        if(hasWon)
        {
            timeline_HideWin.Play();
            yield return new WaitForSeconds((float)timeline_HideWin.duration);
        }
        else
        {
            timeline_HideLost.Play();
            yield return new WaitForSeconds((float)timeline_HideLost.duration);
        }
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);    
    }

    public string GetTooltipDescription()
    {
        return "Gambling is encouraged";
    }

    public string GetTooltipTitle()
    {
        return "GAMBLE ENCOUNTER";
    }
    public Texture GetTooltipTexture()
    {
        return null;
    }
}
