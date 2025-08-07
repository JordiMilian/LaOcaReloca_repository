using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Events;
public enum GameState
{
    Empty, StartBoard, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied
}
public class GameController_Simple : MonoBehaviour
{
    public GameState currentGameState;
    [SerializeField] Board_Controller_simple BoardController;
    Camera mainCamera;
    [SerializeField] ShopController shopController;

    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;
    public Dices_Controller dicesController;


    [SerializeField] GameObject EmptyTile;

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
        ChangeGameState(GameState.StartBoard);
        UpdateMoneyUI();
        UpdateEnemyHpUI();
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
    #region GAME FLOW

    public void ChangeGameState(GameState newState)
    {
        //On EXIT this State
        switch (currentGameState)
        {
            case GameState.StartBoard:
                break;
            case GameState.MovingPlayer:
                if (regularMovingCoroutine != null) { StopCoroutine(regularMovingCoroutine); }
                break;
            case GameState.FreeMode:
                OnFreeModeExit();
                break;
            case GameState.ReachedEnd:
                break;
            case GameState.KilledEnemy:
                break;
            case GameState.PlayerDied:
                break;
        }

        //On ENTER this State
        switch (newState)
        {
            case GameState.StartBoard:
                StartCoroutine(StartGameCoroutine());
                break;
            case GameState.MovingPlayer:
                regularMovingCoroutine = StartCoroutine(OnMovingPlayer_Coroutine());
                break;
            case GameState.FreeMode:
                //zoom out the camera
                //enable shop items purchase
                OnFreeModeEnter();
                break;
            case GameState.ReachedEnd:
                StartCoroutine(OnReachedEnd_Coroutine());
                break;
            case GameState.KilledEnemy:
                //Display some victory screen
                StartCoroutine(KilledEnemy_Coroutine());

                break;
            case GameState.PlayerDied:
                //called when not killing the dude before X turns
                //SHow game over screen
                //restart game
                Debug.Log("PlayerDied");
                RollDiceButton.interactable = false;
                break;
        }
        currentGameState = newState;
    }
    #region START GAME
    IEnumerator StartGameCoroutine()
    {
        shopController.ResetShopItems();
        OnFreeModeExit();
        yield return BoardController.StartBoard();
        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region FREE MODE
    void OnFreeModeEnter()
    {
        RollDiceButton.interactable = true;
        shopController.EnableShop();
    }
    void OnFreeModeExit()
    {
        RollDiceButton.interactable = false;
        shopController.DisableShop();
    }
    #endregion
    #region MOVING PLAYER 
    Coroutine regularMovingCoroutine;
    //This mode is entered when the rolling dice button is pressed
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    public int remainingStepsToTake;
    [Header("stepping audio")]
    [SerializeField] AudioSource StepSound;
    [SerializeField] float addPitchPerStep;
    [Header("Money to Roll")]
    public int MoneyToRoll = 1;
    IEnumerator OnMovingPlayer_Coroutine()
    {
        if(dicesController.GetDicesToRoll().Count == 0)
        {
            Debug.LogWarning("No dices to roll, please select at least one");
            ChangeGameState(GameState.FreeMode);
            yield break;
        }
        RemoveMoney(MoneyToRoll);
        dicesController.SetDicesDraggable(false);
        RollDiceButton.interactable = false;
        yield return dicesController.RollDicesCoroutine();
        dicesController.SetDicesDraggable(true);
        remainingStepsToTake = dicesController.LastRolledValue;

        TMP_rolledDiceAmount.text = remainingStepsToTake.ToString();

        yield return OnRolledDice_CardEffects.ActivateEffects();

        float basePitch = StepSound.pitch;

        while (remainingStepsToTake > 0)
        {
            StepSound.pitch += addPitchPerStep;
            StepSound.Play();
            yield return BoardController.L_StepPlayer(true);
            remainingStepsToTake--;
        }
        while (remainingStepsToTake < 0)
        {
            yield return BoardController.L_StepPlayer(false);
            remainingStepsToTake++;
        }
        StepSound.pitch = basePitch;

        yield return BoardController.L_LandPlayerInCurrentPos();

        yield return DealAcumulatedDamage();

        ChangeGameState(GameState.FreeMode); 
            
    }
    public void Button_RollDicesTestButton()
    {
        ChangeGameState(GameState.MovingPlayer);
    }
    #endregion
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        yield return DealAcumulatedDamage();
        yield return BoardController.JumpPlayerToStartTile();

        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region KILLED ENEMY
    [SerializeField] PlayableDirector Timeline_KilledEnemy;
    IEnumerator KilledEnemy_Coroutine()
    {
        Timeline_KilledEnemy.Play();
        float timelineDuration = (float)Timeline_KilledEnemy.duration;
        yield return new WaitForSeconds(timelineDuration);

        AddMoney(10);
        Enemy_MaxHP *= 1.2f;
        Enemy_CurrentHP = Enemy_MaxHP;
        UpdateEnemyHpUI();

        yield return BoardController.JumpPlayerToStartTile();

        yield return OnKilledEnemy_CardEffects.ActivateEffects();

        ChangeGameState(GameState.FreeMode);
    }
    #endregion
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
            if (!CanPurchase(shopItem.Item.GetBuyingPrice())) { return false; }
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
            MoveTilesInBoard(SelectedTile.indexInBoard, tileInBoard.indexInBoard);
        }
        else if(SelectedTile.tileState == TileState.InHand)
        {
            PlaceTileFromHandToBoard(SelectedTile.IndexInHand, tileInBoard.indexInBoard);
        }

    }
    void MoveTilesInBoard(int indexAtoB, int indexBtoA)
    {
        Tile_Base tileAtoB = BoardController.TilesList[indexAtoB];
        Tile_Base tileBtoA = BoardController.TilesList[indexBtoA];

        transformStats ghostStatsA = tileAtoB.tileMovement.originTransform;

        Vector2Int vectorAtoB = tileAtoB.vectorInBoard;
        Vector2Int vectorBtoA = tileBtoA.vectorInBoard;
        tileAtoB.vectorInBoard = vectorBtoA;
        tileBtoA.vectorInBoard = vectorAtoB;
        BoardController.TilesByPosition[vectorAtoB] = tileBtoA;
        BoardController.TilesByPosition[vectorBtoA] = tileAtoB;


        BoardController.TilesList[indexAtoB] = tileBtoA;
        BoardController.TilesList[indexBtoA] = tileAtoB;
        tileBtoA.indexInBoard = indexAtoB;
        tileAtoB.indexInBoard = indexBtoA;


        tileAtoB.tileMovement.SetOriginTransformWithTransform(tileBtoA.transform);
        tileAtoB.tileMovement.MoveTileToOrigin();

        tileBtoA.tileMovement.SetOriginTransformWithStats(ghostStatsA);
        tileBtoA.tileMovement.MoveTileToOrigin();

        tileAtoB.UpdateTileVisuals();
        tileBtoA.UpdateTileVisuals();

    }
    void PlaceTileFromHandToBoard(int indexInHand, int indexInBoard)
    {
        //Get tile infos
        Tile_Base tileInHand = HandPositions[indexInHand].filledTileInfo;
        Tile_Base tileInBoard = BoardController.TilesList[indexInBoard];

        ReplaceTileInBoard(tileInBoard, tileInHand);
        RemoveTileInHand(indexInHand);

    }
    void PlaceTileFromShopToBoard(Tile_Base tileInBoard, Tile_Base boughtTile)
    {
        RemoveMoney(SelectedTile.GetBuyingPrice());
        ReplaceTileInBoard(tileInBoard, boughtTile);
        ShopItem_Controller boughtItem = shopController.GetShopItem(SelectedTile);
        boughtItem.RemoveItem();

        shopController.UpdatePrices();
    }
    public void ReplaceTileInBoard(Tile_Base tileInBoard, Tile_Base newTile)
    {
        tileInBoard.OnRemovedFromBoard();
        BoardController.TilesList[tileInBoard.indexInBoard] = newTile;
        BoardController.TilesByPosition[tileInBoard.vectorInBoard] = newTile;
        newTile.indexInBoard = tileInBoard.indexInBoard;
        newTile.vectorInBoard = tileInBoard.vectorInBoard;

        newTile.transform.parent = BoardController.transform;

        newTile.tileMovement.SetOriginTransformWithStats(tileInBoard.tileMovement.originTransform);
        newTile.tileMovement.MoveTileToOrigin();
        newTile.SetTileState(TileState.InBoard);

        newTile.OnPlacedInBoard();

        Destroy(tileInBoard.gameObject);

        newTile.UpdateTileVisuals();//Esto sobre casi segur
    }
    public HandHolder[] HandPositions;
    [Serializable]
    public class HandHolder
    {
        public Transform TilePositionTf;
        public bool isFilled;
        public Tile_Base filledTileInfo;
    }
    public int GetEmptyHandIndex()
    {
        for (int i = 0; i < HandPositions.Length; i++)
        {
            if (!HandPositions[i].isFilled) { return i; }
        }
        return -1;
    }
    public void AddTileToHand(GameObject Tile)
    {
        int emptyHandIndex = GetEmptyHandIndex();
        HandHolder hand = HandPositions[emptyHandIndex];
        if (hand == null) { return; }

        hand.filledTileInfo = Tile.GetComponent<Tile_Base>();
        hand.filledTileInfo.tileMovement.SetOriginTransformWithTransform(hand.TilePositionTf);
        hand.filledTileInfo.tileMovement.MoveTileToOrigin();
        hand.filledTileInfo.tileMovement.canBeMoved = true;
        hand.filledTileInfo.IndexInHand = emptyHandIndex;
        hand.filledTileInfo.SetTileState(TileState.InHand);
        hand.isFilled = true;
    }
    void RemoveTileInHand(int index)
    {
        HandHolder holder = HandPositions[index];
        holder.isFilled = false;
        holder.filledTileInfo = null;

    }
    public bool isHandFull()
    {
        foreach (HandHolder holder in HandPositions)
        {
            if (!holder.isFilled) { return false; }
        }
        return true;

    }
    #endregion
    #region DAMAGE
    [Header("Enemy HP")]
    [SerializeField] float AcumulatedDamage;
    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    [SerializeField] Healthbar healthbar;
    public IEnumerator AddAcumulatedDamage(float amount)
    {
        if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedDamage += amount;
        UpdateEnemyHpUI();

        float shakeDuration = .15f;
        TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30);
        yield return new WaitForSeconds(shakeDuration);
    }
    public float GetCurrentAcumulatedDamage() { return AcumulatedDamage; }
    IEnumerator DealAcumulatedDamage()
    {
        Enemy_CurrentHP -= AcumulatedDamage;
        Enemy_CurrentHP = Mathf.Clamp(Enemy_CurrentHP, 0, Enemy_MaxHP);
        AcumulatedDamage = 0;

        UpdateEnemyHpUI();

        float shakeDuration = .4f;
        Sequence shakeSequence = DOTween.Sequence();

        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30));
        shakeSequence.Join(TMP_AcumulatedDamage.rectTransform.DOScale(1.3f, shakeDuration / 2));
        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOScale(1f, shakeDuration / 2));

        yield return new WaitForSeconds(shakeDuration);

        if (Enemy_CurrentHP == 0)
        {
            Enemy_CurrentHP = 0;
            UpdateEnemyHpUI();
            ChangeGameState(GameState.KilledEnemy);
            yield break;
        }
        else if(Enemy_CurrentHP > Enemy_MaxHP) { Enemy_CurrentHP = Enemy_MaxHP; }
        
    }
    void UpdateEnemyHpUI()
    {
        healthbar.UpdateHealthbar(Enemy_CurrentHP, Enemy_MaxHP);
        TMP_AcumulatedDamage.text = MathJ.FloatToString(AcumulatedDamage, 1);
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
