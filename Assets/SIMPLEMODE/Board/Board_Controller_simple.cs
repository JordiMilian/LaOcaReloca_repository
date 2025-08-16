using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public struct transformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector2Int vector;
}
public class Board_Controller_simple : MonoBehaviour
{
    [SerializeField] GameObject Tile_Empty, Tile_Start, Tile_End, Tile_Oca;

    public List<Tile_Base> TilesList = new();
    public List<transformData> TfData = new();
    public Dictionary<Vector2Int, Tile_Base> TilesByPosition = new();
    public int PlayerIndex { get; private set; }

    Transform tilesHolder, UnderTilesHolder;
    public static Board_Controller_simple Instance;
    private void Awake()
    {
        Instance = this;
        tilesHolder = transform.Find("TilesHolder");
        UnderTilesHolder = transform.Find("UndertilesHolder");
    }
    [Header("Player")]
    [SerializeField] GameObject PlayerPrefab;

    [Header("Board visualization")]
    public int StartingTilesCount = 9;
    [SerializeField] float boardSideSize = 6;
    [SerializeField] float TimeToCreateBoard;
    public UnityEvent<int, int> OnPlayerMoved; //(from, to)


    #region STARTING BOARD CREATION
    public IEnumerator StartBoard() //called from game controller
    {
        TilesList = InstantiateStartingTiles();
        UpdateTfData();
        MoveTiles_ToTfData(false);
        UpdateUndertiles_ByTfData();
        yield return Co_AnimateStartingTiles();


        PlayerIndex = 0;
        OnPlayerMoved?.Invoke(0, 0);
        yield return V_StepPlayerToNewPos();
    }
    private List<Tile_Base> InstantiateStartingTiles()
    {
        List<Tile_Base> tempTiles = new();

        for (int i = 0; i < StartingTilesCount; i++)
        {
            GameObject prefabToSpawn;
            if (i == 0) { prefabToSpawn = Tile_Start; }
            else if (i == StartingTilesCount - 1) { prefabToSpawn = Tile_End; }
            else if (i % 4 == 0) { prefabToSpawn = Tile_Oca; }
            //else if(i % 3 == 0) { prefabToSpawn = Tile_Money; }
            else { prefabToSpawn = Tile_Empty; }

            SpawnAndAddNewTile(prefabToSpawn, i);
        }
        return tempTiles;

        void SpawnAndAddNewTile(GameObject tilePrefab, int index)
        {
            GameObject newTile = Instantiate(tilePrefab, tilesHolder);
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
    public void UpdateTfData()
    {
        TfData = new();

        Vector3Int moveingDirection = Vector3Int.left;

        Vector3 nextPosition = transform.position;
        Vector2Int nextVector = new Vector2Int(0, 0);
        Quaternion nextRotation = Quaternion.identity;
        Quaternion halfRotation = Quaternion.AngleAxis(90, Vector3.up);

        int tilesPerSide = GetMinSquareSides(TilesList.Count);
        float scaleMultiplier = boardSideSize / (float)tilesPerSide;
        float distanceBetween = scaleMultiplier;
        if(tilesPerSide % 2 == 0) 
        {
            nextPosition += new Vector3(distanceBetween / 2, 0, -distanceBetween/2);        
        }
        int amountToMove = 1;
        int amountMovedInDirection = 0;

        for (int i = TilesList.Count - 1; i >= 0; i--)
        {
            transformData newTfStat = new();

            //place tile in this position
            newTfStat.position = nextPosition;
            newTfStat.rotation = nextRotation;
            newTfStat.scale = Vector3.one * scaleMultiplier;
            newTfStat.vector = nextVector;
            TfData.Add(newTfStat);


            //rotate if reached end
            if (amountMovedInDirection == amountToMove)
            {
                if (moveingDirection == Vector3Int.forward || moveingDirection == Vector3Int.back)
                {
                    amountToMove++;
                }
                amountMovedInDirection = 0;
                moveingDirection = rotateVectorClockwise90Degrees(moveingDirection);

                nextRotation = halfRotation * nextRotation;
            }

            //move next position
            nextPosition += (Vector3)moveingDirection * distanceBetween;
            nextVector += BoardVector3ToVector2Int(moveingDirection);

            amountMovedInDirection++;

        }
        TfData.Reverse();

        //
        Vector3Int rotateVectorClockwise90Degrees(Vector3Int VectorToRotate)
        {
            return new Vector3Int(VectorToRotate.z, 0, -VectorToRotate.x);
        }
        Vector2Int BoardVector3ToVector2Int(Vector3Int v) { return new Vector2Int(v.x, v.z); }
        int GetMinSquareSides(int tilesCount)
        {
            int maxSides = 10;
            for (int i = 2; i < maxSides; i++)
            {
                if (tilesCount <= Mathf.Pow(i, 2))
                {
                    return i;
                }
            }
            return maxSides; //return the max square side if tiles are too many
        }
    }

    [Header("UnderTiles")]
    [SerializeField] GameObject UnderTilePrefab_Start;
    [SerializeField] GameObject UnderTilePrefab_Straight;
    [SerializeField] GameObject UnderTilePrefab_Curve;
    [SerializeField] Color undertile_StartColor, undertile_EndColor;
    [SerializeField] float underTiles_VerticalOffset = -0.25f;
    List<GameObject> undertiles_List = new();
    public enum UnderTileTypes { Start, Straight, Curve }
    IEnumerator Co_AnimateStartingTiles()
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
            tile.tileMovement. FirstAppeareanceAnim();
        }
    }
    void UpdateUndertiles_ByTfData()
    {
        for(int i = undertiles_List.Count - 1; i >= 0; i--)
        {
            Destroy(undertiles_List[i]);
        }

        InstantiateUnderTile(UnderTileTypes.Start, TfData[0].position, TfData[0].rotation,0);

        for (int i = 1;i < TfData.Count;i++)
        {
            transformData thisTf = TfData[i];
            if(i == TfData.Count -1)
            {
                Quaternion finalQuaterion = thisTf.rotation * Quaternion.AngleAxis(180, Vector3.up);
                InstantiateUnderTile(UnderTileTypes.Start, thisTf.position, finalQuaterion, i);
            }
            else if(thisTf.rotation != TfData[i - 1].rotation)
            {
                InstantiateUnderTile(UnderTileTypes.Curve, thisTf.position,thisTf.rotation, i);
            }
            else
            {
                InstantiateUnderTile(UnderTileTypes.Straight, thisTf.position, thisTf.rotation, i);
            }
        }
        void InstantiateUnderTile(UnderTileTypes undertileType, Vector3 position, Quaternion rotation, int index)
        {
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
            Vector3 underTilePos = position + (Vector3.up * underTiles_VerticalOffset);
            GameObject newUndertile = Instantiate(prefabToSpawn, underTilePos, rotation, UnderTilesHolder);
            newUndertile.transform.localScale = TfData[index].scale;
            undertiles_List.Add(newUndertile);
            newUndertile.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(undertile_StartColor, undertile_EndColor, (float)index / (float)TfData.Count);

        }
    }
    public void MoveTiles_ToTfData(bool withMovement)
    {
        TilesByPosition = new();
        for (int i = 0; i < TilesList.Count; i++)
        {
            Tile_Base tile = TilesList[i];
            transformData tfStat = TfData[i];
            tile.tileMovement.SetOriginTransformWithStats(tfStat);
            tile.indexInBoard = i;
            tile.vectorInBoard = tfStat.vector;
            TilesByPosition.Add(tfStat.vector, tile);
            if (withMovement)
            {
                tile.tileMovement.MoveTileToOrigin();
            }
            else
            {
                tile.tileMovement.PlaceTileInOrigin();
            }
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
    #region BOARD EDITING
    public void ReplaceTileInBoard(Tile_Base oldTileInBoard, Tile_Base newTile)
    {
        oldTileInBoard.OnRemovedFromBoard();
        TilesList[oldTileInBoard.indexInBoard] = newTile;
        TilesByPosition[oldTileInBoard.vectorInBoard] = newTile;
        newTile.indexInBoard = oldTileInBoard.indexInBoard;
        newTile.vectorInBoard = oldTileInBoard.vectorInBoard;

        newTile.transform.parent = transform;

        newTile.tileMovement.SetOriginTransformWithStats(oldTileInBoard.tileMovement.originTransform);
        newTile.tileMovement.MoveTileToOrigin();
        newTile.SetTileState(TileState.InBoard);

        newTile.OnPlacedInBoard();

        Destroy(oldTileInBoard.gameObject);

        newTile.UpdateTileVisuals();//Esto sobre casi segur
    }
    public void AddNewTile(Tile_Base tile, int index)
    {
        TilesList.Insert(index, tile);

        UpdateTfData();

        MoveTiles_ToTfData(true);
        UpdateUndertiles_ByTfData();
        if (PlayerIndex >= index) { PlayerIndex++; }
        StartCoroutine(V_StepPlayerToNewPos());

        tile.SetTileState(TileState.InBoard);
        tile.OnPlacedInBoard();

        tile.transform.parent = tilesHolder;

        //actualitzar escala (mes endavant)
    }  
    public void RemoveTile(int index)
    {
        Tile_Base tileToRemove = TilesList[index];
        TilesList.RemoveAt(index);

        Destroy(tileToRemove.gameObject);

        UpdateTfData();
        MoveTiles_ToTfData(true);
        UpdateUndertiles_ByTfData();
        if(index < PlayerIndex) { PlayerIndex--; }
        StartCoroutine(V_StepPlayerToNewPos());
    }
    public void MoveTileInBoard(int from, int to)
    {
        Tile_Base tileMoved = TilesList[from];
        TilesList.RemoveAt(from);
        TilesList.Insert(to, tileMoved );

        MoveTiles_ToTfData(true);
    }
    #endregion

}
