using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class Board_Controller_simple : MonoBehaviour
{
    [SerializeField] Tile_Profile Tile_Empty, Tile_Start, Tile_End, Tile_Oca;
    [SerializeField] TilesFactory factory;

    public List<TileController> TilesList = new();
    public TileController GetCurrentPlayerTile(int extra = 0) { return TilesList[PlayerIndex+extra]; }
    public List<TileTfData> TfData = new();
    public Dictionary<Vector2Int, TileController> TilesByPosition = new();
    public int PlayerIndex 
    {
        get { return _playerIndex; }
        set
        {
            int prevValue = _playerIndex; 
            _playerIndex = value;
            OnPlayerIndexSet?.Invoke(prevValue,_playerIndex);
        }
    }

    int _playerIndex;

    [SerializeField]Transform tilesHolder;
    public static Board_Controller_simple Instance;
    private void Awake()
    {
        Instance = this;
    }
    [Header("Player")]
    [SerializeField] GameObject PlayerPrefab;

    [Header("Board visualization")]
    public int StartingTilesCount = 9;
    [SerializeField] float TimeToCreateBoard;
    public UnityEvent<int, int> OnPlayerIndexSet; //(from, to)
    public UnityEvent OnBoardModified;
    public UnityEvent<TileController> OnAddedTile;
    public UnityEvent<TileController> OnRemovedTile;

    #region STARTING BOARD CREATION
    public IEnumerator StartBoard() //called from game controller
    {
        TilesList = InstantiateStartingTiles();
        foreach(TileController tile in TilesList)
        {
            tile.transform.SetParent(tilesHolder);
            yield return tile.C_OnPlacedInBoard(); 
        }

        TfData =  GetStrucrtData();
        MoveTiles_ToTfData(false);
        yield return C_AnimateStartingTiles();


        PlayerIndex = 0;
        yield return L_JumpPlayerTo(0, false);
        playerSidePos = PlayerPrefab.transform.position;
        yield return V_StepPlayerToNewPos();
    }
    private List<TileController> InstantiateStartingTiles()
    {
        List<TileController> tempTiles = new();

        for (int i = 0; i < StartingTilesCount; i++)
        {
            Tile_Profile profileToSpawn;
            if (i == 0) { profileToSpawn = Tile_Start; }
            else if (i == StartingTilesCount - 1) { profileToSpawn = Tile_End; }
            else if (i % 4 == 0) { profileToSpawn = Tile_Oca; }
            //else if(i % 3 == 0) { prefabToSpawn = Tile_Money; }
            else { profileToSpawn = Tile_Empty; }

            TileController newTile = factory.InstantiateTile(profileToSpawn);
            tempTiles.Add(newTile);

        }
        return tempTiles;

    }
    IEnumerator C_AnimateStartingTiles()
    {

        float delayBetweenTiles = TimeToCreateBoard / TilesList.Count;

        foreach (TileController tile in TilesList)
        {
            tile.gameObject.SetActive(false);
        }
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
        OnBoardModified?.Invoke();
    }
    #endregion

    [SerializeField] SplineContainer spline;

    [SerializeField] float width = 1f;
    [SerializeField] float maxTileLenght = 3;
    [SerializeField] float ExtraLargePercent = 1.5f;
    [SerializeField] float ExtraSmallPercent = .75f;
    [SerializeField] float ExtraBigPercent = 1.25f;


    private void OnDrawGizmosSelected()
    {
        /*
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
        */
    }
    public List<TileTfData> GetStrucrtData()
    {
        List<TileTfData> tempList = new();
        List<Vector3> tilesCornersFromOrigin = new();
        List<Vector3> tilesOrigins = new();

        int tilesAmount = TilesList.Count;

        float smallT;
        float mediumT;
        float bigT;
        float largeT;

        int smallTilesCount = 0, mediumTilesCount = 0, largeTilesCount = 0, bigTilesCount = 0;

        for (int i = 0; i < tilesAmount; i++)
        {
            Tile_Profile profile = TilesList[i]._Profile;
            switch (profile.tileSize)
            {
                case TileSize.Small: smallTilesCount++; break;
                case TileSize.Medium: mediumTilesCount++; break;
                case TileSize.Big: bigTilesCount++; break;
                case TileSize.Large: largeTilesCount++; break;
            }
        }
        //if the total max lenght is larger than the whole spline, then divide. Else just use the lenght. 
        //Calculate if its small enough to fit
        if ((maxTileLenght * ExtraLargePercent * largeTilesCount) + 
            (maxTileLenght * mediumTilesCount) + 
            (maxTileLenght * ExtraBigPercent * bigTilesCount) +
            (maxTileLenght * ExtraSmallPercent * smallTilesCount)
            > spline.CalculateLength())
        {
            Debug.Log("01 test");
            mediumT = 1f / ((ExtraLargePercent * largeTilesCount) + mediumTilesCount + (ExtraSmallPercent * smallTilesCount));
            smallT = mediumT * ExtraSmallPercent;
            bigT = mediumT * ExtraBigPercent;
            largeT = mediumT * ExtraLargePercent;
            Debug.Log("02 test: "+ mediumT);
        }
        else
        {
            mediumT = GetTWithLenght(maxTileLenght);
            smallT = GetTWithLenght(maxTileLenght * ExtraSmallPercent);
            bigT = GetTWithLenght(maxTileLenght* ExtraBigPercent);
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

            if (i == tilesAmount) { totalT = 1; break; }
            float nextT = 0;
            Tile_Profile profile = TilesList[i]._Profile;
            switch (profile.tileSize)
            {
                case TileSize.Small: nextT = smallT; break;
                case TileSize.Medium: nextT = mediumT; break;
                    case TileSize.Big: nextT = bigT; break;
                case TileSize.Large: nextT = largeT; break;
            }
            totalT += nextT;

        }
        totalT = 0;

        //Get the basic info In and add the corners 
        for (int i = 0; i < tilesAmount; i++)
        {
            TileTfData newTileInfo = new();


            float thisT = 0;
            Tile_Profile profile = TilesList[i]._Profile;
            switch (profile.tileSize)
            {
                case TileSize.Small: thisT = smallT; break;
                case TileSize.Medium: thisT = mediumT; break;
                case TileSize.Big: thisT = bigT; break;
                case TileSize.Large: thisT = largeT; break;
            }

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
            newTileInfo.cornersInLocalWithoutRotation = new();
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

                newTileInfo.cornersInLocalWithoutRotation.Add(sortedWorldPos - newTileInfo.center);
            }
            //newTileInfo.cornersInLocalWithoutRotation = sortVertexByLowest(newTileInfo.cornersInLocalWithoutRotation);
            totalT += thisT;
            tempList.Add(newTileInfo);
        }
        return tempList;

        //
        float GetTWithLenght(float lenght)
        {
            float totalLenght = spline.CalculateLength();
            return Mathf.InverseLerp(0, totalLenght, lenght);
        }
        /*
        //Doesn't really work something is wrong i no tinc ganes de arreglarho
        List<Vector3> sortVertexByLowest(List<Vector3> list)
        {
            float AToB = (list[0].z + list[1].z) / 2;
            float BToC = (list[1].z + list[2].z) / 2;
            float CToD = (list[2].z + list[3].z) / 2;
            float DToA = (list[3].z + list[0].z) / 2;
            float[] centers = new float[] { AToB, BToC, CToD, DToA };
            int lowestIndex = -1;
            float lowestValue = float.MaxValue;
            for (int i = 0; i < centers.Length; i++)
            {
                if (centers[i] < lowestValue)
                {
                    lowestValue = centers[i];
                    lowestIndex = i;
                }
            }
            return rotateList(list, lowestIndex);
        }
        List<Vector3> rotateList(List<Vector3> list, int startIndex)
        {
            List<Vector3> rotated = new();
            rotated.AddRange(list.GetRange(startIndex, list.Count - startIndex));
            rotated.AddRange(list.GetRange(0, startIndex));
            return rotated;
        }
        */
    }
    public void UpdateStructData()
    {
        TfData = GetStrucrtData();
    }

    #region ASSEMBLE/DISASSEMBLE BOARD
    public bool isBoardAssembled = true;
    const float disassembledHeight = 30;
    const float maxRandonTime = 0.3f;
    const float assembleTime = 1.5f;
    public IEnumerator C_AsembleBoard()
    {
        CamerasManager cameras = CamerasManager.instance;
        cameras.SetCameraPriority("CinemachineCamera_Board", 15);

        isBoardAssembled = true;
        foreach (TileController tile in TilesList)
        {
            tile.gameObject.SetActive(true);
        }
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
        cameras.SetCameraPriority("CinemachineCamera_Board", 0);
    }
    public IEnumerator C_DisasembleBoard()
    {
        CamerasManager cameras = CamerasManager.instance;
        cameras.SetCameraPriority("CinemachineCamera_Board", 15);
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
        foreach (TileController tile in TilesList)
        {
            tile.gameObject.SetActive(false);
        }
        cameras.SetCameraPriority("CinemachineCamera_Board", 0);
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
    public IEnumerator L_LandPlayerInCurrentPos()
    {
        Debug.Log($"Landed in:{PlayerIndex}");
        V_ShakePlayer();
        TileController currentTile = GetCurrentPlayerTile();

        yield return currentTile.OnPlayerLanded();
        yield return currentTile.OnTileFinished();
    }
    public IEnumerator L_JumpPlayerTo(int IndexOfTile, bool triggerLanded)
    {
        if(IndexOfTile < 0) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = 0; }
        if(IndexOfTile > TilesList.Count - 1) { Debug.LogWarning($"WARNING: {IndexOfTile} is not a valid index to jump"); IndexOfTile = TilesList.Count - 1; }

        int originalIndex = PlayerIndex;
        PlayerIndex = IndexOfTile;
        TilesList[originalIndex]._Profile.OnSteppedOut();

        TileController endTile = GetCurrentPlayerTile();
        endTile._Profile.remainingSteps--;
        yield return V_JumpPlayerToNewPos();
        V_ShakePlayer();
        if(triggerLanded)
        {
            yield return endTile.OnPlayerStepped();
            yield return L_LandPlayerInCurrentPos();
        }
        else
        {
            yield return endTile.OnPlayerStepped();
            yield return endTile.OnTileFinished();
        }
    }
    #endregion
    #region PLAYER VISUALS
    public IEnumerator V_StepPlayerToNewPos()//step the player to new pos
    {
        Debug.Log("Step anim");
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
    public IEnumerator V_JumpPlayerToNewPos()
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
    public IEnumerator V_AirbornePlayer()
    {
        TileController currentTile = GetCurrentPlayerTile(1);
        const float duration = .2f;
        PlayerPrefab.transform.DOMove(
            currentTile.TfData.origin + (Vector3.up * .5f),
            duration);
        yield return new WaitForSeconds(duration);
    }

    #endregion
    #region BOARD EDITING
    public void ReplaceTileInBoard(TileController oldTileInBoard, TileController newTile) //Not really used
    {
        oldTileInBoard.C_OnRemovedFromBoard();
        TilesList[oldTileInBoard.indexInBoard] = newTile;
        TilesByPosition[oldTileInBoard.vectorInBoard] = newTile;
        newTile.indexInBoard = oldTileInBoard.indexInBoard;
        newTile.vectorInBoard = oldTileInBoard.vectorInBoard;

        newTile.transform.parent = transform;

        newTile.SetOriginTfData(oldTileInBoard.TfData);
        newTile.MoveToTfData();
        newTile.SetTileState(TileState.InBoard);

        newTile.C_OnPlacedInBoard(); //This is a coroutine so It wont work

        Destroy(oldTileInBoard.gameObject);
    }
    public IEnumerator C_AddNewTile(TileController tile, int index)
    {
        InsertTile(tile, index);
        
        UpdateStructData();
        MoveTiles_ToTfData(true);
        if (PlayerIndex > index) { PlayerIndex++; }
        
        yield return V_StepPlayerToNewPos();
        tile.SetTileState(TileState.InBoard);
        tile.CheckForDraggability(0,0); //per alguna raó he de ficar aixo aqui quan ja s'executa al SetTileState. Si no ho fico no pilla el draggabiility be si es coloca sobre el player

        yield return tile.C_OnPlacedInBoard();
        OnAddedTile?.Invoke(tile);

        tile.transform.parent = tilesHolder;

        yield return GameController_Simple.Instance.OnAddedNewTileToBoard_CardEffect.C_ActivateEffects(tile);

        GameController_Simple.Instance.shopController.UpdatePrices();
    }  
    public IEnumerator C_RemoveTile(int index)
    {
        TileController tileToRemove = TilesList[index];
        bool isPlayerTile = index == PlayerIndex;
        bool isLandedTile = tileToRemove == GameController_Simple.Instance.GetTileToLand();

        yield return tileToRemove.C_OnRemovedFromBoard();
        RemoveTile(index);
        OnRemovedTile?.Invoke(tileToRemove);

        Destroy(tileToRemove.gameObject);

        UpdateStructData();
        MoveTiles_ToTfData(true);

        
        if(index <= PlayerIndex) { PlayerIndex--; }
        if (isPlayerTile && isLandedTile)
        {
            GetCurrentPlayerTile()._Profile.remainingSteps = 0;
            yield return V_StepPlayerToNewPos();
        }
        else if(isPlayerTile)
        {
            yield return V_AirbornePlayer();
        }

    }
    public void MoveTileInBoard(int from, int to)
    {
        MoveTile(from, to);

        if(from > PlayerIndex && to <= PlayerIndex) { PlayerIndex++; }
        if(from < PlayerIndex && to > PlayerIndex) { PlayerIndex--; }


        UpdateStructData();
        MoveTiles_ToTfData(true);
    }
    #endregion
    #region TILES LIST EDITING
    List<(int,TileController)> removeAndGetUnmovables(int exception = -1) //get all unmovables registered and remove them from the list. We return the list for future reinsertion
    {
        List<(int, TileController)> unmovibleTiles = new List<(int, TileController)>();
        for (int i = 0; i < TilesList.Count; i++)
        {
            if(i == exception) { continue; }
            TileController tile = TilesList[i];
            if (tile._Profile.genericSkills.Contains(GenericSkills.Unmovable))
            {
                unmovibleTiles.Add((i, tile));
            }
        }
        foreach (var (index,tile) in unmovibleTiles)
        {
            TilesList.Remove(tile);
        }
        return unmovibleTiles;
    }
    void reinsertUnmovables(List<(int, TileController)> unmovables)
    {
        foreach (var (index,tile) in unmovables)
        {
            int insertIndex = index;
            if (insertIndex > TilesList.Count - 2)
            {
                insertIndex = TilesList.Count - 2;
            }
            TilesList.Insert(insertIndex, tile);
        }
    }
    int adjustIndex( int originalIndex, List<(int index, TileController)> unmovables)
    {
        int shift = 0;
        foreach (var (idx, _) in unmovables)
        {
            if (idx < originalIndex)
                shift++;
        }
        return originalIndex - shift;
    }
    void InsertTile(TileController newTile,int newIndex)
    {
        List<(int, TileController)> unmovibleTiles = removeAndGetUnmovables();
        newIndex = adjustIndex(newIndex, unmovibleTiles);

        TilesList.Insert(newIndex, newTile);

        reinsertUnmovables(unmovibleTiles);

    }
    void RemoveTile(int indexToRemove)
    {
        List<(int, TileController)> unmovibleTiles = removeAndGetUnmovables(indexToRemove);

        indexToRemove = adjustIndex(indexToRemove, unmovibleTiles);

        TilesList.RemoveAt(indexToRemove);

        reinsertUnmovables(unmovibleTiles);
    }
    void MoveTile(int from, int to)
    {
        List<(int, TileController)> unmovibleTiles = removeAndGetUnmovables();

        
        from = adjustIndex(from, unmovibleTiles);
        to = adjustIndex(to, unmovibleTiles);

        TileController tileMoved = TilesList[from];

        TilesList.RemoveAt(from);
        TilesList.Insert(to, tileMoved);

        reinsertUnmovables(unmovibleTiles);
    }


    #endregion

}
