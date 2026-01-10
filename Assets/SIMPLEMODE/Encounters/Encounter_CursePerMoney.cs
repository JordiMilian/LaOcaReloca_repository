using System.Collections;
using UnityEngine;

public class Encounter_CursePerMoney : MonoBehaviour, IEncounter, ITooltip
{

    Board_Controller_simple boardController;
    [SerializeField] FreePick freePick;
    [SerializeField] ProfilesGroup PG_Curses;
    TileController curseTile;
    [SerializeField] int moneyOnCurse = 15;
    [SerializeField] Texture Tooltip_texture;
    [SerializeField] string Tooltip_title;
    public IEnumerator OnEncounterEnter()
    {
        boardController = Board_Controller_simple.Instance;

        if (!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        TooltipManager.Instance.RequestTooltip(this);

        curseTile = freePick.SpawnTile(PG_Curses.GetRandomProfile());
        curseTile.OnAddedToBoard.AddListener(OnAddedCurseToBoard);

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(SkipEncounter);
        Dices_Controller.Instance.SetMainButtonText("SKIP");
        Dices_Controller.Instance.Button_Rolldices.interactable = true;
    }
    void OnAddedCurseToBoard()
    {
        curseTile.OnAddedToBoard.RemoveListener(OnAddedCurseToBoard);
        GameController_Simple.Instance.AddMoney(moneyOnCurse);
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
    }
    void SkipEncounter()
    {
        Destroy(curseTile.gameObject);
        GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
    }
    public IEnumerator OnEncounterExit()
    {
        TooltipManager.Instance.RemoveRequest(this);
        yield break;
    }
    public string GetTooltipDescription()
    {
        return $"Take this tile and get {moneyOnCurse} money";
    }

    public Texture GetTooltipTexture()
    {
        return Tooltip_texture;
    }

    public string GetTooltipTitle()
    {
        return Tooltip_title;
    }

    

}
