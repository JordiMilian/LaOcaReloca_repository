using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameController_Simple : MonoBehaviour
{
    [SerializeField] GameState currentGameState;
    [SerializeField] Board_Controller_simple BoardController;

    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;
    public Dices_Controller dicesController;

    [Header("Hand Tiles")]
    [SerializeField] int maxHandTilesCount = 5;
    [SerializeField] List<Tile_Base> tilesInHand = new();
    [SerializeField] float distanceBetweenHandTiles = 1;

    [Header("Place tiles debug")]
    [SerializeField] bool triggerTestPlace;
    [SerializeField] int indexInBoard;
    [SerializeField] int indexInHand;

    bool isSelectingHandTile;
    Tile_Base SelectedTile;

    [SerializeField] GameObject EmptyTile;

    public static GameController_Simple Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChangeGameState(GameState.StartBoard);
    }
    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            if (RollDiceButton.interactable) { Button_RollDicesTestButton(); }
        }
        if(triggerTestPlace)
        {
            triggerTestPlace = false;
            PlaceTileFromHand(indexInHand, indexInBoard);
        }
    }
    
    

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
    IEnumerator OnMovingPlayer_Coroutine()
    {
        RollDiceButton.interactable = false;
        int rolledAmount = dicesController.RollDices();
        TMP_rolledDiceAmount.text = rolledAmount.ToString();

        for (int i = 0; i < Mathf.Abs(rolledAmount); i++)
        {
            yield return BoardController.L_StepPlayer(rolledAmount > 0);
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
        yield return BoardController.L_JumpPlayerTo(0, false);
        Enemy_MaxHP *= 1.2f;
        Enemy_CurrentHP = Enemy_MaxHP;
        UpdateEnemyHpUI();
        ChangeGameState(GameState.FreeMode);
    }
    #endregion

    #region HAND MANAGEMENT
    void PlaceTileFromHand(int indexInHand, int indexInBoard)
    {
        //Get tile infos
        Tile_Base tileInHand = tilesInHand[indexInHand];
        Tile_Base tileInBoard = BoardController.TilesList[indexInBoard];

        //Get position and rotation of the tile in Board
        Vector3 position = tileInBoard.transform.position;
        Quaternion rotation = tileInBoard.transform.rotation;

        //Create new Tile in the proper position and destroy the last
        GameObject newTileGO = InstantiateNewTile(tileInHand, indexInBoard);
        newTileGO.transform.position = position;
        newTileGO.transform.rotation = rotation;
        Destroy(tileInBoard.gameObject);

        //Get the references right and remove the tile from hand
        Tile_Base instantiatedTile = newTileGO.GetComponent<Tile_Base>();
        BoardController.TilesList[indexInBoard] = instantiatedTile;
        tilesInHand.RemoveAt(indexInHand);

        //play animation
        instantiatedTile.FirstAppeareanceAnim();

    }
    public GameObject InstantiateNewTile(Tile_Base tileInfo, int indexInBoard = 0)
    {
        GameObject newTileGO = Instantiate(EmptyTile);
        Tile_Base newTileInfo = (Tile_Base)newTileGO.AddComponent(tileInfo.GetType());
        newTileInfo.CopyVisualData(tileInfo);
        newTileInfo.indexInBoard = indexInBoard;
        newTileInfo.UpdateTileVisuals();
        

        return newTileGO;
    }
    public void AddItemToHand(Tile_Base tileInfo)
    {
        tilesInHand.Add(tileInfo);
        //Visual or whatever
    }
    public bool isHandFull()
    {
        return tilesInHand.Count >= maxHandTilesCount;
    }
    #endregion
    #region DAMAGE
    [Header("Enemy HP")]
    [SerializeField] float AcumulatedDamage;
    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;
    [SerializeField] TextMeshProUGUI TMP_EnemyHp;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    public IEnumerator AddAcumulatedDamage(float amount)
    {
        if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedDamage += amount;
        UpdateEnemyHpUI();
        yield break;
    }
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
        TMP_EnemyHp.text = $"{Enemy_CurrentHP.ToString("F2")}/{Enemy_MaxHP.ToString("F2")}";
        TMP_AcumulatedDamage.text = $"Acumulated damage:{AcumulatedDamage}";
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
    public bool AttemptPurchase(int price)
    {
        if(price > currentMoney) { return false; }
        currentMoney -= price;
        UpdateMoneyUI();
        return true;
    }
    void UpdateMoneyUI()
    {
        TMP_CurrentMoney.text = currentMoney.ToString();
    }
    #endregion

   
}
