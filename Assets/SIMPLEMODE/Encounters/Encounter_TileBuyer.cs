using System.Collections;
using UnityEngine;

public class Encounter_TileBuyer : MonoBehaviour, IEncounter, ITooltip
{
    [SerializeField] GameObject CanvasRoot;
    Board_Controller_simple boardController;
    [SerializeField] GameObject TileSelectoPrefab ;
     GameObject  TileSelectorInstanceGO;
    TilesSelector tileSelector;
    [SerializeField] Texture tooltipTexture;
    public string GetTooltipDescription()
    {
        return "Place the finger over a tile to sell it and gain money equal to its base damage.";
    }

    public Texture GetTooltipTexture()
    {
        return tooltipTexture;
    }

    public string GetTooltipTitle()
    {
        return "TILES BUYER";
    }



    public IEnumerator OnEncounterEnter()
    {
        CanvasRoot.SetActive(false);
        boardController = Board_Controller_simple.Instance;

        if (!boardController.isBoardAssembled)
        {
            yield return boardController.C_AsembleBoard();
        }
        TooltipManager.Instance.RequestTooltip(this);
        TileSelectorInstanceGO = Instantiate(TileSelectoPrefab, Vector3.zero, Quaternion.identity);
        tileSelector = TileSelectorInstanceGO.GetComponent<TilesSelector>();
        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(Button_OnSellPressed);
        Dices_Controller.Instance.SetMainButtonText("Sell selected tile");
        Dices_Controller.Instance.Button_Rolldices.interactable = true;

        CanvasRoot.SetActive(true);

        
    }

    public IEnumerator OnEncounterExit()
    {
        TooltipManager.Instance.RemoveRequest(this);
        Destroy(TileSelectorInstanceGO);
        CanvasRoot.SetActive(false);
        yield break;
    }
    void Button_OnSellPressed()
    {
       
        StartCoroutine(C_sellButtonPressed());

        //
        IEnumerator C_sellButtonPressed()
        {
            if (boardController.TilesList.Count <= 2)
            {
                yield break;
            }
            TileController soldTile = tileSelector.GetClosestTileInRange();
            if(soldTile == null || soldTile._Profile is Tile_Start || soldTile._Profile is Tile_End)
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
