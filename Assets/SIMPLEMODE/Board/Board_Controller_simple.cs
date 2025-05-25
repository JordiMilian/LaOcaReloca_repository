using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Board_Controller_simple : MonoBehaviour
{
    [SerializeField] GameObject Tile_Empty, Tile_Start, Tile_End, Tile_Oca, Tile_Money;
    public List<Tile_Base> TilesList = new();
    int PlayerIndex;

    public static Board_Controller_simple Instance;
    private void Awake()
    {
        Instance = this;
    }
    [Header("Player")]
    [SerializeField] GameObject PlayerPrefab;

    [Header("Board visualization")]
    public int Width;
    public int Height;
    [SerializeField] float distanceBetweenTiles;
    [SerializeField] float TimeToCreateBoard;

    #region STARTING BOARD CREATION
    public IEnumerator StartBoard() //called from game controller
    {
        TilesList = CreateStartingBoardData();
        PlaceTiles();
        yield return AnimateStartingTiles();

        PlayerIndex = 0;
        yield return V_StepPlayerToNewPos();
    }
    List<Tile_Base> CreateStartingBoardData()
    {
        int totalTilesCount = Width * Height;

        List<Tile_Base> tempTiles = new();

        for (int i = 0; i < totalTilesCount; i++)
        {
            GameObject prefabToSpawn;
            if (i == 0) { prefabToSpawn = Tile_Start; }
            else if (i == totalTilesCount - 1) { prefabToSpawn = Tile_End; }
            else if (i % 4 == 0) { prefabToSpawn = Tile_Oca; }
            else if(i % 3 == 0) { prefabToSpawn = Tile_Money; }
            else { prefabToSpawn = Tile_Empty; }

            SpawnAndAddNewTile(prefabToSpawn, i);
        }
        return tempTiles;

        void SpawnAndAddNewTile(GameObject tilePrefab, int index)
        {
            GameObject newTile = Instantiate(tilePrefab);
            if(newTile.TryGetComponent(out Tile_Base tileInfo))
            {
                tileInfo.indexInBoard = index;
                tempTiles.Add(tileInfo);
            }
            else { Debug.LogError("ERROR: TilePrefab is missing a controller"); }
        }
    }
    public List<Tile_Oca> GetAllOcaTiles()
    {
        List<Tile_Oca> ocaTiles = new();
        foreach(Tile_Base tile in TilesList)
        {
            if(tile is Tile_Oca) { ocaTiles.Add(tile as Tile_Oca); }
        }
        return ocaTiles;

    }
    void PlaceTiles()
    {
        int HeightCount = Height;
        int WidthCount = Width;
        int totalTilesCount = HeightCount * WidthCount;

        bool movingHorizontaly = true; //if false, means we are moving vertically
        int iterationsNeeded = (Mathf.Min(HeightCount, WidthCount) * 2) - 1;
        int placedTilesCount = 0;

        Vector2Int movingDirection = Vector2Int.right;
        Quaternion tileRotation = Quaternion.identity;
        Quaternion halfRotation = Quaternion.AngleAxis(45, Vector3.forward);
        Vector2 nextTilePosition = transform.position;

        for (int t = 0; t < iterationsNeeded + 1; t++)
        {
            if (movingHorizontaly)
            {
                if (!PlaceInDirection(WidthCount)) //try to spawn, if negative(count 0), then finish
                {
                    return;
                }
                ;
                RotateForNextDirection();

                HeightCount--;
                movingHorizontaly = false;
            }
            else
            {
                if (!PlaceInDirection(HeightCount))
                {
                    return;
                }

                RotateForNextDirection();

                WidthCount--;
                movingHorizontaly = true;
            }
        }
        void PlaceNewTile()
        {
            Tile_Base thisTile = TilesList[placedTilesCount];
            thisTile.transform.position = nextTilePosition;
            thisTile.transform.rotation = tileRotation;

            placedTilesCount++;

            Debug.Log($"Spawned Tile {placedTilesCount}/{totalTilesCount}");
        }
        void RotateForNextDirection()
        {
            tileRotation = halfRotation * tileRotation;
            movingDirection = MathJ.rotateVectorUnclockwise90Degrees(movingDirection);
            nextTilePosition += (Vector2)movingDirection * distanceBetweenTiles;
        }
        bool PlaceInDirection(int Count)
        {
            if (Count == 0) { return false; }

            for (int r = 0; r < Count; r++)
            {
                // if its the last in the series, rotate and spawn, else spawn and make a step
                if (r == Count - 1)
                {
                    if (placedTilesCount != totalTilesCount - 1) //do not rotate the exit piece
                    {
                        tileRotation = halfRotation * tileRotation;
                    }
                    PlaceNewTile();
                }
                else
                {
                    PlaceNewTile();
                    nextTilePosition += (Vector2)movingDirection * distanceBetweenTiles;
                }
            }
            return true;
        }
    }
    IEnumerator AnimateStartingTiles()
    {
        float delayBetweenTiles = TimeToCreateBoard / TilesList.Count;

        foreach (Tile_Base tile in TilesList)
        {
            tile.gameObject.SetActive(false);
        }
        foreach (Tile_Base tile in TilesList)
        {
            yield return new WaitForSeconds(delayBetweenTiles);
            tile.gameObject.SetActive(true);
            tile.FirstAppeareanceAnim();
        }
    }
    #endregion
    #region MAIN PUBLIC METHODS FOR BOARD MOVEMENT
    public IEnumerator L_StepPlayer(bool positiveStep) //If false, its negative step
    {
        if (positiveStep && PlayerIndex == TilesList.Count - 1) { yield break; }
        if (!positiveStep && PlayerIndex == 0) { yield break; }

        int stepAmount = positiveStep ? 1 : -1;
        PlayerIndex += stepAmount;

        yield return V_StepPlayerToNewPos();
        yield return TilesList[PlayerIndex].OnPlayerStepped();
    }
    public IEnumerator L_LandPlayerInCurrentPos()
    {
        Debug.Log($"Landed in:{PlayerIndex}");
        V_ShakePlayer();
        Tile_Base thisTile = TilesList[PlayerIndex];
        yield return GameController_Simple.Instance.AddAcumulatedDamage(thisTile.GetLandedDamageAmount());
        yield return TilesList[PlayerIndex].OnPlayerLanded();
    }
    public IEnumerator L_JumpPlayerTo(int IndexOfTile, bool triggerLanded)
    {
        if(IndexOfTile < 0) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = 0; }
        if(IndexOfTile > TilesList.Count - 1) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = TilesList.Count - 1; }
        int originalIndex = PlayerIndex;
        PlayerIndex = IndexOfTile;

        yield return V_JumpPlayerToNewPos(originalIndex);
        V_ShakePlayer();
        yield return TilesList[PlayerIndex].OnPlayerStepped();
        if (triggerLanded) yield return L_LandPlayerInCurrentPos();
        
    }
    #endregion
    #region PLAYER VISUALS
    IEnumerator V_StepPlayerToNewPos()//step the player to new pos
    {
        const float duration = 0.25f;
        Vector3 newPos = TilesList[PlayerIndex].transform.position;

        Sequence seq =
            DOTween.Sequence().
                Append(PlayerPrefab.transform.DOMove(newPos, duration)).SetEase(Ease.InCubic)
                ;
        yield return new WaitForSeconds(duration);
    }
    IEnumerator V_JumpPlayerToNewPos(int startingIndex)//move the player but jump over all the tiles that skiped. Maybe implement it later, not prioritized
    {
        yield return V_StepPlayerToNewPos();//placeholder, pls make it cooler
        yield break;
    }
    void V_ShakePlayer()
    {
        PlayerPrefab.transform.DOShakePosition(0.2f, .1f, 1);
    }

    #endregion
   
}
