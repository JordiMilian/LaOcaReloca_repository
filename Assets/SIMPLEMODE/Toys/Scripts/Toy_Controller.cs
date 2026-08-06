using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Toy_Controller : MonoBehaviour, IBuyable, ITooltip
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Toy_Info _Profile;
    public bool isActive;
    public ToySlot currentSlot;
    public Transform originTf;
    bool isInShop;
    public UnityEvent OnAddedToBoard;
    //public TileState currentState = TileState.none;
    
    public void SetProfile(Toy_Info profile)
    {
        _Profile = profile;
        _Profile.InitializeProfile(this);

        if(_Profile.toyMesh != null)
        {
            GetComponent<MeshFilter>().mesh = _Profile.toyMesh;
        }
            
    }
    public void ActivateToy()
    {
        _Profile.OnActivatedToy();
        OnAddedToBoard?.Invoke();
    }
    public void DeactivateToy()
    {
        _Profile.OnDeactivatedToy();
    }

    #region BUYABLE
    public int GetBuyingPrice()
    {
        return 15;
    }

    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        //Pick a random profile and set it
        SetProfile(ToysManager.Instance.GetRandomToyProfile());
        originTf = shopItemController.transform;
        transform.position = originTf.position;
        isInShop = true;
        //currentState = TileState.InShop;
    }

    public void OnEnablePurchase()
    {
        isDraggable = true;
    }

    public void OnDisablePurchase()
    {
        isDraggable = false;
    }
    #endregion
    #region TOOLTIPS
    public string GetTooltipDescription()
    {
        return _Profile.GetTooltipDescription();
    }

    public string GetTooltipTitle()
    {
        return _Profile.Title;
    }

    public Texture GetTooltipTexture()
    {
        return ConfigsDatabase.GetToyConfigWithId(_Profile.configId)._texture;
    }
    public void OnPointerEnter(PointerEventData eventData) { RequestTooltip(); }
    public void OnPointerExit(PointerEventData eventData) { StopRequestTooltip(); }

    void StopRequestTooltip() { TooltipManager.Instance.RemoveRequest(this); }
    void RequestTooltip() { TooltipManager.Instance.RequestTooltip(this); }
    void ForceTooltip() { TooltipManager.Instance.ForceTooltip(this); }
    void StopForcingThisTooltip() { TooltipManager.Instance.StopForcingThisTooltip(this); }

    
    #endregion
    #region MOVEMENT
    public bool isDraggable = true;
    Coroutine dragginCoroutine;
    public void returnToyToOrigin()
    {
        transform.DOMove(originTf.transform.position,.5f).SetEase(Ease.OutBack);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;
        ForceTooltip();
        dragginCoroutine = StartCoroutine(C_draggingCoroutine());

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDraggable) return;
        StopForcingThisTooltip();
        if (dragginCoroutine != null) StopCoroutine(dragginCoroutine);


        CheckIfOverlappingSlot();


        void CheckIfOverlappingSlot()
        {
            Camera mainCamera = Camera.main;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitsArray;
            hitsArray = Physics.RaycastAll(ray);
            GameController_Simple gameController = GameController_Simple.Instance;
            foreach (RaycastHit hit in hitsArray)
            {
                if (hit.collider.TryGetComponent(out ToySlot slotUnder))
                {
                    if(isInShop)
                    {
                        if(gameController.CanPurchase(GetBuyingPrice()))
                        {
                            isInShop = false;
                            //currentState = TileState.InBoard;
                            gameController.RemoveMoney(GetBuyingPrice());
                            gameController.shopController.GetShopItem(this).RemoveItem();
                            slotUnder.OnPlacedToyInSlot(this);
                            return;
                        }
                        else
                        {
                            returnToyToOrigin();
                        }
                    }
                    else //not in shop
                    {
                        slotUnder.OnPlacedToyInSlot(this);
                        return;
                    } 
                }
            }
            returnToyToOrigin(); 
        }
    }
    IEnumerator C_draggingCoroutine()
    {
        Camera mainCamera = Camera.main;
        float heightWhileDragged = 2f;
        while (true)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.up * heightWhileDragged);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mousePosInPlane = ray.GetPoint(distance);
                transform.position = Vector3.MoveTowards(transform.position, mousePosInPlane, 40 * Time.deltaTime);
            }
            if (!isDraggable) yield break;
            yield return null;
        }
    }
    #endregion
}
