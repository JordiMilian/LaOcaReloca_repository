using System.Collections;
using UnityEngine;

public class Encounter_CursePerMoney : Encounter
{

    Board_Controller_simple boardController;
    [SerializeField] FreePick freePick;
    [SerializeField] ProfilesGroup PG_Curses;
    TileController curseTile;
    [SerializeField] int moneyOnCurse = 15;
    [SerializeField] Texture Tooltip_texture;
    [SerializeField] string Tooltip_title;
    public override IEnumerator OnEncounterEnter()
    {
        yield return base.OnEncounterEnter();

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

    public override string GetTooltipDescription()
    {
        return $"Take this tile and get {moneyOnCurse} money";
    }

    public override bool MeetsRequirementsToSpawn()
    {
        if(GameController_Simple.Instance.GetCurrentMoney() < 15) { return true; }
        return false;
    }


}
