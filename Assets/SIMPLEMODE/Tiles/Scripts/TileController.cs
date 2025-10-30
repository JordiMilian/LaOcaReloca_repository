using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour, IBuyable, ITooltip
    ,IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

{
    [HideInInspector] public TileState tileState = TileState.none;

    [HideInInspector] public int indexInBoard;
    [HideInInspector] public Vector2Int vectorInBoard;
    public Material tileMaterial;

    //Basic references to other scripts
    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    [HideInInspector] public TileSharedVisuals tileMovement;

    [SerializeField] Transform zeroRotationTf;

    public Tile_Profile _Profile;

    #region NEW TF DATA
    [Header("Mesh references")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    public TileTfData TfData { get; private set; }
    public void SetOriginTfData(TileTfData tileData)
    {
        TfData = tileData;
    }
    public void SetToTfData()
    {
        transform.position = TfData.center;
        //transform.rotation = TfData.rotation;

        //zeroRotationTf.localRotation = Quaternion.Inverse(TfData.rotation);

        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(TfData.cornersInLocalWithoutRotation);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;

        RecalculateTextureScaling();
    }
    public void MoveToTfData()
    {
        StartCoroutine(moving());

        IEnumerator moving()
        {
            float timer = 0;
            const float movingTime = .5f;
            transform.DOMove(TfData.center, movingTime).SetEase(Ease.OutBack);

            RecalculateTextureScaling();
            while (timer < movingTime)
            {
                timer += Time.deltaTime;

                List<Vector3> newVerts = new();
                for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
                {
                    Vector3 targetPos = TfData.cornersInLocalWithoutRotation[i];
                    Vector3 currentPos = meshFilter.mesh.vertices[i];
                    Vector3 lerpedPos = Vector3.Lerp(currentPos, targetPos, timer / movingTime);
                    newVerts.Add(lerpedPos);
                }
                meshFilter.mesh.SetVertices(newVerts);
                

                //Rotation
                //Quaternion targetRot = TfData.rotation;
                //Quaternion currentRot = transform.rotation;
                //Quaternion lerpedRot = Quaternion.Lerp(currentRot, targetRot, timer / movingTime);
                //zeroRotationTf.localRotation = Quaternion.Inverse(lerpedRot);
                //transform.rotation = lerpedRot;
                yield return null;
            }

            SetToTfData();
        }
    }

    void RecalculateTextureScaling()
    {
        Vector3 furthestVertex = Vector2.zero;
        float furthestDistance = 0;
        foreach(Vector3 point in TfData.cornersInLocalWithoutRotation)
        {
            float distance = point.sqrMagnitude;
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestVertex = point;
            }
            Debug.DrawLine(TfData.center, TfData.center + point, Color.white, 2);
        }

        float t = MathJ.GetSquare1Intersection(furthestVertex);
        tileMaterial.SetFloat("_uvsMultiplier", t);
        Debug.DrawLine(TfData.center, TfData.center + furthestVertex * t,Color.red, 2);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3[] squarePos = new Vector3[]
        {
            new Vector3(.5f,0,.5f),
            new Vector3(.5f,0,-.5f),
            new Vector3(-.5f,0,-.5f),
            new Vector3(-.5f,0,.5f)
        };
        Gizmos.DrawLine(TfData.center + squarePos[0], TfData.center + squarePos[1]);
        Gizmos.DrawLine(TfData.center + squarePos[1], TfData.center + squarePos[2]);
        Gizmos.DrawLine(TfData.center + squarePos[2], TfData.center + squarePos[3]);
        Gizmos.DrawLine(TfData.center + squarePos[3], TfData.center + squarePos[0]);
    }
    #endregion
    public void SetTileProfile(Tile_Profile profile)
    {
        _Profile = Instantiate(profile);
        _Profile._Tile = this;
        tileMaterial.SetColor("_OutlineColor", _Profile.tileColor);
        if(_Profile.tileTexture != null)
        {
            tileMaterial.SetTexture("_mainTexture", _Profile.tileTexture);
        }
        tileMovement.UpdateDmgDisplayText();
        _Profile.Initialize();
    }

    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;
        tileMovement = GetComponent<TileSharedVisuals>();

        tileMaterial = Instantiate(tileMaterial);
        GetComponent<MeshRenderer>().material = tileMaterial;
    }
    #region DAMAGE MODIFIERS
    public List<float> DamagesToDeal = new();
    public virtual float GetBaseDamage()
    {
        return _Profile.BaseDamage;
    }
    public void SetBaseDamage(float newDamage)
    {
        _Profile.BaseDamage = newDamage;
        tileMovement.UpdateDmgDisplayText();
    }
    public virtual void AddBaseDamage(float dmgToAdd)
    {
        float addedDmg = _Profile.AddBaseDamage(dmgToAdd);
        
        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage("+" + MathJ.FloatToString(addedDmg, 1), TileMessageType.AddBaseDamage);
    }
    public void RemoveBaseDamage(float damageToRemove)
    {
       float removedDmg = _Profile.RemoveBaseDamage(damageToRemove);

        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage($"-{removedDmg}", TileMessageType.AddBaseDamage);
    }
    public void MultiplyBaseDamage(float mult)
    {
        _Profile.MultiplyBaseDamage(mult);

        tileMovement.shakeTile(Intensity.mid);
        tileMovement.DisplayMessage($"x{mult}", TileMessageType.AddBaseDamage);
    }
    public IEnumerator C_DealAllDamageToDeal()
    {
        if(DamagesToDeal.Count == 0) { yield break; }

        float totalDamage = 0;

        string displayMessage = "";
        for (int i = 0; i < DamagesToDeal.Count; i++)
        {
            displayMessage += MathJ.FloatToString(DamagesToDeal[i], 1);
            if (i != DamagesToDeal.Count - 1) { displayMessage += "+"; }
        }
        tileMovement.DisplayMessage(displayMessage, TileMessageType.DealDamage);

        foreach (float dmg in DamagesToDeal) { totalDamage += dmg; }
        yield return GameController.C_AddAcumulatedDamage(totalDamage);

        DamagesToDeal.Clear();
    }
    #endregion
    public void SetTileState(TileState newState)
    {
        if(newState == tileState) { return; }

        //EXIT
        switch(tileState)
        {
            case TileState.InBoard:
                BoardController.OnPlayerMoved.RemoveListener(CheckForDraggability);
                break;
        }

        //ENTER
        switch (newState)
        {
            case TileState.none:
                break;
            case TileState.InShop: 
                canBeMoved = true;
                break;
            case TileState.InBoard:
                if(_Profile is Tile_End || _Profile is Tile_Start) { canBeMoved = false; break; }
                canBeMoved = true;
                BoardController.OnPlayerMoved.AddListener(CheckForDraggability);
                CheckForDraggability(0, BoardController.PlayerIndex);
                break;
        }
        tileState = newState;
    }

    [HideInInspector] public bool isBehindPlayer;
    void CheckForDraggability(int from, int to)
    {
        isBehindPlayer = BoardController.PlayerIndex >= indexInBoard;
        if (isBehindPlayer) { tileMovement.SetBasicPanelColor_Transparent(); }
        else { tileMovement.SetBasicPanelColor(); }
    }
    #region MAIN VIRTUAL LOGIC METHODS

    public IEnumerator OnPlayerStepped()
    {
        //Add visual and sound feedback
       yield return _Profile.OnPlayerStepped();
    }
    public IEnumerator OnPlayerLanded()
    {
        //Add more visual and sound feedback
        tileMovement.shakeTile(Intensity.mid);

        yield return _Profile.OnPlayerLanded(); 
    }
    public void OnPlacedInBoard() { _Profile.OnPlacedInBoard(); }
    public void OnRemovedFromBoard() { _Profile.OnRemovedFromBoard(); }

    public string GetTooltipText()
    {
        return _Profile.GetTooltipText();
    }
    #endregion
    #region BUY/SELL
    
    public virtual int GetBuyingPrice()
    {
        int repeatedCards = 0;
        foreach (TileController tile in BoardController.TilesList)
        {
            if (tile._Profile.GetType() == _Profile.GetType()) { repeatedCards++; }
        }

        int baseValue;
        switch (_Profile.rarity)
        {
            case Rarity.Common: { baseValue = 2; break; }
            case Rarity.Rare: { baseValue = 4; break; }
            case Rarity.Legendary: { baseValue = 10; break; }
            case Rarity.Unique: { return _Profile.uniquePrice; }
            default: { Debug.LogError("ERROR: Pls set a valid rarity to this Tile"); return 0; }
        }
        return MathJ.GetFibonacciValue(baseValue, repeatedCards);

    }
    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        SetTileProfile(TilesFactory.instance.GetRandomProfile());
        SetOriginTfData(new TileTfData(shopItemController.buyablePositionTf));
        SetToTfData();
        SetTileState(TileState.InShop);
    }
    public void OnEnablePurchase()
    {
        canBeMoved = true;
    }

    public void OnDisablePurchase()
    {
        canBeMoved = false;
    }
    #endregion
    #region TOOLTIPS
    public void OnPointerEnter(PointerEventData eventData) { RequestTooltip(); }
    public void OnPointerExit(PointerEventData eventData) { StopRequestTooltip(); }

    void StopRequestTooltip() { TooltipManager.Instance.RemoveRequest(this); }
    void RequestTooltip() { TooltipManager.Instance.RequestTooltip(this); }
    void ForceTooltip() { TooltipManager.Instance.ForceTooltip(this); }
    void StopForcingThisTooltip() { TooltipManager.Instance.StopForcingThisTooltip(this); }
    public string GetTooltipDescription()
    {
        return _Profile.GetTooltipText();
    }
    public string GetTooltipTitle()
    {
        return _Profile.Title;
    }
    public Texture GetTooltipTexture()
    {
        return _Profile.tileTexture;
    }
    #endregion
    #region DRAGGING
    public bool canBeMoved = true;
    Coroutine draggingCoroutine;
    [SerializeField] float heightWhileDragged = 1;
    public void OnPointerDown(PointerEventData eventData)
    {
        if( AttemptStartDragging())
        {
            GameController.SelectedNewTile(this);
            ForceTooltip();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopDragging();
        if(GameController.CanPlaceTile())
        {
            GameController.PlaceTile();
        }
        else
        {
            MoveToTfData();
        }
        GameController.UnselectCurrentTile();
        StopForcingThisTooltip();
    }

    bool AttemptStartDragging()
    {
        if (!canBeMoved) { return false; }
        if (isBehindPlayer) { return false; }
        if (GameController_Simple.Instance.currentGameState == GameState.MovingPlayer) { MoveToTfData(); return false; }

        Camera mainCamera = Camera.main;

        draggingCoroutine = StartCoroutine(C_draggingCoroutine());
        return true;
        //
        IEnumerator C_draggingCoroutine()
        {
            while (true)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                Plane plane = new Plane(Vector3.up, Vector3.up * heightWhileDragged);

                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePosInPlane = ray.GetPoint(distance);
                    Debug.DrawLine(transform.position, mousePosInPlane);
                    transform.position = Vector3.MoveTowards(transform.position, mousePosInPlane, 40 * Time.deltaTime);
                }
                yield return null;
            }
        }
    }
    void StopDragging()
    {
        if(draggingCoroutine != null) { StopCoroutine(draggingCoroutine); }
    }
    #endregion
    
}
