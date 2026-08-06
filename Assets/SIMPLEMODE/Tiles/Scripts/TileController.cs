using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Events;

public class TileController : MonoBehaviour, IBuyable, ITooltip
    ,IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

{
    [HideInInspector] public int indexInBoard;
    public Material tileMaterial;

    //Basic references to other scripts
    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    [HideInInspector] public TileSharedVisuals tileMovement;

    [SerializeField] Transform zeroRotationTf;

    public TileInfo _Info;

    public UnityEvent OnAddedToBoard; //currently used by encounters that trigger when you place a tile in board (curse per money)

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

            RecalculateTextureScaling();
            transform.DOMove(TfData.center, movingTime).SetEase(Ease.OutBack);
            
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
    #region BASIC SETUP
    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;
        tileMovement = GetComponent<TileSharedVisuals>();

        tileMaterial = Instantiate(tileMaterial);
        GetComponent<MeshRenderer>().material = tileMaterial;
    }
    public void SetTileProfile(TileInfo info)
    {
        _Info = info;
        _Info._Controller = this;
        tileMaterial.SetColor("_OutlineColor", _Info.tileColor);

        /* TEXTURE IS NOW STORED IN THE CONFIG
        if(_Info.tileTexture != null)
        {
            tileMaterial.SetTexture("_mainTexture", _Info.tileTexture);
        }
        */
        tileMovement.UpdateDmgDisplayText();
        _Info.Initialize();
    }
    #endregion
    #region SET MATERIAL TO LAND
    public void SetTileMaterial_ToLand()
    {
        tileMaterial.SetFloat("_sineScale", 1.05f);
        tileMaterial.SetFloat("_sineSpeed", 10f);
        tileMaterial.SetColor("_OutlineColor",_Info.tileColor * 8);
    }
    public void SetTileMaterial_Regular()
    {
        tileMaterial.SetFloat("_sineScale", 1f);
        tileMaterial.SetFloat("_sineSpeed", 0f);
        tileMaterial.SetColor("_OutlineColor", _Info.tileColor * 1);
    }
    #endregion
    #region DAMAGE MODIFIERS
    public List<float> DamagesToDeal = new();
    public Func<float, float> BaseDamageModifiers;
    public float GetModifiedBaseDamage()
    {
        if(BaseDamageModifiers == null) { return GetBaseDamage(); }
        float totalDmg = GetBaseDamage();
        foreach(Func<float,float> modifier in BaseDamageModifiers.GetInvocationList())
        {
            totalDmg = modifier(totalDmg);
        }
        return totalDmg;
    }
    public float GetBaseDamage()
    {
        return _Info.BaseDamage;
    }
    public void SetBaseDamage(float newDamage)
    {
        _Info.BaseDamage = newDamage;
        tileMovement.UpdateDmgDisplayText();
    }
    public virtual IEnumerator AddBaseDamage(float dmgToAdd)
    {
        float addedDmg = _Info.AddBaseDamage(dmgToAdd);
        
        tileMovement.shakeTile(Intensity.mid);
        yield return tileMovement.DisplayMessage("+" + MathJ.FloatToString(addedDmg, 1), TileMessageType.AddBaseDamage);
    }
    public IEnumerator RemoveBaseDamage(float damageToRemove)
    {
       float removedDmg = _Info.RemoveBaseDamage(damageToRemove);

        tileMovement.shakeTile(Intensity.mid);
        yield return tileMovement.DisplayMessage($"-{removedDmg}", TileMessageType.AddBaseDamage);
    }
    public IEnumerator C_MultiplyBaseDamage(float mult)
    {
        _Info.MultiplyBaseDamage(mult);

        tileMovement.shakeTile(Intensity.mid);
        yield return tileMovement.DisplayMessage($"x{mult}", TileMessageType.AddBaseDamage);
    }
    public IEnumerator C_DealAllDamageToDeal()
    {
        if(DamagesToDeal.Count == 0) { yield break; }

        float totalDamage = 0;

        string displayMessage = "";
        for (int i = 0; i < DamagesToDeal.Count; i++)
        {
            if(Mathf.Approximately(DamagesToDeal[i], 0)) { continue; }
            displayMessage += MathJ.FloatToString(DamagesToDeal[i], 1);
            if (i != DamagesToDeal.Count - 1) { displayMessage += "+"; }
        }
        yield return tileMovement.DisplayMessage(displayMessage, TileMessageType.DealDamage);

        foreach (float dmg in DamagesToDeal) { totalDamage += dmg; }
        yield return GameController.C_AddAcumulatedDamage(totalDamage);

        DamagesToDeal.Clear();
    }
    #endregion
    #region TILE STATE
    [HideInInspector] public TileState tileState = TileState.none;
    public void SetTileState(TileState newState)
    {
        if(newState == tileState) { return; }

        //EXIT
        switch(tileState)
        {
            case TileState.InBoard:
                BoardController.OnPlayerIndexSet.RemoveListener(CheckForDraggability);
                break;
            default: break;
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
                if(_Info is Tile_End || _Info is Tile_Start) { canBeMoved = false; break; }
                canBeMoved = true;
                BoardController.OnPlayerIndexSet.AddListener(CheckForDraggability);
                CheckForDraggability(0, BoardController.PlayerIndex);
                break;
            default: break;
        }
        tileState = newState;
    }
    #endregion
    #region CALL PROFILE LOGIC

    public IEnumerator OnPlayerStepped()
    {
        //Add visual and sound feedback
       yield return _Info.OnPlayerStepped();
    }
    public IEnumerator OnPlayerLanded()
    {
        //Add more visual and sound feedback
        tileMovement.shakeTile(Intensity.mid);

        yield return _Info.OnPlayerLanded(); 
    }
    public IEnumerator OnTileFinished() { yield return _Info.OnTileFinished(); }
    public IEnumerator C_OnPlacedInBoard() { yield return _Info.OnPlacedInBoard(); OnAddedToBoard?.Invoke(); }
    public IEnumerator C_OnRemovedFromBoard() { yield return _Info.OnRemovedFromBoard(); }

    #endregion
    #region BUY/SELL
    
    public virtual int GetBuyingPrice()
    {
        int repeatedCards = 0;
        foreach (TileController tile in BoardController.TilesList)
        {
            if (tile._Info.GetType() == _Info.GetType()) { repeatedCards++; }
        }

        int baseValue;
        switch (_Info.rarity)
        {
            case Rarity.Common: { baseValue = 4; break; }
            case Rarity.Rare: { baseValue = 7; break; }
            case Rarity.Legendary: { baseValue = 12; break; }
            case Rarity.Unique: { return _Info.uniquePrice; }
            default: { Debug.LogError("ERROR: Pls set a valid rarity to this Tile"); return 0; }
        }
        return MathJ.GetFibonacciValue(baseValue, repeatedCards);

    }
    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        //SetTileProfile(TilesFactory.instance.GetRandomProfile(null));
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
        return _Info.GetGenericSkillsText()+ _Info.GetTooltipText();
    }
    public string GetTooltipTitle()
    {
        return _Info.Title;
    }
    public Texture GetTooltipTexture()
    {
        return ConfigsDatabase.GetTileConfigWithId(_Info._configId)._texture;
        //return _Info.tileTexture;
    }
    #endregion
    #region DRAGGING
    public bool canBeMoved = true;
    Coroutine draggingCoroutine;
    [SerializeField] float heightWhileDragged = 1;
    [HideInInspector] public bool isBehindPlayer;
    public void CheckForDraggability(int from, int to)
    {
        isBehindPlayer = BoardController.PlayerIndex >= indexInBoard;
        if (isBehindPlayer) { tileMovement.SetBasicPanelColor_Transparent(); }
        else { tileMovement.SetBasicPanelColor(); }
    }
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
        if (_Info.genericSkills.Contains(GenericSkills.Unmovable) && tileState == TileState.InBoard) { return false; }
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
