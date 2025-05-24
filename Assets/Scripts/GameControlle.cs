using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum GameState
{
    Null, StartBoard, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied
}
public class GameControlle : MonoBehaviour
{
    public static GameControlle Instance;
    private void Awake()
    {
        Instance = this;
    }

    Board_Data BoardData;
    public Board_Controller BoardController;
    Board_View BoardView;
    
    [SerializeField] int BoardWidth = 10;
    [SerializeField] int BoardHeight = 10;

    [Header("Board display")]
    [SerializeField] GameObject TilePrefab;
    [SerializeField] float distanceBetweenTiles = 1;
    [SerializeField] Color startColor, endColor;
    [SerializeField] float TimeToCreateBoard = 3;
    [SerializeField] Transform Tf_BoardParent;
    
    public GameState currentGameState = GameState.Null;

    [Header("References")]
    [SerializeField] Dices_Controller dicesControler;
    #region ENEMY HP
    [Header("Enemy HP")]
    [SerializeField] float AcumulatedDamage;
    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;
    [SerializeField] Slider Slider_EnemyHp;
    [SerializeField] TextMeshProUGUI TMP_EnemyHp;
    [SerializeField] TextMeshProUGUI TMP_AcumulatedDamage;
    void UpdateEnemyHpUI()
    {
        Slider_EnemyHp.maxValue = Enemy_MaxHP;
        Slider_EnemyHp.value = Enemy_CurrentHP;
        TMP_EnemyHp.text = $"Enemy HP: {Enemy_CurrentHP.ToString("F2")}/{Enemy_MaxHP.ToString("F2")}";
        TMP_AcumulatedDamage.text = $"Acumulated damage:{AcumulatedDamage}";
    }
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
    #endregion

    private void Start()
    {
        ChangeGameState(GameState.StartBoard);
    }
     //MOST IMPORTANT METHOD OF THE CLASS. CONTROLS THE FLOW OF THE TURN
    public void ChangeGameState(GameState newState)
    {
        //On EXIT this State
        switch (currentGameState)
        {
            case GameState.StartBoard:
                //Nothing?
                break;
            case GameState.MovingPlayer:
                if(regularMovingCoroutine != null) { StopCoroutine(regularMovingCoroutine); }
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

    #region START BOARD
    IEnumerator StartGameCoroutine()
    {
        BoardData = CreateStartingBoardData();
        List<Tile_Controller> tempTiles = InstantiateTiles(BoardData);
        BoardController.InitializeBoard( BoardData,tempTiles);
        yield return BoardView.AnimateStartingBoard();
        yield return BoardController.JumpPlayerTo(BoardData.PlayerIndex,false);

        //introduce the enemy 
        //introduce the player
        ChangeGameState(GameState.FreeMode);
    }
    public Board_Data CreateStartingBoardData()
    {
        int totalTilesCount = BoardWidth * BoardHeight;

        Board_Data tempData = new Board_Data(new List<Tile_Data>(),BoardWidth, BoardHeight,0);
        for (int i = 0; i < totalTilesCount; i++)
        {
            if(i == 0) { tempData.TilesList.Add(new StartingTile(i, tempData)); continue; }
            else if(i == totalTilesCount - 1) { tempData.TilesList.Add(new EndTile(i, tempData)); continue;}
            else if(i%4 == 0) { tempData.TilesList.Add(new OcaTile(i, tempData)); continue; }

            tempData.TilesList.Add(new EmptyTile(i, tempData));
        }
        return tempData;
    }
    List<Tile_Controller> InstantiateTiles(Board_Data board)
    {
        int HeightCount = board.Height;
        int WidthCount = board.Width;
        int totalTilesCount = HeightCount * WidthCount;

        bool movingHorizontaly = true; //if false, means we are moving vertically
        int iterationsNeeded = (Mathf.Min(HeightCount, WidthCount) * 2) - 1;
        int spawnedTilesCount = 0;

        Vector2Int movingDirection = Vector2Int.right;
        Quaternion tileRotation = Quaternion.identity;
        Quaternion halfRotation = Quaternion.AngleAxis(45, Vector3.forward);
        Vector2 nextTilePosition = BoardController.transform.position;

        List<Tile_Controller> appearedTiles = new List<Tile_Controller>();

        for (int t = 0; t < iterationsNeeded + 1; t++)
        {
            if (movingHorizontaly)
            {
                if (!SpawnInDirection(WidthCount)) //try to spawn, if negative(count 0), then finish
                {
                    InitBoardController();
                    return appearedTiles;
                }
                ;
                RotateForNextDirection();

                HeightCount--;
                movingHorizontaly = false;
            }
            else
            {
                if (!SpawnInDirection(HeightCount)) 
                { 
                    InitBoardController();
                    return appearedTiles; 
                }

                RotateForNextDirection();

                WidthCount--;
                movingHorizontaly = true;
            }
        }
        return appearedTiles;
        void SpawnNewTile(Tile_Data tileType)
        {
            GameObject newTile = Instantiate(TilePrefab, nextTilePosition, tileRotation, BoardController.transform);
            Tile_Controller tileController = newTile.GetComponent<Tile_Controller>();
            //call the tileController to update the visuals of the Tile

            appearedTiles.Add(tileController);

            Color tileColor = Color.Lerp(startColor, endColor, (float)spawnedTilesCount / (float)totalTilesCount);
            if (tileType is StartingTile) { tileColor = Color.white; }
            else if (tileType is EndTile) { tileColor = Color.black; }
            else if (tileType is OcaTile) { tileColor = Color.magenta; }
            newTile.GetComponentInChildren<SpriteRenderer>().color = tileColor;

            spawnedTilesCount++;
            Debug.Log($"Spawned Tile {spawnedTilesCount}/{totalTilesCount}");
        }
        void RotateForNextDirection()
        {
            tileRotation = halfRotation * tileRotation;
            movingDirection = MathJ.rotateVectorUnclockwise90Degrees(movingDirection);
            nextTilePosition += (Vector2)movingDirection * distanceBetweenTiles;
        }
         bool SpawnInDirection(int Count)
        {
            if (Count == 0) { return false; }

            for (int r = 0; r < Count; r++)
            {
                // if its the last in the series, rotate and spawn, else spawn and make a step
                if (r == Count - 1)
                {
                    if (spawnedTilesCount != totalTilesCount - 1) //do not rotate the exit piece
                    {
                        tileRotation = halfRotation * tileRotation;
                    }
                    SpawnNewTile(board.TilesList[spawnedTilesCount]);
                }
                else
                {
                    SpawnNewTile(board.TilesList[spawnedTilesCount]);
                    nextTilePosition += (Vector2)movingDirection * distanceBetweenTiles;
                }
            }
            return true;
        }
        void InitBoardController()
        {
            BoardController.InitializeBoard(BoardData, appearedTiles);
        }
    }
    
    #endregion
    #region MOVING PLAYER 
    Coroutine regularMovingCoroutine;
    //This mode is entered when the dices are rolled
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    IEnumerator OnMovingPlayer_Coroutine()
    {
        RollDiceButton.interactable = false;
        int rolledAmount = dicesControler.RollDices();
        TMP_rolledDiceAmount.text = rolledAmount.ToString();

        for (int i = 0; i < Mathf.Abs(rolledAmount); i++)
        {
            yield return BoardController.StepPlayer(rolledAmount > 0);
        }
        yield return BoardController.LandPlayerInCurrentPos();

        yield return DealAcumulatedDamage();

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
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return DealAcumulatedDamage();
        yield return StartCoroutine(BoardController.JumpPlayerTo(0,false));

        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region KILLED ENEMY
    IEnumerator KilledEnemy_Coroutine()
    {
        yield return BoardController.JumpPlayerTo(0, false);
        Enemy_MaxHP *= 1.2f;
        Enemy_CurrentHP = Enemy_MaxHP;
        UpdateEnemyHpUI();
        ChangeGameState (GameState.FreeMode);
    }
    #endregion

    #region ROLL BUTTON
    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;
    public void UI_RollDice()
    {
        ChangeGameState(GameState.MovingPlayer);
    }


    #endregion
}
