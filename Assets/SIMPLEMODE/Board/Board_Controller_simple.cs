using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;


public class Board_Controller_simple : MonoBehaviour
{
    [SerializeField] GameObject Tile_Empty, Tile_Start, Tile_End, Tile_Oca;

    public List<Tile_Base> TilesList = new();
    public Dictionary<Vector2Int, Tile_Base> TilesByPosition = new();
    public int PlayerIndex { get; private set; }

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
    public UnityEvent<int, int> OnPlayerMoved; //(from, to)

    [SerializeField] List<Vector2Int> vectorsInOrder = new(); //this is created to store the vectors when creating the undertiles and then passing the info into the tilesByPosition dictionary
    [SerializeField] Vector3 tilePosOffset;

    #region STARTING BOARD CREATION
    public IEnumerator StartBoard() //called from game controller
    {
        TilesList = CreateStartingBoardData();
        CreateUnderTiles();
        PlaceTilesOverUnderTile();
        yield return AnimateStartingTiles();

        PlayerIndex = 0;
        OnPlayerMoved?.Invoke(0, 0);
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
            //else if(i % 3 == 0) { prefabToSpawn = Tile_Money; }
            else { prefabToSpawn = Tile_Empty; }

            SpawnAndAddNewTile(prefabToSpawn, i);
        }
        return tempTiles;

        void SpawnAndAddNewTile(GameObject tilePrefab, int index)
        {
            GameObject newTile = Instantiate(tilePrefab, transform);
            if(newTile.TryGetComponent(out Tile_Base tileInfo))
            {
                tileInfo.indexInBoard = index;
                tempTiles.Add(tileInfo);
                tileInfo.UpdateTileVisuals();
                tileInfo.SetTileState(TileState.InBoard);
            }
            else { Debug.LogError("ERROR: TilePrefab is missing a controller"); }
        }
    }
   
    [Header("UnderTiles")]
    [SerializeField] GameObject UnderTilePrefab_Start;
    [SerializeField] GameObject UnderTilePrefab_Straight;
    [SerializeField] GameObject UnderTilePrefab_Curve;
    [SerializeField] Color undertile_StartColor, undertile_EndColor;
    public enum UnderTileTypes { Start, Straight, Curve }
    List<GameObject> underTilesList;
    void CreateUnderTiles()
    {
        underTilesList = new();

        int HeightCount = Height;
        int WidthCount = Width;
        int totalTilesCount = HeightCount * WidthCount;

        bool movingHorizontaly = true; //if false, means we are moving vertically
        int iterationsNeeded = (Mathf.Min(HeightCount, WidthCount) * 2) - 1;
        int placedTilesCount = 0;

        Vector3Int movingDirection = Vector3Int.right;
        Quaternion tileRotation = Quaternion.identity;
        Quaternion halfRotation = Quaternion.AngleAxis(-90, Vector3.up);
        Vector3 nextTilePosition = transform.position;
        Vector2Int CurrentVector = new Vector2Int(0, 0);

        for (int t = 0; t < iterationsNeeded + 1; t++)
        {
            if (movingHorizontaly)
            {
                if(WidthCount == 0) { return; }

                PlaceInDirection(WidthCount);
                RotateForNextDirection();

                HeightCount--;
                movingHorizontaly = false;
            }
            else
            {
                if(HeightCount == 0) { return; }

                PlaceInDirection(HeightCount);
                RotateForNextDirection();

                WidthCount--;
                movingHorizontaly = true;
            }
        }

        //
        void RotateForNextDirection()
        {
            tileRotation = halfRotation * tileRotation;
            movingDirection = new Vector3Int(-movingDirection.z, 0, movingDirection.x);
            //movingDirection = MathJ.rotateVectorUnclockwise90Degrees(movingDirection);
            nextTilePosition += (Vector3)movingDirection * distanceBetweenTiles;
            CurrentVector += new Vector2Int(movingDirection.x, movingDirection.z);
        }
        void  PlaceInDirection(int Count)
        {
            for (int r = 0; r < Count; r++)
            {
                if(placedTilesCount == 0) //START tile
                { 
                    InstantiateUnderTile(UnderTileTypes.Start, nextTilePosition, tileRotation);
                    nextTilePosition += (Vector3)movingDirection * distanceBetweenTiles;
                    CurrentVector += new Vector2Int(movingDirection.x,movingDirection.z);
                    continue; 
                }
                if(placedTilesCount == totalTilesCount-1) //END tile
                {
                    tileRotation = halfRotation * tileRotation;
                    tileRotation = halfRotation * tileRotation;
                    InstantiateUnderTile(UnderTileTypes.Start, nextTilePosition, tileRotation);
                    return; }


                if (r == Count - 1) //last tile in the series
                {
                    InstantiateUnderTile(UnderTileTypes.Curve,nextTilePosition,tileRotation);
                    
                }
                else //straight tile
                {
                    InstantiateUnderTile(UnderTileTypes.Straight, nextTilePosition, tileRotation);
                    nextTilePosition += (Vector3)movingDirection * distanceBetweenTiles;
                    CurrentVector += new Vector2Int(movingDirection.x, movingDirection.z);
                }
            }
        }
        void InstantiateUnderTile(UnderTileTypes undertileType, Vector3 position, Quaternion rotation)
        {
            placedTilesCount++;
            GameObject prefabToSpawn = null;
            switch (undertileType)
            {
                case UnderTileTypes.Start:
                    prefabToSpawn = UnderTilePrefab_Start;
                    break;
                case UnderTileTypes.Straight:
                    prefabToSpawn = UnderTilePrefab_Straight;
                    break;
                case UnderTileTypes.Curve:
                    prefabToSpawn = UnderTilePrefab_Curve;
                    break;
            }
            GameObject newUndertile = Instantiate(prefabToSpawn, position, rotation,transform);
            newUndertile.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(undertile_StartColor, undertile_EndColor, (float)placedTilesCount / (float)totalTilesCount);

            underTilesList.Add(newUndertile);
            vectorsInOrder.Add(CurrentVector);
        }
    }
    void PlaceTilesOverUnderTile()
    {
        for (int i = 0; i < underTilesList.Count; i++)
        {
            Tile_Base thisTile = TilesList[i];
            TileMovement movement = thisTile.tileMovement;
            movement.SetOriginTransformWithStats(new transformStats
            {
                position = underTilesList[i].transform.position + tilePosOffset,
                rotation = Quaternion.identity,
                scale = underTilesList[i].transform.localScale
            });

            movement.PlaceTileInOrigin();

            TilesByPosition.Add(vectorsInOrder[i], thisTile);
            thisTile.vectorInBoard = vectorsInOrder[i];
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

        OnPlayerMoved?.Invoke(PlayerIndex - stepAmount, PlayerIndex);

        yield return V_StepPlayerToNewPos();
        yield return TilesList[PlayerIndex].OnPlayerStepped();
    }
    public IEnumerator L_LandPlayerInCurrentPos()
    {
        Debug.Log($"Landed in:{PlayerIndex}");
        V_ShakePlayer();
        Tile_Base thisTile = TilesList[PlayerIndex];

        yield return TilesList[PlayerIndex].OnPlayerLanded();
    }
    public IEnumerator L_JumpPlayerTo(int IndexOfTile, bool triggerLanded)
    {
        if(IndexOfTile < 0) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = 0; }
        if(IndexOfTile > TilesList.Count - 1) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = TilesList.Count - 1; }
        int originalIndex = PlayerIndex;
        PlayerIndex = IndexOfTile;
        OnPlayerMoved?.Invoke(originalIndex, PlayerIndex);

        yield return V_JumpPlayerToNewPos(originalIndex);
        V_ShakePlayer();
        yield return TilesList[PlayerIndex].OnPlayerStepped();
        if (triggerLanded) yield return L_LandPlayerInCurrentPos();
        
    }
    public IEnumerator JumpPlayerToStartTile()
    {
        yield return L_JumpPlayerTo(0, true);
        yield return GameController_Simple.Instance.OnReachedStartTile_CardEffects.ActivateEffects();
    }
    #endregion
    #region PLAYER VISUALS
    IEnumerator V_StepPlayerToNewPos()//step the player to new pos
    {
        const float duration = 0.25f;
        Vector3 newPos = TilesList[PlayerIndex].tileMovement.originTransform.position;

        float jumpHeight = .5f;
        Sequence seq =
            DOTween.Sequence().
                Append(PlayerPrefab.transform.DOJump(
                    newPos,
                    jumpHeight,
                    1,
                    duration
                    ));

                ;
        yield return new WaitForSeconds(duration);
    }
    IEnumerator V_JumpPlayerToNewPos(int startingIndex)
    {
        const float duration = .5f;
        Vector3 newPos = TilesList[PlayerIndex].tileMovement.originTransform.position;

        float jumpHeight = 1;
        Sequence seq =
            DOTween.Sequence().
                Append(PlayerPrefab.transform.DOJump(
                    newPos,
                    jumpHeight,
                    1,
                    duration
                    ));

        ;
        yield return new WaitForSeconds(duration);
    }
    void V_ShakePlayer()
    {
        PlayerPrefab.transform.DOShakePosition(0.2f, .1f, 1);
    }

    #endregion

}
