using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Toy_Controller : MonoBehaviour, IBuyable, ITooltip
    , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Toy_Profile _Profile;
    public bool isActive;
    public ToySlot currentSlot;
    
    public void SetProfile(Toy_Profile profile)
    {
        _Profile = Instantiate(profile);
        _Profile.InitializeProfile(this);

        GetComponent<MeshFilter>().mesh = _Profile.toyMesh;
    }
    public void ActivateToy()
    {
        _Profile.OnActivatedToy();
    }
    public void DeactivateToy()
    {
        _Profile.OnDeactivatedToy();
    }

    #region BUYABLE
    public int GetBuyingPrice()
    {
        return 50;
    }

    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
        //Pick a random profile and set it
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
        return _Profile.Name;
    }

    public Texture GetTooltipTexture()
    {
        return null;
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
    public void returnToyToSlot()
    {
        transform.DOMove(currentSlot.transform.position,.2f).SetEase(Ease.OutBack);
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
            foreach (RaycastHit hit in hitsArray)
            {
                if (hit.collider.TryGetComponent(out ToySlot slotUnder))
                {
                    slotUnder.OnPlacedToyInSlot(this);
                    return;
                }
            }
            if (isActive) { returnToyToSlot(); }
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
