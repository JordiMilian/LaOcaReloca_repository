using System.Collections;
using UnityEngine;

public class Encounter_TileBuyer : Encounter_SelectedTileEffect
{
    [SerializeField] GameObject CanvasRoot;
    Board_Controller_simple boardController;

    public override string GetMainButtonText()
    {
        return "SELL TILE";
    }

    public override string GetTooltipDescription()
    {
        return "Place the finger over a tile to sell it and gain money equal to its base damage.";
    }

    public override bool MeetsRequirementsToSpawn() { if (Board_Controller_simple.Instance.TilesList.Count > 3) { return true; } else return false; }
    public override IEnumerator OnEncounterEnter()
    {
        yield return base.OnEncounterEnter();

        CanvasRoot.SetActive(false);
        boardController = Board_Controller_simple.Instance;

        if (!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        CanvasRoot.SetActive(true);
    }

    public override IEnumerator OnEncounterExit()
    {
        yield return base.OnEncounterExit();

        CanvasRoot.SetActive(false);
        yield break;
    }
    public override void OnSelectedTileAction(TileController soldTile)
    {
        StartCoroutine(C_sellButtonPressed());

        //
        IEnumerator C_sellButtonPressed()
        {
            if (soldTile == null || soldTile._Info is Tile_Start || soldTile._Info is Tile_End)
            {
                yield break;
            }

            Dices_Controller.Instance.Button_Rolldices.interactable = false;
            CanvasRoot.SetActive(false);

            GameController_Simple.Instance.AddMoney((int)soldTile.GetBaseDamage());

            yield return boardController.C_RemoveTile(soldTile.indexInBoard);

            GameController_Simple.Instance.ChangeGameState(GameState.EncountersTransition);
        }
    }
}
