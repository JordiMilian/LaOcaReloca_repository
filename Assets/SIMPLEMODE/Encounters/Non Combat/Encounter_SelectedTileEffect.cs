using System.Collections;
using UnityEngine;

public abstract class Encounter_SelectedTileEffect : Encounter
{
    Board_Controller_simple boardController;
    [SerializeField] GameObject TileSelectoPrefab;
    GameObject TileSelectorInstanceGO;
    TilesSelector tileSelector;
   


    public override IEnumerator OnEncounterEnter()
    {
        yield return base.OnEncounterEnter();

        boardController = Board_Controller_simple.Instance;

        if (!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        TooltipManager.Instance.RequestTooltip(this);
        TileSelectorInstanceGO = Instantiate(TileSelectoPrefab, Vector3.zero, Quaternion.identity);
        tileSelector = TileSelectorInstanceGO.GetComponent<TilesSelector>();
        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(onButtonPressed);
        Dices_Controller.Instance.SetMainButtonText(GetMainButtonText());
        Dices_Controller.Instance.Button_Rolldices.interactable = true;

    }

    public override IEnumerator OnEncounterExit()
    {
        yield return base .OnEncounterExit();

        TooltipManager.Instance.RemoveRequest(this);
        Destroy(TileSelectorInstanceGO);
        yield break;
    }
    void onButtonPressed()
    {
        OnSelectedTileAction(tileSelector.GetClosestTileInRange());
    }
    public abstract void OnSelectedTileAction(TileController selectedTile);

    public abstract string GetMainButtonText();
}
