using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Splines;

public struct transformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector2Int vector;
}
public class Board_Controller_simple : MonoBehaviour
{
    [SerializeField] Tile_Profile Tile_Empty, Tile_Start, Tile_End, Tile_Oca;
    [SerializeField] TilesFactory factory;

    public List<TileController> TilesList = new();
    public List<TileTfData> TfData = new();
    public Dictionary<Vector2Int, TileController> TilesByPosition = new();
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
        TfData =  GetStrucrtData(StartingTilesCount);
        MoveTiles_ToTfData(false);
        yield return C_AnimateStartingTiles();


        PlayerIndex = 0;
        OnPlayerMoved?.Invoke(0, 0);
        playerSidePos = PlayerPrefab.transform.position;
        yield return V_StepPlayerToNewPos();
    }
    private List<TileController> InstantiateStartingTiles()
    {
        List<TileController> tempTiles = new();

        for (int i = 0; i < StartingTilesCount; i++)
        {
            Tile_Profile prefabToSpawn;
            if (i == 0) { prefabToSpawn = Tile_Start; }
            else if (i == StartingTilesCount - 1) { prefabToSpawn = Tile_End; }
            else if (i % 3 == 0) { prefabToSpawn = Tile_Oca; }
            //else if(i % 3 == 0) { prefabToSpawn = Tile_Money; }
            else { prefabToSpawn = Tile_Empty; }

            TileController newTile = factory.InstantiateTile(prefabToSpawn);
            tempTiles.Add(newTile);

        }
        return tempTiles;

    }
    IEnumerator C_AnimateStartingTiles()
    {

        float delayBetweenTiles = TimeToCreateBoard / TilesList.Count;

        foreach (TileController tile in TilesList)
        {
            tile.SetToTfData();
        }
        yield break;
        foreach (TileController tile in TilesList)
        {
            yield return new WaitForSeconds(delayBetweenTiles);
            tile.gameObject.SetActive(true);
            tile.tileMovement. FirstAppeareanceAnim();
        }
    }
    public void MoveTiles_ToTfData(bool withMovement)
    {
        TilesByPosition = new();
        for (int i = 0; i < TilesList.Count; i++)
        {
            TileController tile = TilesList[i];
            TileTfData tfStat = TfData[i];
            tile.SetOriginTfData(tfStat);
            
            tile.indexInBoard = i;
            tile.SetTileState(TileState.InBoard);
            if (withMovement)
            {
                tile.MoveToTfData();
            }
            else
            {
                tile.SetToTfData();
            }
        }
    }
    #endregion

    [SerializeField] SplineContainer spline;

    [SerializeField] float width = 1f;
    [SerializeField] float maxTileLenght = 3;
    [SerializeField] float ExtraLargePercent = 1.5f;


    private void OnDrawGizmosSelected()
    {
       List<TileTfData> temptructs = GetStrucrtData(StartingTilesCount);

        foreach (TileTfData info in temptructs)
        {
            Gizmos.color = Color.purple;

            for (int i = 0; i < info.cornersInWorld.Count; i++)
            {
                switch (i)
                {
                    case 0: Gizmos.DrawLine(info.cornersInWorld[0], info.cornersInWorld[1]); break;
                    case 1: Gizmos.DrawLine(info.cornersInWorld[1], info.cornersInWorld[3]); break;
                    case 2: Gizmos.DrawLine(info.cornersInWorld[2], info.cornersInWorld[0]); break;
                    case 3: Gizmos.DrawLine(info.cornersInWorld[3], info.cornersInWorld[2]); break;
                }
            }
        }
    }
    public List<TileTfData> GetStrucrtData(int tilesAmount)
    {
        List<TileTfData> tempList = new();
        List<Vector3> tilesCornersFromOrigin = new();
        List<Vector3> tilesOrigins = new();

        float smallT;
        float largeT;

        //if the total max lenght is larger than the whole spline, then divide. Else just use the lenght. 
        //We multiply by 2 because it's only the Start Tile and End Tile
        if ((maxTileLenght * ExtraLargePercent * 2) + maxTileLenght * (tilesAmount - 2) > spline.CalculateLength())
        {
            smallT = 1f / (ExtraLargePercent * 2 + (tilesAmount - 2));
            largeT = smallT * ExtraLargePercent;
        }
        else
        {
            smallT = GetTWithLenght(maxTileLenght);
            largeT = GetTWithLenght(maxTileLenght * ExtraLargePercent);
        }

        //Get corners from all Origins
        float totalT = 0;
        for (int i = 0; i < tilesAmount + 1; i++)
        {
            float thisT = totalT;

            Vector3 tan = spline.EvaluateTangent(thisT);
            Vector3 forward = tan.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            tilesCornersFromOrigin.Add(right * width / 2);
            tilesCornersFromOrigin.Add(-right * width / 2);
            tilesOrigins.Add(spline.EvaluatePosition(thisT));

            float nextT;
            if (i == 0 || i == tilesAmount - 1) { nextT = largeT; }
            else { nextT = smallT; }
            totalT += nextT;

        }
        totalT = 0;
        //Get the basic info In and add the corners 
        for (int i = 0; i < tilesAmount; i++)
        {
            TileTfData newTileInfo = new();
            

            float thisT;
            if (i == 0 || i == tilesAmount - 1) { thisT = largeT; }
            else { thisT = smallT; }

            float centerT = totalT + (thisT / 2);

            newTileInfo.center = spline.EvaluatePosition(centerT);

            Vector3 tan = spline.EvaluateTangent(centerT);
            newTileInfo.forward = tan.normalized;
            newTileInfo.up = Vector3.up;
            newTileInfo.right = Vector3.Cross(newTileInfo.up, newTileInfo.forward);

            newTileInfo.origin = tilesOrigins[i];
            newTileInfo.rotation = Quaternion.LookRotation(newTileInfo.forward, Vector3.up);

            //Get the corners from Origin
            Vector3 difBetweenOrigins = tilesOrigins[i + 1] - tilesOrigins[i];
            int startingCornerIndex = i * 2;

            newTileInfo.cornersInWorld = new();
            newTileInfo.cornersInLocal = new();
            for (int j = 0; j < 4; j++)
            {
                //The order of the mesh vertices is:
                //3-----1
                //|     |
                //2-----0
                //But when we created the corners they were in this order:
                //1-----3
                //|     |
                //0-----2
                //So here we are sorting them in the proper order in the struct
                Vector3 sortedWorldPos;
                switch (j)
                {
                    case 0: sortedWorldPos = newTileInfo.origin + difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 2]; break;
                    case 1: sortedWorldPos = newTileInfo.origin + difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 3]; break;
                    case 2: sortedWorldPos = newTileInfo.origin + tilesCornersFromOrigin[startingCornerIndex]; break;
                    case 3: sortedWorldPos = newTileInfo.origin + tilesCornersFromOrigin[startingCornerIndex + 1]; break;
                    default: sortedWorldPos = Vector3.zero; break;
                }

                //Now we collect the World and Local positions out of that mess
                newTileInfo.cornersInWorld.Add(sortedWorldPos);

                Vector3 localPos = MathJ.worldToLocal2D(
                    newTileInfo.cornersInWorld[j],
                    newTileInfo.center,
                    newTileInfo.right,
                    newTileInfo.forward);

                newTileInfo.cornersInLocal.Add(localPos);
            }
            totalT += thisT;
            tempList.Add(newTileInfo);
        }
        return tempList;
        Debug.Log(TfData.Count);
        //
        float GetTWithLenght(float lenght)
        {
            float totalLenght = spline.CalculateLength();
            return Mathf.InverseLerp(0, totalLenght, lenght);
        }
    }
    public void UpdateStructData()
    {
        TfData = GetStrucrtData(TilesList.Count);
    }

    #region ASSEMBLE/DISASSEMBLE BOARD
    public bool isBoardAssembled = true;
    const float disassembledHeight = 7;
    const float maxRandonTime = 0.3f;
    const float assembleTime = 1.5f;
    public IEnumerator C_AsembleBoard()
    {
        isBoardAssembled = true;
        for (int i = 0;i <TilesList.Count; i++)
        {
            TileController tile = TilesList[i];
            Vector3 finalPos = TfData[i].center;
            Sequence seq = DOTween.Sequence().
                   AppendInterval(Random.Range(0, maxRandonTime)).
                   Append(tile.transform.DOMove(finalPos, assembleTime)).SetEase(Ease.InOutCubic);
        }
        yield return new WaitForSeconds(assembleTime + maxRandonTime);
        yield return V_JumpPlayerToNewPos();
    }
    public IEnumerator C_DisasembleBoard()
    {
        isBoardAssembled = false;
        yield return C_JumpPlayerToSide();

        foreach(TileController tile in TilesList)
        {
            Vector3 finalPos = new Vector3(tile.transform.position.x, disassembledHeight, tile.transform.position.z);
            Sequence seq = DOTween.Sequence().
                    AppendInterval(Random.Range(0, maxRandonTime)).
                    Append(tile.transform.DOMove(finalPos, assembleTime)).SetEase(Ease.InOutCubic);
        }
        yield return new WaitForSeconds(assembleTime + maxRandonTime);

    }
    Vector3 playerSidePos;//this position is set at the starting board
    IEnumerator C_JumpPlayerToSide()
    {
        const float duration = .5f;

        float jumpHeight = 1;
        Sequence seq =
            DOTween.Sequence().
                Append(PlayerPrefab.transform.DOJump(
                    playerSidePos,
                    jumpHeight,
                    1,
                    duration
                    ));

        ;
        yield return new WaitForSeconds(duration);
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
        TileController thisTile = TilesList[PlayerIndex];

        yield return TilesList[PlayerIndex].OnPlayerLanded();
    }
    public IEnumerator L_JumpPlayerTo(int IndexOfTile, bool triggerLanded)
    {
        if(IndexOfTile < 0) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = 0; }
        if(IndexOfTile > TilesList.Count - 1) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = TilesList.Count - 1; }

        int originalIndex = PlayerIndex;
        PlayerIndex = IndexOfTile;
        OnPlayerMoved?.Invoke(originalIndex, PlayerIndex);

        yield return V_JumpPlayerToNewPos();
        V_ShakePlayer();
        if(triggerLanded)
        {
            GameController_Simple.Instance.remainingStepsToTake = 1;
            yield return TilesList[PlayerIndex].OnPlayerStepped();
            GameController_Simple.Instance.remainingStepsToTake = 0;
            yield return L_LandPlayerInCurrentPos();
        }
        else
        {
            yield return TilesList[PlayerIndex].OnPlayerStepped();
        }
    }
    public IEnumerator JumpPlayerToStartTile()
    {
        yield return L_JumpPlayerTo(0, true);
    }
    #endregion
    #region PLAYER VISUALS
    IEnumerator V_StepPlayerToNewPos()//step the player to new pos
    {
        const float duration = 0.25f;
        Vector3 newPos = TilesList[PlayerIndex].TfData.center;

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
    IEnumerator V_JumpPlayerToNewPos()
    {
        const float duration = .5f;
        Vector3 newPos = TilesList[PlayerIndex].TfData.center;

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
    public void ReplaceTileInBoard(TileController oldTileInBoard, TileController newTile)
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
    }
    public void AddNewTile(TileController tile, int index)
    {
        TilesList.Insert(index, tile);

        TfData = GetStrucrtData(TilesList.Count);

        MoveTiles_ToTfData(true);
        if (PlayerIndex >= index) { PlayerIndex++; }
        StartCoroutine(V_StepPlayerToNewPos());

        tile.SetTileState(TileState.InBoard);
        tile.OnPlacedInBoard();

        tile.transform.parent = tilesHolder;

        //actualitzar escala (mes endavant)
    }  
    public void RemoveTile(int index)
    {
        TileController tileToRemove = TilesList[index];
        tileToRemove.OnRemovedFromBoard();
        TilesList.RemoveAt(index);

        Destroy(tileToRemove.gameObject);

        TfData = GetStrucrtData(TilesList.Count);
        MoveTiles_ToTfData(true);
        if(index <= PlayerIndex) { PlayerIndex--; }
        StartCoroutine(V_StepPlayerToNewPos());
    }
    public void MoveTileInBoard(int from, int to)
    {
        TileController tileMoved = TilesList[from];
        TilesList.RemoveAt(from);
        TilesList.Insert(to, tileMoved );

        MoveTiles_ToTfData(true);
    }
    #endregion

}
