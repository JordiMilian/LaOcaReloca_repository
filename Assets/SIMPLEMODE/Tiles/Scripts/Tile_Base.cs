using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Intensity
{
    empty, low, mid, large
}
public enum TileState
{
    none, InShop, InBoard
}
public enum Rarity
{
    none, Common, Rare, Legendary, Unique
}
public enum TileTags
{
    NoTag,EmptyTile, Oca
}
public enum TileMessageType
{
    Neutral, AddDamage, AddMultiplier, AddPermaDamage
}
public class Tile_Base : MonoBehaviour, IBuyable
{
    public string TitleText = "NO NAME";
    public TileTags tileTag;
    [HideInInspector] public TileState tileState = TileState.none;
    public float BaseDamage = 1;
    public Rarity rarity = Rarity.none;
    [HideInInspector] public int indexInBoard;
    [HideInInspector] public int IndexInHand;
    [HideInInspector] public Vector2Int vectorInBoard;

    [Header("Color testing")]
    public Color tileColor;

    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    [HideInInspector] public TileSharedVisuals tileMovement;
    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;
        tileMovement = GetComponent<TileSharedVisuals>();

    }
    #region DAMAGE MODIFIERS
    protected List<float> DamagesToDeal = new();
    public virtual float GetBaseDamage()
    {
        return BaseDamage;
    }
    protected void SetBaseDamage(float newDamage)
    {
        BaseDamage = newDamage;
        tileMovement.UpdateDmgDisplayText();
    }
    public virtual void AddBaseDamage(float addedDamage)
    {
        SetBaseDamage(BaseDamage + addedDamage);
        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage("+" + MathJ.FloatToString(addedDamage, 1), TileMessageType.AddPermaDamage);
    }
    public virtual void RemoveBaseDamage(float removedDamage)
    {
        if(removedDamage > BaseDamage)
        {
            removedDamage = BaseDamage;
        }
        SetBaseDamage(BaseDamage - removedDamage);
        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage($"-{removedDamage}", TileMessageType.AddPermaDamage);
    }
    public void MultiplyBaseDamage(float mult)
    {
        SetBaseDamage(BaseDamage * mult);
        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage($"x{mult}", TileMessageType.AddPermaDamage);
    }
    protected IEnumerator C_DealAllDamageToDeal()
    {
        if(DamagesToDeal.Count == 0) { yield break; }

        float totalDamage = 0;

        string displayMessage = "";
        for (int i = 0; i < DamagesToDeal.Count; i++)
        {
            displayMessage += MathJ.FloatToString(DamagesToDeal[i], 1);
            if (i != DamagesToDeal.Count - 1) { displayMessage += "+"; }
        }
        tileMovement.DisplayMessage(displayMessage, TileMessageType.AddDamage);

        foreach (float dmg in DamagesToDeal) { totalDamage += dmg; }
        yield return GameController.C_AddAcumulatedDamage(totalDamage);

        

        DamagesToDeal.Clear();
    }
    #endregion
    public void SetTileState(TileState newState)
    {
        if(newState == tileState) { return; }

        //EXIT
        switch(tileState)
        {
            case TileState.InBoard:
                BoardController.OnPlayerMoved.RemoveListener(CheckForDraggability);
                break;
        }

        //ENTER
        switch (newState)
        {
            case TileState.none:
                break;
            case TileState.InShop: 
                tileMovement.canBeMoved = true;
                break;
            case TileState.InBoard:
                if(this is Tile_End || this is Tile_Start) { tileMovement.canBeMoved = false; break; }
                tileMovement.canBeMoved = true;
                BoardController.OnPlayerMoved.AddListener(CheckForDraggability);
                CheckForDraggability(0, BoardController.PlayerIndex);
                break;
        }
        tileState = newState;
    }

    [HideInInspector] public bool isBehindPlayer;
    void CheckForDraggability(int from, int to)
    {
        isBehindPlayer = BoardController.PlayerIndex >= indexInBoard;
        if (isBehindPlayer) { tileMovement.SetBasicPanelColor_Transparent(); }
        else { tileMovement.SetBasicPanelColor(); }
    }
    #region MAIN VIRTUAL LOGIC METHODS

    public virtual IEnumerator OnPlayerStepped()
    {
        DamagesToDeal.Add(GetBaseDamage());
        DamagesToDeal.Reverse();

        if (GameController.remainingStepsToTake != 1) { yield return C_DealAllDamageToDeal(); } //Deal damage unless its the last step
    }
    public virtual IEnumerator OnPlayerLanded()
    {
        tileMovement.shakeTile(Intensity.mid);

        yield return C_DealAllDamageToDeal(); 
    }
    public virtual void OnPlacedInBoard() { }
    public virtual void OnRemovedFromBoard() { }

    public virtual string GetTooltipText()
    {
        return $"EMPTY TILE";
    }
    #endregion
    #region BUY/SELL
    public virtual int GetBuyingPrice()
    {
        int repeatedCards = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile.GetType() == this.GetType()) { repeatedCards++; }
        }

        int baseValue;
        switch (rarity)
        {
            case Rarity.Common: { baseValue = 2; break; }
            case Rarity.Rare: { baseValue = 4; break; }
            case Rarity.Legendary: { baseValue = 10; break; }
            default: { Debug.LogError("ERROR: Pls set a valid rarity to this Tile"); return 0; }
        }
        return MathJ.GetFibonacciValue(baseValue, repeatedCards);

    }
    public int GetSellingPrice()
    {
        switch (rarity)
        {
            case Rarity.Common: { return 2; }
            case Rarity.Rare: { return 4; }
            case Rarity.Legendary: { return 8; }
            default: { Debug.LogError($"ERROR: Pls set a valid rarity to this {GetType()}"); return 0; }
        }
    }
    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        tileMovement.SetOriginTransformWithTransform(shopItemController.buyablePositionTf);
        tileMovement.PlaceTileInOrigin();
        SetTileState(TileState.InShop);
    }
    public void OnEnablePurchase()
    {
        tileMovement.canBeMoved = true;
    }

    public void OnDisablePurchase()
    {
        tileMovement.canBeMoved = false;
    }

    #endregion
    #region TOOLTIP INTRO
    protected const string OnCrossed = "<b>- ON CROSSED:</b>";
    protected const string OnLanded = "<b>- ON LANDED:</b>";
    protected const string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    protected const string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    protected const string OnReached = "<b>- ON REACHED:</b>";
    protected const string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";

    #endregion
}
