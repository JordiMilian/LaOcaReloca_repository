using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public enum GameState
{
    Empty, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied, EncountersTransition
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
    public CardEffectsDelegate OnReachedStartTile_CardEffects = new();

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
            case GameState.MovingPlayer:
                currentStateCoroutine = StartCoroutine(OnMovingPlayer_Coroutine());
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
        dicesController.EnableRollButtons();
        shopController.EnableShop();
        if(BoardController.PlayerIndex == BoardController.TilesList.Count -1)
        {
            ChangeGameState(GameState.ReachedEnd);
        }
    }
    void OnFreeModeExit()
    {
        dicesController.DisableRollButtons();
        shopController.DisableShop();
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
        yield return dicesController.RollDicesCoroutine();
        remainingStepsToTake = dicesController.LastRolledValue;

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

        yield return DealTotalDamage();

        ChangeGameState(GameState.FreeMode); 
            
    }
    public void ChangeStateToMoving()
    {
        ChangeGameState(GameState.MovingPlayer);
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

    IEnumerator C_LoadNextEncounter()
    {
        if (currentEncounterObject != null)
        {
            yield return currentEncounter.OnEncounterExit();
            Destroy(currentEncounterObject);
            currentEncounterObject = null;
        }
        currentEncounterIndex++;
        currentEncounterObject = Instantiate(EncountersPrefabs[currentEncounterIndex], transform);
        currentEncounter = currentEncounterObject.GetComponent<IEncounter>();
        yield return currentEncounter.OnEncounterEnter();

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
            ChangeGameState(GameState.EncountersTransition);
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
