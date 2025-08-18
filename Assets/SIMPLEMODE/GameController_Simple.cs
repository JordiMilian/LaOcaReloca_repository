using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Events;

public class GameController_Simple : MonoBehaviour
{
    public Board_Controller_simple BoardController;
    Camera mainCamera;
    public ShopController shopController;

    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;
    public Dices_Controller dicesController;


    //COROUTINE EVENTS
    public CardEffectsDelegate OnRolledDice_CardEffects = new();
    public CardEffectsDelegate OnKilledEnemy_CardEffects = new();
    public CardEffectsDelegate OnReachedStartTile_CardEffects = new();

    public static GameController_Simple Instance;
    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }
    private void Start()
    {
        UpdateMoneyUI();
        UpdateAcumulatedDamageDisplay();
        UpdateEnemyHPBar();
    }
    private void Update()
    {
        GetIntersectingTilesToMouse();   
    }
    #region INTERSECTING TILES WITH MOUSE
    [SerializeField] List<Tile_Base> intersecticTiles;
    void GetIntersectingTilesToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitsArray;
        intersecticTiles = new();
        hitsArray = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hitsArray)
        {
            if (hit.collider.TryGetComponent(out Tile_Base tileBase))
            {
                intersecticTiles.Add(tileBase);
            }
        }
    }
        #endregion

    #region PLACE AND MOVE TILES
    Tile_Base SelectedTile;
    public void SelectedNewTile(Tile_Base tile)
    {
        SelectedTile = tile;
    }
    public bool CanPlaceTile()
    {
        if(intersecticTiles.Count <= 1) { return false; }

        Tile_Base tileBelow = null;
        foreach (Tile_Base tile in intersecticTiles)
        {
            if (tile == SelectedTile) { continue; }
            tileBelow = tile;
            break;
        }

        if(tileBelow.tileState != TileState.InBoard) { return false; }
        if (!tileBelow.tileMovement.canBeMoved) { return false; }
        if (tileBelow.isBehindPlayer && SelectedTile.tileState == TileState.InBoard) { return false; }
        if(SelectedTile.tileState == TileState.InShop)
        {
            ShopItem_Controller shopItem = shopController.GetShopItem(SelectedTile);
            if (!CanPurchase(shopItem.buyable.GetBuyingPrice() +1)) { return false; }
        }
       
        if(tileBelow is Tile_End || tileBelow is Tile_Start) { return false; }
        return true;
    }
    public void PlaceTile() //Called from TileMovement OnMouseUp
    {
        Tile_Base tileInBoard = null;
        foreach (Tile_Base tile in intersecticTiles) //Search for the tile in board
        {
            if(tile == SelectedTile) { continue; }
            tileInBoard = tile;
            break;
        }
        //Depending on state, do something
        if(SelectedTile.tileState == TileState.InShop && tileInBoard.tileState == TileState.InBoard)
        {
            PlaceTileFromShopToBoard(tileInBoard, SelectedTile); //The price check is done in the CanPlace()
        }
        if(SelectedTile.tileState == TileState.InBoard)
        {
            BoardController.MoveTileInBoard(SelectedTile.indexInBoard, tileInBoard.indexInBoard);
            //MoveTilesInBoard(SelectedTile.indexInBoard, tileInBoard.indexInBoard);
        }
    }
    void PlaceTileFromShopToBoard(Tile_Base tileInBoard, Tile_Base boughtTile)
    {
        RemoveMoney(SelectedTile.GetBuyingPrice());
        //BoardController.ReplaceTileInBoard(tileInBoard, boughtTile);
        BoardController.AddNewTile(boughtTile, tileInBoard.indexInBoard);
        ShopItem_Controller boughtItem = shopController.GetShopItem(SelectedTile);
        boughtItem.RemoveItem();

        shopController.UpdatePrices();
    }
    #endregion
    #region DAMAGE
    [Header("Enemy HP")]
    [SerializeField] float AcumulatedDamage;
    [SerializeField] float AcumulatedMultiplier;
    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    [SerializeField] Healthbar healthbar;
    public IEnumerator Co_AddAcumulatedDamage(float amount)
    {
        //if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedDamage += amount;
        UpdateAcumulatedDamageDisplay();

        const float shakeDuration = .15f;
        TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30);
        yield return new WaitForSeconds(shakeDuration);
    }
    public IEnumerator Co_AddAcumulatedMultiplier(float amount)
    {
        if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedMultiplier += amount;
        UpdateAcumulatedDamageDisplay();

        const float shakeDuration = .15f;
        TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30);
        yield return new WaitForSeconds(shakeDuration);
    }
    public float GetCurrentAcumulatedDamage() { return AcumulatedDamage; }
    IEnumerator DealTotalDamage()
    {
        float totalDamage = AcumulatedDamage * AcumulatedMultiplier;
        Enemy_CurrentHP -= totalDamage;
        Enemy_CurrentHP = Mathf.Clamp(Enemy_CurrentHP, 0, Enemy_MaxHP);
        AcumulatedDamage = 0;
        AcumulatedMultiplier = 1;

        TMP_AcumulatedDamage.text = $"<color=purple>{MathJ.FloatToString(totalDamage, 1)}";
        UpdateEnemyHPBar();

        float shakeDuration = .5f;
        Sequence shakeSequence = DOTween.Sequence();

        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30));
        shakeSequence.Join(TMP_AcumulatedDamage.rectTransform.DOScale(1.3f, shakeDuration / 2));
        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOScale(1f, shakeDuration / 2));

        yield return new WaitForSeconds(shakeDuration);
        

        if (Mathf.Approximately( Enemy_CurrentHP,0))
        {
            ChangeGameState(GameState.KilledEnemy);
        }
    }
    void UpdateAcumulatedDamageDisplay()
    {
        
        TMP_AcumulatedDamage.text = $"<color=blue>{MathJ.FloatToString(AcumulatedDamage, 1)}<color=white> x <color=red>{MathJ.FloatToString(AcumulatedMultiplier,1)}";
    }
    void UpdateEnemyHPBar()
    {
        healthbar.UpdateHealthbar(Enemy_CurrentHP, Enemy_MaxHP);
    }
    #endregion
    #region MONEY
    [Header("Money")]
    public UnityEvent<int> OnMoneyUpdated;
    [SerializeField] int currentMoney;

    [SerializeField] TextMeshProUGUI TMP_CurrentMoney;
    public void AddMoney(int money) { SetMoney(currentMoney + money); }
    public void RemoveMoney(int money) { SetMoney(currentMoney - money); }
    void SetMoney(int newMoney)
    { 
        currentMoney = newMoney; 
        if (currentMoney < 0) { currentMoney = 0; }
        UpdateMoneyUI();
        OnMoneyUpdated.Invoke(currentMoney);

        if (currentMoney < MoneyToRoll) { ChangeGameState(GameState.PlayerDied); }
    }
    public int GetCurrentMoney() { return currentMoney; }
    public bool CanPurchase(int price) { return price <= currentMoney; }
    void UpdateMoneyUI()
    {
        TMP_CurrentMoney.text = currentMoney.ToString();
    }
    #endregion


}
