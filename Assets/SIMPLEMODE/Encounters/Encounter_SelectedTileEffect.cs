using System.Collections;
using UnityEngine;

public abstract class Encounter_SelectedTileEffect : MonoBehaviour, IEncounter, ITooltip
{
    Board_Controller_simple boardController;
    [SerializeField] GameObject TileSelectoPrefab;
    GameObject TileSelectorInstanceGO;
    protected TilesSelector tileSelector;
    [SerializeField] Texture tooltipTexture;
    [SerializeField] string tooltipTitle;
    

    public Texture GetTooltipTexture()
    {
        return tooltipTexture;
    }

    public string GetTooltipTitle()
    {
        return tooltipTitle;
    }



    public IEnumerator OnEncounterEnter()
    {
        boardController = Board_Controller_simple.Instance;

        if (!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        TooltipManager.Instance.RequestTooltip(this);
        TileSelectorInstanceGO = Instantiate(TileSelectoPrefab, Vector3.zero, Quaternion.identity);
        tileSelector = TileSelectorInstanceGO.GetComponent<TilesSelector>();
        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(Button_OnMainButtonPressed);
        Dices_Controller.Instance.SetMainButtonText(GetButtonText());
        Dices_Controller.Instance.Button_Rolldices.interactable = true;

    }

    public IEnumerator OnEncounterExit()
    {
        TooltipManager.Instance.RemoveRequest(this);
        Destroy(TileSelectorInstanceGO);
        yield break;
    }
    public abstract void Button_OnMainButtonPressed();
    public abstract string GetButtonText();
    public abstract string GetTooltipDescription();
}
