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
public class GameController_C : MonoBehaviour
{
    public static GameController_C Instance;
    private void Awake()
    {
        Instance = this;
    }

    Board_Data BoardData;
    public Board_Controller BoardController;
    
    [SerializeField] int BoardWidth = 10;
    [SerializeField] int BoardHeight = 10;

    [Header("Board display")]
    float delayBetweenTiles;
    [SerializeField] GameObject TilePrefab;
    [SerializeField] float distanceBetweenTiles = 1;
    [SerializeField] Color startColor, endColor;
    [SerializeField] float TimeToCreateBoard = 3;
    [SerializeField] Transform Tf_BoardParent;
    List<GameObject> SpawnedTiles = new List<GameObject>();
    
    public GameState currentGameState = GameState.Null;

    [Header("Enemy")]
    [SerializeField] float Enemy_MaxHP;
    [SerializeField] float Enemy_CurrentHP;

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
                //Receive money
                //Display some victory screen
                //return to start and change to free mode
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
        CreateEmptyBoard();
        yield return StartCoroutine(AppearStartingBoard(BoardData));

        //introduce the enemy 
        //introduce the player
        ChangeGameState(GameState.FreeMode);
    }
    public void CreateEmptyBoard()
    {
        int totalTilesCount = BoardWidth * BoardHeight;
        List<Tile_Data> EmptyTiles = new List<Tile_Data>();
        BoardData = new Board_Data(EmptyTiles, 0, 0, 0);
        for (int i = 0; i < totalTilesCount; i++)
        {
            if(i == 0) { EmptyTiles.Add(new StartingTile(i, BoardData)); continue; }
            else if(i == totalTilesCount - 1) { EmptyTiles.Add(new EndTile(i, BoardData)); continue;}
            else if(i%4 == 0) { EmptyTiles.Add(new OcaTile(i, BoardData)); continue; }

            EmptyTiles.Add(new EmptyTile(i, BoardData));
        }
        BoardData = new Board_Data(EmptyTiles, 0, BoardWidth, BoardHeight);
    }
    IEnumerator AppearStartingBoard(Board_Data board)
    {
        int HeightCount = board.Height;
        int WidthCount = board.Width;
        int totalTilesCount = BoardWidth * BoardHeight;

        bool movingHorizontaly = true; //if false, means we are moving vertically
        int iterationsNeeded = (Mathf.Min(HeightCount, WidthCount) * 2) - 1;
        int spawnedTilesCount = 0;

        Vector2Int movingDirection = Vector2Int.right;
        Quaternion tileRotation = Quaternion.identity;
        Quaternion halfRotation = Quaternion.AngleAxis(45, Vector3.forward);
        Vector2 nextTilePosition = Tf_BoardParent.position;

        delayBetweenTiles = TimeToCreateBoard / totalTilesCount;

        for (int t = 0; t < iterationsNeeded + 1; t++)
        {
            if (movingHorizontaly)
            {
                yield return StartCoroutine(SpawnInDirection(WidthCount));
                RotateForNextDirection();

                HeightCount--;
                movingHorizontaly = false;
            }
            else
            {
                yield return StartCoroutine(SpawnInDirection(HeightCount));
                RotateForNextDirection();

                WidthCount--;
                movingHorizontaly = true;
            }
        }
        void SpawnNewTile(Tile_Data tileType)
        {
            GameObject newTile = Instantiate(TilePrefab, nextTilePosition, tileRotation, Tf_BoardParent);
            Color tileColor = Color.Lerp(startColor, endColor, (float)spawnedTilesCount / (float)totalTilesCount);
            if (tileType is StartingTile) { tileColor = Color.white; }
            else if (tileType is EndTile) { tileColor = Color.black; }
            else if(tileType is OcaTile) { tileColor = Color.magenta; }

                newTile.GetComponent<SpriteRenderer>().color = tileColor;
            SpawnedTiles.Add(newTile);
            spawnedTilesCount++;
            Debug.Log($"Spawned Tile {spawnedTilesCount}/{totalTilesCount}");
        }
        void RotateForNextDirection()
        {
            tileRotation = halfRotation * tileRotation;
            movingDirection = MathJ.rotateVectorUnclockwise90Degrees(movingDirection);
            nextTilePosition += (Vector2)movingDirection * distanceBetweenTiles;
        }
        IEnumerator SpawnInDirection(int Count)
        {
            if (Count == 0) { yield break; }

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
                yield return new WaitForSeconds(delayBetweenTiles);
            }
        }
        yield return null;
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
        int rolledAmount = GetRandomDiceNumber();
        TMP_rolledDiceAmount.text = rolledAmount.ToString();
        yield return StartCoroutine(MovePlayer_VisualCoroutine(rolledAmount));
        ChangeGameState(GameState.FreeMode);
    }
    #endregion
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(BoardController.JumpPlayerTo(0));

        ChangeGameState(GameState.FreeMode);
    }
    #endregion


    #region TEST ROLL
    [Header("Test roll")]
    [SerializeField] TextMeshProUGUI TMP_rolledDiceAmount;
    [SerializeField] Button RollDiceButton;

    public void UI_RollDice()
    {
        ChangeGameState(GameState.MovingPlayer);
    }
    int GetRandomDiceNumber()
    {
        return UnityEngine.Random.Range(1, 6);
    }
    IEnumerator MovePlayer_VisualCoroutine(int amount)
    {
        for (int i = 0; i < Mathf.Abs(amount); i++)
        {
            yield return StartCoroutine(BoardController.StepPlayer(amount > 0));
        }
        yield return StartCoroutine(BoardController.LandPlayerInCurrentPos());
    }

    #endregion
}
