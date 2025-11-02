using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GameState
{
    Empty,RollingDices ,MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied, EncountersTransition
}
public class GameController_Simple : MonoBehaviour
{
    [Header("References")]
    public GameState currentGameState;
    public Board_Controller_simple BoardController;
    Camera mainCamera;
    public ShopController shopController;
    public Dices_Controller dicesController;


    //COROUTINE EVENTS
    public CardEffectsDelegate OnRolledDice_CardEffects = new();
    public CardEffectsDelegate OnKilledEnemy_CardEffects = new();
    public CardEffectsDelegate OnReachedEndTile_CardEffects = new();
    public CardEffectsDelegate OnLanded_CardEffects = new(); //Any card effect that triggers when landing on another tile. The regular Onlanded effect of all cards is not concerned with this
    public CardEffectsDelegate OnCrossed_CardEffects = new();
    public CardEffectsDelegate OnAddedNewTileToBoard_CardEffect = new();

    public static GameController_Simple Instance;
    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }
    private IEnumerator Start()
    {
        UpdateMoneyUI();
        UpdateAcumulatedDamageDisplay();
        UpdateEnemyHPBar();

        shopController.DisableShop();

        yield return BoardController.StartBoard();

        shopController.ResetAllShopItems();

        ChangeGameState(GameState.EncountersTransition);

    }
    
    #region INTERSECTING TILES WITH MOUSE
    [SerializeField] List<TileController> intersecticTiles;
    void GetIntersectingTilesToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitsArray;
        intersecticTiles = new();
        hitsArray = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hitsArray)
        {
            if (hit.collider.TryGetComponent(out TileController tileBase))
            {
                intersecticTiles.Add(tileBase);
            }
        }
    }
        #endregion
    #region GAME FLOW

    public void ChangeGameState(GameState newState)
    {
        if(currentStateCoroutine != null)
        {
            StopCoroutine(currentStateCoroutine);
            currentStateCoroutine = null;
        }

        //On EXIT this State
        switch (currentGameState)
        {
            case GameState.FreeMode:
                OnFreeModeExit();
                break;
        }
        Debug.Log($"switching gameState from {currentGameState} to {newState}");
        currentGameState = newState;
        //On ENTER this State
        switch (currentGameState)
        {
            case GameState.RollingDices:
                currentStateCoroutine = StartCoroutine(C_RollingDIces());
                break;
            case GameState.MovingPlayer:
                currentStateCoroutine = StartCoroutine(C_MovingPlayer());
                break;
            case GameState.FreeMode:
                OnFreeModeEnter();
                break;
            case GameState.ReachedEnd:
                currentStateCoroutine = StartCoroutine(OnReachedEnd_Coroutine());
                break;
            case GameState.PlayerDied:
                //TO DO
                Debug.Log("PlayerDied");
                break;
            case GameState.EncountersTransition:
                currentStateCoroutine = StartCoroutine(C_LoadNextEncounter());
                break;
        }
    }
    #region FREE MODE
    void OnFreeModeEnter()
    {
        dicesController.EnableRollButton();
        dicesController.EnableAddExtraRollValueButton();
        shopController.EnableShop();
        if(BoardController.PlayerIndex == BoardController.TilesList.Count -1)
        {
            ChangeGameState(GameState.ReachedEnd);
        }
    }
    void OnFreeModeExit()
    {
        dicesController.DisableRollButton();
        dicesController.DisableAddExtraRollValueButton();
        shopController.DisableShop();
    }
    #endregion
    #region ROLLING DICES
    IEnumerator C_RollingDIces()
    {
        if (dicesController.GetDicesToRoll().Count == 0)
        {
            Debug.LogWarning("No dices to roll, please select at least one");
            ChangeGameState(GameState.FreeMode);
            yield break;
        }
        SetRemainingRolls(RollsRemaining-1);
        yield return dicesController.RollDicesCoroutine();

        ChangeGameState(GameState.MovingPlayer);

    }
    #endregion
    #region MOVING PLAYER 
    Coroutine currentStateCoroutine;
    //This mode is entered when the rolling dice button is pressed
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    public int remainingStepsToTake;
    [Header("stepping audio")]
    [SerializeField] AudioSource StepSound;
    [SerializeField] float addPitchPerStep;
    [Header("Money to Roll")]
    public int MoneyToRoll = 0;
    
    IEnumerator C_MovingPlayer()
    {
        remainingStepsToTake = dicesController.LastRolledValue;

        yield return OnRolledDice_CardEffects.C_ActivateEffects();

        while (remainingStepsToTake > 0)
        {
            yield return BoardController.L_StepPlayer();
            remainingStepsToTake--;
            if(remainingStepsToTake > 0)
            {
                yield return BoardController.GetCurrentPlayerTile().C_DealAllDamageToDeal();
            }
        }

        yield return OnLanded_CardEffects.C_ActivateEffects();

        yield return BoardController.L_LandPlayerInCurrentPos();
        yield return BoardController.GetCurrentPlayerTile().C_DealAllDamageToDeal();

        yield return DealTotalDamage();

        if(RollsRemaining <= 0) { ChangeGameState(GameState.PlayerDied); }
        else { ChangeGameState(GameState.FreeMode); }
           
    }
    public void ChangeStateToRollingDice()
    {
        ChangeGameState(GameState.RollingDices);
    }
    #endregion
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        yield return DealTotalDamage();
        yield return BoardController.JumpPlayerToStartTile();

        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region ENCOUNTERS
    [SerializeField] List<GameObject> EncountersPrefabs = new List<GameObject>();
    IEncounter currentEncounter;
    GameObject currentEncounterObject;
    int currentEncounterIndex = -1;
    [SerializeField] int[] EnemyEncountersHP;
    int enemiesEncountered = 0;
    IEnumerator C_LoadNextEncounter()
    {
        if (currentEncounterObject != null)
        {
            yield return currentEncounter.OnEncounterExit();
            Destroy(currentEncounterObject);
            currentEncounterObject = null;
        }
        currentEncounterIndex++;
        currentEncounterObject = Instantiate(EncountersPrefabs[currentEncounterIndex], transform.position,Quaternion.identity,transform);
        currentEncounter = currentEncounterObject.GetComponent<IEncounter>();

        //Cutre cutre pls refactor
        if(currentEncounter is Encounter_BasicEnemy)
        {
            Encounter_BasicEnemy enemyEncounter = currentEncounter as Encounter_BasicEnemy;
            enemyEncounter.MaxHp = EnemyEncountersHP[enemiesEncountered];
            enemiesEncountered++;
        }

        yield return currentEncounter.OnEncounterEnter();
    }
    #endregion
    #endregion
    #region PLACE AND MOVE TILES
    TileController SelectedTile;
    public void SelectedNewTile(TileController tile)
    {
        SelectedTile = tile;
    }
    public void UnselectCurrentTile() { SelectedTile = null; }
    public bool CanPlaceTile()
    {
        if(intersecticTiles.Count <= 1) { return false; }

        TileController tileBelow = null;
        foreach (TileController tile in intersecticTiles)
        {
            if (tile == SelectedTile) { continue; }
            tileBelow = tile;
            break;
        }

        if(tileBelow.tileState != TileState.InBoard) { return false; }
        if (!tileBelow.canBeMoved) { return false; }
        if (tileBelow.isBehindPlayer && SelectedTile.tileState == TileState.InBoard) { return false; }
        if(SelectedTile.tileState == TileState.InShop)
        {
            ShopItem_Controller shopItem = shopController.GetShopItem(SelectedTile);
            if (!CanPurchaseWithoutLosing(shopItem.buyable.GetBuyingPrice())) { return false; }
        }
       
        if(tileBelow._Profile is Tile_End || tileBelow._Profile is Tile_Start) { return false; }
        return true;
    }
    public void PlaceTile() //Called from TileMovement OnMouseUp
    {
        TileController tileInBoard = null;
        foreach (TileController tile in intersecticTiles) //Search for the tile in board
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
    void PlaceTileFromShopToBoard(TileController tileInBoard, TileController boughtTile)
    {
        RemoveMoney(SelectedTile.GetBuyingPrice());
        //BoardController.ReplaceTileInBoard(tileInBoard, boughtTile);
        StartCoroutine( BoardController.C_AddNewTile(boughtTile, tileInBoard.indexInBoard));
        ShopItem_Controller boughtItem = shopController.GetShopItem(SelectedTile);
        boughtItem.RemoveItem();

        shopController.UpdatePrices();
    }
    #endregion
    #region DAMAGE
    [Header("Enemy HP")]
    [SerializeField] float AcumulatedDamage;

    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    [SerializeField] Healthbar healthbar;
    public IEnumerator C_AddAcumulatedDamage(float amount)
    {
        //if (Mathf.Approximately(amount, 0)) { yield break; }

        AcumulatedDamage += amount;
        UpdateAcumulatedDamageDisplay();

        const float shakeDuration = .15f;
        TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30);
        yield return new WaitForSeconds(shakeDuration);
    }
    public float GetCurrentAcumulatedDamage() { return AcumulatedDamage; }
    IEnumerator DealTotalDamage() //sdfsdf
    {
        float totalDamage = AcumulatedDamage;
        Enemy_CurrentHP -= totalDamage;
        Enemy_CurrentHP = Mathf.Clamp(Enemy_CurrentHP, 0, Enemy_MaxHP);
        AcumulatedDamage = 0;

        TMP_AcumulatedDamage.text = $"<color=red>{MathJ.FloatToString(totalDamage, 1)}";
        UpdateEnemyHPBar();

        float shakeDuration = .5f ;
        Sequence shakeSequence = DOTween.Sequence();

        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOShakeRotation(shakeDuration, 30));
        shakeSequence.Join(TMP_AcumulatedDamage.rectTransform.DOScale(1.3f, shakeDuration / 2));
        shakeSequence.Append(TMP_AcumulatedDamage.rectTransform.DOScale(1f, shakeDuration / 2));

        yield return new WaitForSeconds(shakeDuration);
        

        if (Mathf.Approximately( Enemy_CurrentHP,0))
        {
            ChangeGameState(GameState.EncountersTransition);
        }
    }
    void UpdateAcumulatedDamageDisplay()
    {
        
        TMP_AcumulatedDamage.text = $"<color=white>{MathJ.FloatToString(AcumulatedDamage, 1)}";
    }
    void UpdateEnemyHPBar()
    {
        healthbar.UpdateHealthbar(Enemy_CurrentHP, Enemy_MaxHP);
    }
    public void SetNewEnemyMaxHP(float MaxHP)
    {
        Enemy_MaxHP = MaxHP;
        Enemy_CurrentHP = MaxHP;
        UpdateEnemyHPBar();
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
    }
    public int GetCurrentMoney() { return currentMoney; }
    public bool CanPurchase(int price) { return price <= currentMoney; }
    public bool CanPurchaseWithoutLosing(int price)
    {
        return price <= currentMoney;
    }
    void UpdateMoneyUI()
    {
        TMP_CurrentMoney.text = currentMoney.ToString();
    }
    #endregion
    #region ROLLS PER ENCOUNTER
    [Header("RollS")]
    public int MaxRollsPerEncounter = 6;
    public int RollsRemaining;
    public int MoneyPerRemainignRoll = 3;
    [SerializeField] TextMeshProUGUI TMP_Rolls;
    public void SetRemainingRolls(int amount)
    {
        RollsRemaining = amount;
        TMP_Rolls.text = $"{RollsRemaining}/{MaxRollsPerEncounter}";
    }
    #endregion
    private void Update()
    {
        GetIntersectingTilesToMouse();

        //Shift + 0 => Add 10 money
        //Shift + 1-9 => Move X Steps in board
        if (Keyboard.current[Key.LeftShift].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame) { AddMoney(10); }
  
            if (Keyboard.current[Key.Digit9].wasPressedThisFrame) { AttemptForceMovingState(9); }
            if (Keyboard.current[Key.Digit8].wasPressedThisFrame) { AttemptForceMovingState(8); }
            if (Keyboard.current[Key.Digit7].wasPressedThisFrame) { AttemptForceMovingState(7); }
            if (Keyboard.current[Key.Digit6].wasPressedThisFrame) { AttemptForceMovingState(6); }
            if (Keyboard.current[Key.Digit5].wasPressedThisFrame) { AttemptForceMovingState(5); }
            if (Keyboard.current[Key.Digit4].wasPressedThisFrame) { AttemptForceMovingState(4); }
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame) { AttemptForceMovingState(3); }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame) { AttemptForceMovingState(2); }
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame) { AttemptForceMovingState(1); }
        }

        void AttemptForceMovingState(int steps)
        {
            if (currentGameState == GameState.FreeMode)
            {
                dicesController.LastRolledValue = steps;
                ChangeGameState(GameState.MovingPlayer);
            }
        }
    }
   

}
