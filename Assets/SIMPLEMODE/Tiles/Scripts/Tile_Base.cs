using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;

public enum Intensity
{
    empty, low, mid, large
}
public enum TileState
{
    none, InShop, InHand, InBoard
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
public class Tile_Base : MonoBehaviour
{
    public string TitleText = "NO NAME";
    public TileTags tileTag;
    [HideInInspector] public TileState tileState = TileState.none;
    public float defaultCrossedDamage = 1;
    public Rarity rarity = Rarity.none;
    [HideInInspector] public int indexInBoard;
    [HideInInspector] public int IndexInHand;
    [HideInInspector] public Vector2Int vectorInBoard;
    [HideInInspector] public Vector2 positionInBoardAxis;

    [Header("Color testing")]
    public Color tileColor;
    SpriteRenderer tileTestSprite;

    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    [HideInInspector] public TileSharedVisuals tileMovement;
    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;

        tileTestSprite = GetComponentInChildren<SpriteRenderer>(); //remember placeholder

        tileMovement = GetComponent<TileSharedVisuals>();

    }
    private void Start()
    {
        UpdateTileVisuals();
    }
    public void CopyData(Tile_Base copyingTile)
    {
        tileColor = copyingTile.tileColor;
        rarity = copyingTile.rarity;
    }
    public void UpdateTileVisuals()
    {
        tileTestSprite.color = tileColor;
        tileMovement.UpdateDmgDisplayText();
    }
    #region CROSSING DAMAGE
    public float GetDefaultCrossedDamage()
    {
        return defaultCrossedDamage;
    }
    void SetDefaultCrossingDamage(float newDamage)
    {
        defaultCrossedDamage = newDamage;
        tileMovement.UpdateDmgDisplayText();
    }
    public void AddPermaDamage(float addedDamage)
    {
        SetDefaultCrossingDamage(defaultCrossedDamage + addedDamage);
        tileMovement.shakeTile(Intensity.mid);
        if (Mathf.Approximately(addedDamage, 0)) { return; }
        if(addedDamage >= 0)
        {
            tileMovement.DisplayMessage("+" + MathJ.FloatToString(addedDamage, 1), TileMessageType.AddPermaDamage);
        }
        else
        {
            tileMovement.DisplayMessage(MathJ.FloatToString(addedDamage, 1), TileMessageType.AddPermaDamage);
        }
    }
    public void MultiplyCrossingDamage(float mult)
    {
        SetDefaultCrossingDamage(defaultCrossedDamage * mult);
        tileMovement.shakeTile(Intensity.mid);
        //Number display
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
            case TileState.InShop: //TO DO: InShop_Affordable, InShop_Unaffordable maybe
                tileMovement.canBeMoved = true;
                break;
            case TileState.InHand: 
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
        if (isBehindPlayer) { tileTestSprite.color = new Color(tileColor.r, tileColor.g, tileColor.b, 0.75f); }
        else { tileTestSprite.color = tileColor; }
    }
    #region MAIN VIRTUAL LOGIC METHODS
    public virtual IEnumerator OnPlayerStepped()
    {
        UpdateTileVisuals();
        yield return GameController.Co_AddAcumulatedDamage(GetCrossedDamageAmount());

    }
    public virtual IEnumerator OnPlayerLanded()
    {
        tileMovement.shakeTile(Intensity.mid);
        yield break;
    }
    public virtual void OnPlacedInBoard() { }
    public virtual void OnRemovedFromBoard() { }

    public virtual float GetCrossedDamageAmount()
    {
        return defaultCrossedDamage;
    }
    public virtual string GetTooltipText()
    {
        return $"EMPTY TILE";
    }
    #endregion
    #region BUY/SELL
    public int GetBuyingPrice()
    {
        int repeatedCards = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile.GetType() == this.GetType()) { repeatedCards++; }
        }
        foreach(GameController_Simple.HandHolder handPOs in GameController.HandPositions)
        {
            if(handPOs.isFilled && handPOs.filledTileInfo.GetType() == this.GetType())
            {
                repeatedCards++;
            }
        }

        int baseValue = 0;
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


    #endregion
    #region TOOLTIP INTRO
    protected enum On
    {
        OnCrossed, OnLanded, OnRolledDice, OnReached
    }
    protected string ON(On on)
    {
        switch (on)
        {
            case On.OnCrossed: return "<b>ON CROSSED:<b>";
            case On.OnLanded: return "<b>ON LANDED:<b>";
            case On.OnRolledDice: return "<b>ON ROLLED DICES:<b>";
            case On.OnReached:return "<b>ON REACHED:<b>";
            default: return "";

        }
    }
    #endregion
    protected List<Tile_Base> GetTilesAround(bool ignoreSelf)
    {
        List<Tile_Base> tilesAround = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int tileIndex = vectorInBoard + new Vector2Int(i, j);

                if (i == 0 && j == 0)
                {
                    if(!ignoreSelf){ tilesAround.Add(BoardController.TilesByPosition[tileIndex]); }
                    continue;
                }   

                if (BoardController.TilesByPosition.ContainsKey(tileIndex))
                {
                    tilesAround.Add(BoardController.TilesByPosition[tileIndex]);
                }
            }
        }
        return tilesAround;
    }
}
