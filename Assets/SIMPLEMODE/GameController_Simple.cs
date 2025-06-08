using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
public enum GameState
{
    Empty, StartBoard, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied
}
public class GameController_Simple : MonoBehaviour
{
    public GameState currentGameState { get; private set; }
    [SerializeField] Board_Controller_simple BoardController;
    Camera mainCamera;

    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;
    public Dices_Controller dicesController;

    [Header("Hand Tiles")]
    [SerializeField] int maxHandTilesCount = 5;
    [SerializeField] float distanceBetweenHandTiles = 1;

    [SerializeField] GameObject EmptyTile;

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
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            if (RollDiceButton.interactable) { Button_RollDicesTestButton(); }
        }
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
                //Nothing?
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
                break;
        }
        currentGameState = newState;
    }
    #region START GAME
    IEnumerator StartGameCoroutine()
    {
        yield return BoardController.StartBoard();
        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region FREE MODE
    void OnFreeModeEnter()
    {
        RollDiceButton.interactable = true;
    }
    void OnFreeModeExit()
    {
        RollDiceButton.interactable = false;
    }
    #endregion
    #region MOVING PLAYER 
    Coroutine regularMovingCoroutine;
    //This mode is entered when the dices are rolled
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    public int remainingStepsToTake;
    IEnumerator OnMovingPlayer_Coroutine()
    {
        RollDiceButton.interactable = false;
        remainingStepsToTake = dicesController.RollDices();
        TMP_rolledDiceAmount.text = remainingStepsToTake.ToString();

        while(remainingStepsToTake > 0)
        {
            yield return BoardController.L_StepPlayer(true);
            remainingStepsToTake--;
        }
        while (remainingStepsToTake < 0)
        {
            yield return BoardController.L_StepPlayer(false);
            remainingStepsToTake++;
        }

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
        yield return StartCoroutine(BoardController.L_JumpPlayerTo(0, false));

        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region KILLED ENEMY
    IEnumerator KilledEnemy_Coroutine()
    {
        AddMoney(10);
        yield return BoardController.L_JumpPlayerTo(0, false);
        Enemy_MaxHP *= 1.2f;
        Enemy_CurrentHP = Enemy_MaxHP;
        UpdateEnemyHpUI();
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
       
        if(tileBelow is Tile_End || tileBelow is Tile_Start) { return false; }
        return true;
    }
    public void PlaceTile() //Called from TileMovement OnMouseUp
    {
        Tile_Base tileInBoard = null;
        foreach (Tile_Base tile in intersecticTiles)
        {
            if(tile == SelectedTile) { continue; }
            tileInBoard = tile;
            break;
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

        //Get position and rotation of the tile in Board
        transformStats inBoardTransformStats = tileInBoard.tileMovement.originTransform;

        //Create new Tile in the proper position and destroy the last
        GameObject newTileGO = tileInHand.gameObject;
        
        Destroy(tileInBoard.gameObject);

        //Get the references right and remove the tile from hand
        BoardController.TilesList[indexInBoard] = tileInHand;
        BoardController.TilesList[indexInBoard].indexInBoard = indexInBoard;
        BoardController.TilesList[indexInBoard].UpdateTileVisuals();

        RemoveTileInHand(indexInHand);

        //play animations
        TileMovement newTileMovement = tileInHand.GetComponent<TileMovement>();
        newTileMovement.SetOriginTransformWithStats(inBoardTransformStats);
        newTileMovement.MoveTileToOrigin(); 
        tileInHand.SetTileState(TileState.InBoard);
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
    [SerializeField] TextMeshProUGUI TMP_EnemyHp;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    [SerializeField] Slider slider_EnemyHP;
    public IEnumerator AddAcumulatedDamage(float amount)
    {
        if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedDamage += amount;
        UpdateEnemyHpUI();
        yield break;
    }
    public float GetCurrentAcumulatedDamage() { return AcumulatedDamage; }
    IEnumerator DealAcumulatedDamage()
    {
        yield return new WaitForSeconds(0.1f);
        Enemy_CurrentHP -= AcumulatedDamage;
        AcumulatedDamage = 0;
        if (Enemy_CurrentHP <= 0)
        {
            Enemy_CurrentHP = 0;
            UpdateEnemyHpUI();
            ChangeGameState(GameState.KilledEnemy);
            yield break;
        }
        UpdateEnemyHpUI();
    }
    void UpdateEnemyHpUI()
    {
        slider_EnemyHP.maxValue = Enemy_MaxHP;
        slider_EnemyHP.value = Enemy_CurrentHP;
        TMP_EnemyHp.text = $"{Enemy_CurrentHP.ToString("F1")}/{Enemy_MaxHP.ToString("F1")}";
        TMP_AcumulatedDamage.text = $"{AcumulatedDamage}";
    }
    #endregion
    #region MONEY
    [Header("Money")]
    [SerializeField] int currentMoney;
    [SerializeField] TextMeshProUGUI TMP_CurrentMoney;
    public void AddMoney(int money)
    {
        currentMoney += money;
        if (currentMoney < 0) { currentMoney = 0; }
        UpdateMoneyUI();
    }
    void SetMoney(int newMoney)
    { currentMoney = newMoney; 
      if (currentMoney < 0) { currentMoney = 0; }
        UpdateMoneyUI();
    }
    public bool CanPurchase(int price) { return price <= currentMoney; }
    public void Purchase(int price)
    {
        currentMoney -= price;
        UpdateMoneyUI();
    }
    void UpdateMoneyUI()
    {
        TMP_CurrentMoney.text = currentMoney.ToString();
    }
    #endregion

   
}
