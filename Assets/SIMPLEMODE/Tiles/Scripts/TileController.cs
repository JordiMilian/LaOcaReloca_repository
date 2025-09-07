using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour, IBuyable, ITooltip, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public TileState tileState = TileState.none;

    [HideInInspector] public int indexInBoard;
    [HideInInspector] public Vector2Int vectorInBoard;
    [SerializeField] Material tileMaterial;

    //Basic references to other scripts
    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    [HideInInspector] public TileSharedVisuals tileMovement;

    public Tile_Profile _Profile;

    #region NEW TF DATA
    [Header("Mesh references")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    public TileTfData TfData { get; private set; }
    bool isDataSet = false; //For gizmo drawing pls kill
    public void SetOriginTfData(TileTfData tileData)
    {
        TfData = tileData;
        isDataSet = true;
    }
    public void SetToTfData()
    {
        transform.position = TfData.center;
        transform.rotation = TfData.rotation;

        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(TfData.cornersInLocal);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }
    public void MoveToTfData()
    {
        StartCoroutine(moving());

        IEnumerator moving()
        {
            float timer = 0;
            const float movingTime = .5f;
            yield return new WaitForSeconds(Random.Range(0, movingTime));
            transform.DOMove(TfData.center, movingTime).SetEase(Ease.OutBack);

            while (timer < movingTime)
            {
                timer += Time.deltaTime;

                List<Vector3> newVerts = new();
                for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
                {
                    Vector3 targetPos = TfData.cornersInLocal[i];
                    Vector3 currentPos = meshFilter.mesh.vertices[i];
                    Vector3 lerpedPos = Vector3.Lerp(currentPos, targetPos, timer / movingTime);
                    newVerts.Add(lerpedPos);
                }
                meshFilter.mesh.SetVertices(newVerts);

                Quaternion targetRot = TfData.rotation;
                Quaternion currentRot = transform.rotation;
                Quaternion lerpedRot = Quaternion.Lerp(currentRot, targetRot, timer / movingTime);
                transform.rotation = lerpedRot;
                yield return null;
            }

            SetToTfData();
        }
    }
    #endregion
    public void SetTileProfile(Tile_Profile profile)
    {
        _Profile = Instantiate(profile);
        _Profile.Tile = this;
        tileMaterial.color = _Profile.tileColor;
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
                tileMovement.canBeMoved = true;
                break;
            case TileState.InBoard:
                if(_Profile is Tile_End || _Profile is Tile_Start) { tileMovement.canBeMoved = false; break; }
                tileMovement.canBeMoved = true;
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
            if (tile.GetType() == this.GetType()) { repeatedCards++; }
        }

        int baseValue;
        switch (_Profile.rarity)
        {
            case Rarity.Common: { baseValue = 2; break; }
            case Rarity.Rare: { baseValue = 4; break; }
            case Rarity.Legendary: { baseValue = 10; break; }
            default: { Debug.LogError("ERROR: Pls set a valid rarity to this Tile"); return 0; }
        }
        return MathJ.GetFibonacciValue(baseValue, repeatedCards);

    }
    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        SetTileProfile(GetRandomProfile());
        tileMovement.SetOriginTransformWithTransform(shopItemController.buyablePositionTf);
        tileMovement.PlaceTileInOrigin();
        SetTileState(TileState.InShop);

        //
        Tile_Profile GetRandomProfile()
        {
            Debug.LogError("TO DO");
            return null;
        }
    }
    public void OnEnablePurchase()
    {
        tileMovement.canBeMoved = true;
    }

    public void OnDisablePurchase()
    {
        tileMovement.canBeMoved = false;
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
    #endregion
}
